using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeroftheMagic.Scripts.Interactables;
using TeroftheMagic.Scripts.Universe;
using TeroftheMagic.Scripts.Utility;
using static TeroftheMagic.Scripts.Utility.Functions;
using static TeroftheMagic.Scripts.Utility.TileUtil;
using Logger = TeroftheMagic.Scripts.Utility.Logger;

namespace TeroftheMagic.Scripts;

public partial class Game : Node2D {
	public static Game Instance { get; private set; }
	[Export] private CharacterBody2D player;
	public static CharacterBody2D Player { get => Instance.player; }
	public static PlayerInventory PlayerInventory { get => PlayerInventory.Instance; }
	public static readonly byte ppPerTick = 2;
	public static readonly byte tickMs = (byte)Math.Round(ppPerTick * 1000f / Engine.PhysicsTicksPerSecond);
	public static List<Task> GenTasks = [];
	public static bool loaded = false;
	public static Vector2I WorldChunks;
	public static ushort WorldWidthChunks = 100;
	public static ushort WorldHeightChunks = 20;
	public static uint WorldWidth { get => (uint)(WorldChunks.X * WorldData.chunkSize); }
	public static uint WorldHeight { get => (uint)(WorldChunks.Y * WorldData.chunkSize); }
	public static uint WorldWidthPx { get => WorldWidth * TilePixelSize; }
	public static uint WorldHeightPx {  get => WorldHeight * TilePixelSize; }
	private static byte minHeight = 75;
	public static byte MinHeight { get => minHeight; set => minHeight = value; }
	private static byte maxHeight = 85;
	public static byte MaxHeight { get => maxHeight; set => maxHeight = value; }
	private static byte smoothIterations = 5;
	public static byte SmoothIterations { get => smoothIterations; set => smoothIterations = value; }
	private static int seed = 69;
	public static int Seed { get => seed; set => seed = value; }
	public static Random random;
	public static readonly FastNoiseLite noise = new();
	private static float surfaceFrequency = .5f;
	/// <summary>
	/// Wie die Frequenz einer Sinuskurve
	/// </summary>
	public static float SurfaceFrequency { get => surfaceFrequency; set => surfaceFrequency = value; }
	private static float caveMod = .5f;
	/// <summary>
	/// Größenfaktor der Höhlen
	/// </summary>
	public static float CaveMod { get => caveMod; set => caveMod = value; }
	private static byte caveThreshold = 60;
	/// <summary>
	/// Grenzwert für Höhlengenerierung (0-100)
	/// 0 = alles Höhle, 100 = keine Höhle
	/// </summary>
	public static byte CaveThreshold { get => caveThreshold; set => caveThreshold = value; }

	public static bool initGen;

	private const byte treeChance = 10;
	private const byte minTreeHeight = 7;
	private const byte maxTreeHeight = 23;
	private const byte branchChance = 20;
	private System.Diagnostics.Stopwatch s;

	/// <summary>
	/// Render distance in Chunks.
	/// (1,1) means the 3x3 chunks surrounding the Player will be rendered.
	/// </summary>
	public static Vector2I RenderDistance = new(1, 1);
	/// <summary>
	/// List of Chunk IDs
	/// </summary>
	private static List<Vector2I> RenderedChunks = [];


	public override void _Ready() {
		// Called every time the node is added to the scene.
		// Initialization here.
		GD.Print("Hello from C# to Godot :)");
		Instance = this;
		noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
		World.Back = GetNode<Node2D>("World/BackLayer");
		World.Main = GetNode<Node2D>("World/MainLayer");
		World.Front = GetNode<Node2D>("World/FrontLayer");
		World.Entities = GetNode<Node2D>("World/Entities");
		World.Borders = GetNode<Node2D>("World/Borders");
		Init();
	}

	public void Init() {
		RenderedChunks = [];
		WorldChunks = new(WorldWidthChunks, WorldHeightChunks);
		s = System.Diagnostics.Stopwatch.StartNew();
		loaded = false;
		random = new(seed);
		noise.Seed = seed;
		initGen = true;
		World.New(WorldChunks);
		Task.WaitAll([.. GenTasks]);
		// foreach (Task task in GenTasks) {
		//     GD.Print($"Task {task.Id} Status: {task.Status}");
		// }
		GenTasks.Clear();
		initGen = false;
		GD.Print($"Time for Gen: {s.ElapsedMilliseconds}ms");
		s.Restart();
		SmoothWorld();
		GD.Print($"Time for Smooth: {s.ElapsedMilliseconds}ms");
		s.Restart();
		GrowMoss();
		GD.Print($"Time for Moss: {s.ElapsedMilliseconds}ms");
		s.Restart();
		PlantTrees();
		GD.Print($"Time for Trees: {s.ElapsedMilliseconds}ms");
		s.Restart();
		GrowVegetation();
		GD.Print($"Time for Vegetation: {s.ElapsedMilliseconds}ms");
		s.Restart();
		World.Load();
		Task.WaitAll([.. GenTasks]);
		GenTasks.Clear();
		loaded = true;
		GD.Print($"Time for Load: {s.ElapsedMilliseconds}ms");
		s.Restart();
		SpawnPlayer();
		RenderChunks();
		GD.Print($"Time for Render: {s.ElapsedMilliseconds}ms");
		s.Restart();
	}

