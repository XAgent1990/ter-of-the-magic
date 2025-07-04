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
	[Export] private Inventory playerInventory;
	public static Inventory PlayerInventory { get => Instance.playerInventory; }
	public static readonly byte ppPerTick = 2;
	public static readonly byte tickMs = (byte)Math.Round(ppPerTick * 1000f / Engine.PhysicsTicksPerSecond);
	public static List<Task> GenTasks = [];
	public static bool loaded = false;
	private static Vector2I worldChunks = new(1, 1);
	public static ushort WorldWidth { get => (ushort)worldChunks.X; set => worldChunks.X = value; }
	public static ushort WorldHeight { get => (ushort)worldChunks.Y; set => worldChunks.Y = value; }
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
	public static Vector2I Up = Vector2I.Down;
	public static Vector2I Down = Vector2I.Up;
	public static Vector2I Left = Vector2I.Left;
	public static Vector2I Right = Vector2I.Right;


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
		GD.Print($"Time for Gen: {s.ElapsedMilliseconds}ms");
		s.Restart();
		SmoothWorld();
		GD.Print($"Time for Smooth: {s.ElapsedMilliseconds}ms");
		s.Restart();
		GrowMoss();
		GD.Print($"Time for Moss: {s.ElapsedMilliseconds}ms");
		s.Restart();
		SpawnPlayer();
		PlantTrees();
		GD.Print($"Time for Trees: {s.ElapsedMilliseconds}ms");
		s.Restart();
		World.Load();
		Task.WaitAll([.. GenTasks]);
		GenTasks.Clear();
		loaded = true;
		GD.Print($"Time for Load: {s.ElapsedMilliseconds}ms");
		s.Restart();
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
		if (Input.IsActionJustPressed("ToggleInventory"))
			PlayerInventory.Visible = !PlayerInventory.Visible;
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
			Vector2I idPos = new(x, WorldData.size.Y - minTreeHeight);
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

	public static void BreakBlock(WorldLayer layer) {
		Logger.StartTimer("Game.BreakBlock");
		Vector2 mousePos = World.Main.ToGlobal(World.Main.GetLocalMousePosition());
		Vector2I mapPos = new((int)(mousePos.X / 16), (int)Math.Ceiling(-mousePos.Y / 16));
		if (IsOutOfBounds(mapPos) || IsUnbreakable(layer, mapPos))
			return;
		if (layer == WorldLayer.back && !IsAir(WorldLayer.main, mapPos))
			return;
		World.BreakBlock(layer, mapPos);
		Logger.StopTimer("Game.BreakBlock");
	}

	public static void PlaceBlock(WorldLayer layer) {
		Vector2 mousePos = World.Main.ToGlobal(World.Main.GetLocalMousePosition());
		Vector2I mapPos = new((int)(mousePos.X / 16), (int)Math.Ceiling(-mousePos.Y / 16));
		if (IsOutOfBounds(mapPos) || !IsAir(layer, mapPos))
			return;
		World.PlaceBlock(layer, mapPos, new("totm:stone"));
	}
}
