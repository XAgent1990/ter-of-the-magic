using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeroftheMagic.Scripts.Utility;
using static TeroftheMagic.Scripts.Utility.Functions;
using Logger = TeroftheMagic.Scripts.Utility.Logger;

namespace TeroftheMagic.Scripts;

public partial class Game : Node2D {
	private static Game instance;
	public static Game Instance { get => instance; }
	public static readonly byte ppPerTick = 2;
	public static readonly byte tickMs = (byte)Math.Round(ppPerTick * 1000f / Engine.PhysicsTicksPerSecond);
	public static List<Task> GenTasks = new();
	public static bool loaded = false;
	private static Vector2I worldChunks = new(1, 1);
	public static ushort WorldWidth { get => (ushort)worldChunks.X; set => worldChunks.X = value; }
	public static ushort WorldHeight { get => (ushort)worldChunks.Y; set => worldChunks.Y = value; }
	private static byte minHeight = 75;
	public static byte MinHeight { get => minHeight; set => minHeight = value; }
	private static byte maxHeight = 85;
	public static byte MaxHeight { get => maxHeight; set => maxHeight = value; }
	private static byte smoothIterations = 2;
	public static byte SmoothIterations { get => smoothIterations; set => smoothIterations = value; }
	private static int seed = 69;
	public static int Seed { get => seed; set => seed = value; }
	public static Random random;
	public static readonly FastNoiseLite noise = new();
	private static float heightMod = .5f;
	/// <summary>
	/// Wie die Frequenz einer Sinuskurve
	/// </summary>
	public static float HeightMod { get => heightMod; set => heightMod = value; }
	private static float caveMod = .5f;
	/// <summary>
	/// Zoomfaktor der Höhlen
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

	public override void _Ready() {
		// Called every time the node is added to the scene.
		// Initialization here.
		GD.Print("Hello from C# to Godot :)");
		instance = this;
		noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
		World.Back = GetNode<Node2D>("World/BackLayer");
		World.Main = GetNode<Node2D>("World/MainLayer");
		World.Front = GetNode<Node2D>("World/FrontLayer");
		Init();
	}
	public void Init() {
		s = System.Diagnostics.Stopwatch.StartNew();
		loaded = false;
		random = new(seed);
		noise.Seed = seed;
		initGen = true;
		World.New(worldChunks);
		Task.WaitAll([.. GenTasks]);
		// foreach (Task task in GenTasks) {
		//     GD.Print($"Task {task.Id} Status: {task.Status}");
		// }
		GenTasks.Clear();
		initGen = false;
		GD.Print($"Time for after Gen: {s.ElapsedMilliseconds}");
		SmoothWorld();
		GD.Print($"Time for after Smooth: {s.ElapsedMilliseconds}");
		GrowMoss();
		GD.Print($"Time for after Moss: {s.ElapsedMilliseconds}");
		PlantTrees();
		GD.Print($"Time for after Trees: {s.ElapsedMilliseconds}");
		World.Load();
		Task.WaitAll([.. GenTasks]);
		GenTasks.Clear();
		SpawnPlayer();
		loaded = true;
		GD.Print($"Time for after Load: {s.ElapsedMilliseconds}");
	}