	private static void GrowVegetation() {
		GrowGrass();
	}

	private static void GrowGrass() {
		for (ushort x = 0; x < WorldData.size.X; x++) {
			var pos = new Vector2I(x, WorldData.heightMap[x]);
			if (WorldData.main[pos].ID == Block.Air && WorldData.main[pos + Vector2I.Up].ID == Block.Moss) {
				WorldData.main[pos] = new("totm:grass_plant");
			}
		}
	}

	private static void SmoothWorld() {
		for (int i = 0; i < SmoothIterations; i++) {
			World.SmoothWorld();
			Task.WaitAll([.. GenTasks]);
			GenTasks.Clear();
		}
	}

	private static void GrowMoss() {
		Vector2I pos = new();
		for (pos.X = 0; pos.X < WorldData.size.X; pos.X++) {
			for (pos.Y = 0; pos.Y < WorldData.heightMap[pos.X]; pos.Y++) {
				if (WorldData.main[pos].ID == "totm:dirt" && World.SurroundingGround(pos) < 8)
					WorldData.main[pos].ID = "totm:moss";
			}
		}
	}

	private static void PlantTrees() {
		for (ushort x = 1; x < WorldData.size.X - 2; x++) {
			Vector2I idPos = new(x, WorldData.heightMap[x]);
			while (WorldData.main[idPos].ID == Block.Air) {
				idPos.Y--;
			}
			idPos.Y++;
			if (random.Next(100) < treeChance && TryPlantTree(idPos))
				x += 2;
		}
	}

	public static bool TryPlantTree(Vector2I pos) {
		Vector2I temp = pos;
		temp.Y--;
		// Check for Moss ground
		if (WorldData.main[temp + Vector2I.Left].ID != "totm:moss" ||
			WorldData.main[temp].ID != "totm:moss" ||
			WorldData.main[temp + Vector2I.Right].ID != "totm:moss")
			return false;

		byte height = (byte)random.Next(minTreeHeight, maxTreeHeight + 1);
		// Check if fits
		if (pos.Y + height > WorldData.size.Y)
			return false;

		// Check for space
		for (temp.Y += 1; temp.Y < pos.Y + height; temp.Y++) {
			if (WorldData.main[temp + Vector2I.Left].ID != Block.Air ||
				WorldData.main[temp].ID != Block.Air ||
				WorldData.main[temp + Vector2I.Right].ID != Block.Air)
				return false;
		}

		PlantTree(pos, height);

		return true;
	}

	private static void PlantTree(Vector2I pos, byte height) {
		// Place Stump
		WorldData.main[pos] = new("totm:log", "stump");

		// Place Logs
		bool left, right, lastLeft = false, lastRight = false;
		for (int y = 1; y < height - 3; y++) {
			pos.Y++;
			left = !lastLeft && random.Next(100) < branchChance;
			right = !lastRight && random.Next(100) < branchChance;
			lastLeft = left;
			lastRight = right;
			if (!left && !right) {
				WorldData.main[pos] = new("totm:log");
			}
			else if (left && !right) {
				WorldData.main[pos] = new("totm:log", "left");
				WorldData.main[pos + Vector2I.Left] = new("totm:branch", "left");
			}
			else if (!left && right) {
				WorldData.main[pos] = new("totm:log", "right");
				WorldData.main[pos + Vector2I.Right] = new("totm:branch", "right");
			}
			else if (left && right) {
				WorldData.main[pos] = new("totm:log", "both");
				WorldData.main[pos + Vector2I.Left] = new("totm:branch", "left");
				WorldData.main[pos + Vector2I.Right] = new("totm:branch", "right");
			}
		}

		// Place Crown
		pos.Y++;
		WorldData.main[pos] = new("totm:leaves", "bottom");
		WorldData.main[pos + Vector2I.Left] = new("totm:leaves", "bottomleft");
		WorldData.main[pos + Vector2I.Right] = new("totm:leaves", "bottomright");
		pos.Y++;
		WorldData.main[pos] = new("totm:leaves");
		WorldData.main[pos + Vector2I.Left] = new("totm:leaves", "left");
		WorldData.main[pos + Vector2I.Right] = new("totm:leaves", "right");
		pos.Y++;
		WorldData.main[pos] = new("totm:leaves", "top");
		WorldData.main[pos + Vector2I.Left] = new("totm:leaves", "topleft");
		WorldData.main[pos + Vector2I.Right] = new("totm:leaves", "topright");
	}

	public Node2D GetPlayer() => GetNode<Node2D>("World/Entities/Player");