	public override void _Process(double delta) {
		base._Process(delta);
		if (!loaded) return;
		if (s.IsRunning) {
			GD.Print($"Time for after Frame: {s.ElapsedMilliseconds}");
			s.Stop();
		}
		if (Input.IsPhysicalKeyPressed(Key.L))
			Logger.LogCurrentMax();
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
				if (WorldData.main[pos].id == 2 && World.SurroundingGround(pos) < 8)
					WorldData.main[pos].id = 1;
			}
		}
	}

	private static void PlantTrees() {
		for (ushort x = 1; x < WorldData.size.X - 2; x++) {
			Vector2I idPos = new(x, WorldData.size.Y - minTreeHeight);
			while (WorldData.main[idPos].id == 0) {
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
		if (WorldData.main[temp + Vector2I.Left].id != 1 ||
			WorldData.main[temp].id != 1 ||
			WorldData.main[temp + Vector2I.Right].id != 1)
			return false;

		byte height = (byte)random.Next(minTreeHeight, maxTreeHeight + 1);
		// Check if fits
		if (pos.Y + height > WorldData.size.Y)
			return false;

		// Check for space
		for (temp.Y += 1; temp.Y < pos.Y + height; temp.Y++) {
			if (WorldData.main[temp + Vector2I.Left].id != 0 ||
				WorldData.main[temp].id != 0 ||
				WorldData.main[temp + Vector2I.Right].id != 0)
				return false;
		}

		PlantTree(pos, height);

		return true;
	}

	private static void PlantTree(Vector2I pos, byte height) {
		// Place Stump
		WorldData.main[pos] = new(TileSetId.tree, 9);

		// Place Logs
		bool left, right, lastLeft = false, lastRight = false;
		for (int y = 1; y < height - 3; y++) {
			pos.Y++;
			left = !lastLeft && random.Next(100) < branchChance;
			right = !lastRight && random.Next(100) < branchChance;
			lastLeft = left;
			lastRight = right;
			if (!left && !right) {
				WorldData.main[pos] = new(TileSetId.tree, 6);
			}
			else if (left && !right) {
				WorldData.main[pos] = new(TileSetId.tree, 3);
				WorldData.main[pos + Vector2I.Left] = new(TileSetId.tree, 7);
			}
			else if (!left && right) {
				WorldData.main[pos] = new(TileSetId.tree, 3, 1);
				WorldData.main[pos + Vector2I.Right] = new(TileSetId.tree, 7, 1);
			}
			else if (left && right) {
				WorldData.main[pos] = new(TileSetId.tree, 8);
				WorldData.main[pos + Vector2I.Left] = new(TileSetId.tree, 7);
				WorldData.main[pos + Vector2I.Right] = new(TileSetId.tree, 7, 1);
			}
		}

		// Place Crown
		pos.Y++;
		WorldData.main[pos] = new(TileSetId.tree, 5);
		WorldData.main[pos + Vector2I.Left] = new(TileSetId.tree, 4);
		WorldData.main[pos + Vector2I.Right] = new(TileSetId.tree, 4, 1);
		pos.Y++;
		WorldData.main[pos] = new(TileSetId.tree, 2);
		WorldData.main[pos + Vector2I.Left] = new(TileSetId.tree, 1);
		WorldData.main[pos + Vector2I.Right] = new(TileSetId.tree, 1, 1);
		pos.Y++;
		WorldData.main[pos] = new(TileSetId.tree, 1, 2);
		WorldData.main[pos + Vector2I.Left] = new(TileSetId.tree, 4, 2);
		WorldData.main[pos + Vector2I.Right] = new(TileSetId.tree, 4, 3);
	}

	public void SpawnPlayer() {
		Node2D player = GetNode<Node2D>("Player");
		Vector2I mapPos = new(
			WorldData.size.X / 2,
			WorldData.size.Y - 1
		);
		TileData td = WorldData.main[mapPos];
		while (td.id == 0 || td.sourceId != TileSetId.main) {
			mapPos.Y--;
			td = WorldData.main[mapPos];
		}
		Vector2 pos = mapPos * 16;
		pos.Y = -pos.Y;
		player.Position = pos;
	}

	public static void BreakBlock(bool backLayer = false) {
		Logger.StartTimer("Game.BreakBlock");
		Vector2 mousePos = World.Main.ToGlobal(World.Main.GetLocalMousePosition());
		Vector2I mapPos = new((int)(mousePos.X / 16), (int)Math.Ceiling(-mousePos.Y / 16));
		if (IsOutOfBounds(mapPos) || IsBedrock(mapPos))
			return;
		if (!backLayer) {
			if (IsAir(mapPos))
				return;
			World.BreakBlock(WorldLayer.main, mapPos);
		}
		else {
			if (!IsAir(mapPos))
				return;
			World.BreakBlock(WorldLayer.back, mapPos);
		}
		Logger.StopTimer("Game.BreakBlock");
	}

	public static void PlaceBlock(bool backLayer) {
		Vector2 mousePos = World.Main.GetLocalMousePosition();
		Vector2I mapPos = new((int)(mousePos.X / 16), (int)(mousePos.Y / 16));

		mapPos.Y *= -1;
		if (IsOutOfBounds(mapPos) || IsBedrock(mapPos))
			return;
		if (!backLayer) {
			if (WorldData.main[mapPos].id != 0)
				return;
			WorldData.main[mapPos] = new(TileSetId.main, 3);
			UpdateCell(WorldLayer.main, mapPos);
		}
		else {
			if (WorldData.back[mapPos].id != 0)
				return;
			WorldData.main[mapPos] = new(TileSetId.main, 3, 1);
			UpdateCell(WorldLayer.back, mapPos);
		}
	}
}