	public void SpawnPlayer() {
		Node2D player = GetPlayer();
		Vector2I mapPos = new(
			WorldData.size.X / 2,
			WorldData.size.Y - 1
		);
		while (IsAir(WorldLayer.main, mapPos))
			mapPos.Y--;
		Vector2 pos = mapPos * 16;
		pos.Y = -pos.Y;
		player.Position = pos;
	}

	public static void RenderChunks() {
		// Unrender all chunks first, since they start off rendered
		foreach (WorldChunk chunk in WorldData.main.chunks) {
			if (IsInRenderDistance(chunk.ID)) {
				RenderedChunks.Add(chunk.ID);
				if (!chunk.TML.Enabled) {
					chunk.TML.Render();
					WorldData.back.chunks[chunk.ID.X, chunk.ID.Y].TML.Render();
				}
			}
			else if (chunk.TML.Enabled) {
				chunk.TML.Unrender();
				WorldData.back.chunks[chunk.ID.X, chunk.ID.Y].TML.Unrender();
			}
		}
	}

	public override void _Process(double delta) {
		base._Process(delta);
		if (!loaded) return;
		if (s.IsRunning) {
			GD.Print($"Time for Frame: {s.ElapsedMilliseconds}ms");
			s.Stop();
		}
		if (Input.IsPhysicalKeyPressed(Key.L))
			Logger.LogCurrentMax();
		if (Input.IsActionJustPressed("ToggleInventory")) {
			Control UIInventory = (Control)PlayerInventory.GetGrandparent();
			UIInventory.Visible = !UIInventory.Visible;
		}
	}

	private static readonly byte ttu = 12;
	private byte ppCounter = ttu;

	public static bool IsInRenderDistance(Vector2I chunkID) {
		Vector2I pChunkID = ((PlayerMovement)Player).CurrentChunkID;
		if (pChunkID.X >= RenderDistance.X && pChunkID.X < WorldChunks.X - RenderDistance.X) {
			return
				Math.Abs(pChunkID.X - chunkID.X) <= RenderDistance.X &&
				Math.Abs(pChunkID.Y - chunkID.Y) <= RenderDistance.Y;
		}
		else {
			return
				!(chunkID.X > (pChunkID.X + RenderDistance.X).Mod(WorldChunks.X)
				&& chunkID.X < (pChunkID.X - RenderDistance.X).Mod(WorldChunks.X))
				&& Math.Abs(pChunkID.Y - chunkID.Y) <= RenderDistance.Y;
		}
	}

	public override void _PhysicsProcess(double delta) {
		base._PhysicsProcess(delta);
		ppCounter++;
		if (ppCounter >= ttu) {
			// GD.Print("Render Check");

			Vector2I pChunkID = World.GetChunkID(Player.Position);
			if (pChunkID == ((PlayerMovement)Player).CurrentChunkID)
				return;
			else
				((PlayerMovement)Player).CurrentChunkID = pChunkID;

			for (int i = RenderedChunks.Count - 1; i >= 0; i--) {
				Vector2I chunkID = RenderedChunks[i];
				if (!IsInRenderDistance(chunkID)) {
					WorldData.main.chunks[chunkID.X, chunkID.Y].TML.Unrender();
					WorldData.back.chunks[chunkID.X, chunkID.Y].TML.Unrender();
					RenderedChunks.RemoveAt(i);
				}
			}

			int x = (pChunkID.X - RenderDistance.X).Mod(WorldChunks.X);
			int y = Math.Max(pChunkID.Y - RenderDistance.Y, 0);
			int min = x;
			int max = Math.Min(pChunkID.Y + RenderDistance.Y, WorldChunks.Y - 1);

			while (y <= max) {
				if (!WorldData.main.chunks[x, y].TML.Enabled) {
					WorldData.main.chunks[x, y].TML.Render();
					WorldData.back.chunks[x, y].TML.Render();
					RenderedChunks.Add(new(x, y));
				}
				x = (++x).Mod(WorldChunks.X);
				if (!IsInRenderDistance(new(x, y))) {
					x = min;
					y++;
				}
			}

			// for (int i = 0; i < WorldChunks.X; i++) {
			// 	for (int ii = 0; ii < WorldChunks.Y; ii++) {
			// 		TileMapLayerController tml = WorldData.main.chunks[i, ii].TML;
			// 		float distance = (pos / TilePixelSizeV - tml.Center).Length();
			// 		if (distance <= RenderDistance && !tml.Enabled) {
			// 			tml.Render();
			// 			WorldData.back.chunks[i, ii].TML.Render();
			// 		}
			// 		else if (distance > RenderDistance && tml.Enabled) {
			// 			tml.Unrender();
			// 			WorldData.back.chunks[i, ii].TML.Unrender();
			// 		}
			// 	}
			// }
			ppCounter -= ttu;
		}
	}
	public enum DamageType {
		Fire, Ice, Lightning, Thunder, Poison, Acid, Holy, Unholy, Arcane, Force
	}
}
