using Godot;
using System;
using TeroftheMagic.Scripts.Utility;
using static TeroftheMagic.Scripts.Utility.Functions;

namespace TeroftheMagic.Scripts;

public partial class Game : Node2D {
    private static Game instance;
    public static Game Instance { get => instance; }
    public static World world;
    private static byte minHeight = 75;
    public static byte MinHeight { get => minHeight; set => minHeight = value; }
    private static byte maxHeight = 85;
    public static byte MaxHeight { get => maxHeight; set => maxHeight = value; }
    private static byte smoothIterations = 12;
    public static byte SmoothIterations { get => smoothIterations; set => smoothIterations = value; }
    private static int seed = 69;
    public static int Seed { get => seed; set => seed = value; }
    private static Random random;
    private static readonly FastNoiseLite noise = new();
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

    private static bool initGen;

    public override void _Ready() {
        // Called every time the node is added to the scene.
        // Initialization here.
        GD.Print("Hello from C# to Godot :)");
        instance = this;
        noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
        CallDeferred("Init");
    }

    public void Init() {
        GenerateNewWorld();
        world.back = GetNode<TileMapLayer>("World/BackLayer");
        world.main = GetNode<TileMapLayer>("World/MainLayer");
        world.front = GetNode<TileMapLayer>("World/FrontLayer");
        LoadWorld();
        SpawnPlayer();
    }

    public static void GenerateNewWorld() {
        random = new(seed);
        noise.Seed = seed;
        world = new(1000, 500);
        GenerateHeightMap();
        initGen = true;
        for (ushort x = 0; x < world.data.size.X; x++) {
            for (ushort y = 0; y < world.data.heightMap[x]; y++) {
                ushort material = GetMaterial(x, y);
                if (!IsCave(x, y))
                    world.data.main[x, y] = material;
                world.data.back[x, y] = material;
            }
        }
        initGen = false;
        SmoothWorld();
        GrowMoss();
    }

    public static void GenerateHeightMap() {
        for (ushort x = 0; x < world.data.size.X; x++) {
            world.data.heightMap[x] = (ushort)ValueMap(
                0,
                1,
                PercentToWorldHeight(minHeight),
                PercentToWorldHeight(maxHeight),
                noise.GetNoise1D(x * heightMod) + .5f
            );
        }
    }

    public static ushort GetMaterial(ushort x, ushort y) {
        if (initGen && y < 5)
            return (ushort)(random.Next(0, 5) >= y ? 5 : 4);
        else if (y < world.data.heightMap[x] / 4)
            return 4;
        else if (y < world.data.heightMap[x] * 3.5f / 4)
            return 3;
        else if (y < world.data.heightMap[x])
            return 2;
        return 0;
    }

    public static bool IsCave(ushort x, ushort y) {
        Vector2I max = world.data.size;
        if (GetMaterial(x, y) == 5) // Bedrock
            return false;
        if (IsOnEdge(x, y) && GetMaterial(x, y) != 2)
            return false;
        byte r = (byte)Math.Round((noise.GetNoise2D(x / caveMod, y / caveMod) + .5f) * 100);
        // float r = random.Next(0, 100);
        // r /= 1 + y / 600f;
        return r > caveThreshold;
    }

    public static void SmoothWorld() {
        WorldLayerData temp = world.data.main.Clone();
        for (int i = 0; i < smoothIterations; i++) {
            for (ushort x = 1; x < world.data.size.X - 1; x++) {
                for (ushort y = 1; y < world.data.heightMap[x]; y++) {
                    if (world.data.main[x, y] == 5) { // Bedrock
                        temp[x, y] = 5;
                        continue;
                    }
                    temp[x, y] = SurroundingGround(x, y) switch {
                        < 4 => 0,
                        > 4 => GetMaterial(x, y),
                        _ => world.data.main[x, y],
                    };
                }
            }
            world.data.main = temp.Clone();
        }
    }

    public static byte SurroundingGround(ushort x, ushort y) {
        Vector2I max = world.data.size;
        // if (IsOnEdge(x, y))
        // 	return 4;
        byte count = 0;
        for (int X = x - 1; X <= x + 1; X++) {
            for (int Y = y - 1; Y <= y + 1; Y++) {
                if (X == x && Y == y)
                    continue;
                if (IsOutOfBounds(X, Y))
                    count++;
                else if (world.data.main[X, Y] != 0)
                    count++;
            }
        }
        return count;
    }

    public static void GrowMoss() {
        for (ushort x = 0; x < world.data.size.X; x++) {
            for (ushort y = 0; y < world.data.heightMap[x]; y++) {
                if (world.data.main[x, y] == 2 && SurroundingGround(x, y) < 8)
                    world.data.main[x, y] = 1;
            }
        }
    }

    public static void LoadWorld() {
        Vector2I pos = new();
        for (pos.Y = 0; pos.Y < world.data.size.Y; pos.Y++) {
            for (pos.X = 0; pos.X < world.data.size.X; pos.X++) {
                world.main.SetCell(pos, 1, TileMapIdToCoord(world.data.main[pos]));
                world.back.SetCell(pos, 1, TileMapIdToCoord(world.data.back[pos]), 1);
            }
        }
    }

    public void SpawnPlayer() {
        Node2D player = GetNode<Node2D>("Player");
        Vector2 pos = new();
        Vector2I idPos = new();
        idPos.X = world.data.size.X / 2;
        idPos.Y = world.data.size.Y - 1;
        while (world.data.main[idPos] == 0)
            idPos.Y--;
        pos = idPos * 16;
        pos.Y = -pos.Y;
        player.Position = pos;
    }

    public static void BreakBlock(bool backLayer = false) {
        Vector2I mapPos = world.main.LocalToMap(world.main.GetLocalMousePosition());
        if (IsOutOfBounds(mapPos) || world.data.main[mapPos] == 5)
            return;
        if (!backLayer) {
            if (world.data.main[mapPos] == 0)
                return;
            world.data.main[mapPos] = 0;
            world.main.SetCell(mapPos);
        }
        else {
            if (world.data.main[mapPos] != 0)
                return;
            world.data.back[mapPos] = 0;
            world.back.SetCell(mapPos);
        }
    }

    public static void PlaceBlock(bool backLayer) {
        Vector2I mapPos = world.main.LocalToMap(world.main.GetLocalMousePosition());
        if (IsOutOfBounds(mapPos) || world.data.back[mapPos] == 5)
            return;
        if (!backLayer) {
            if (world.data.main[mapPos] != 0)
                return;
            world.data.main[mapPos] = 3;
            world.main.SetCell(mapPos, 1, TileMapIdToCoord(3));
        }
        else {
            if (world.data.back[mapPos] != 0)
                return;
            world.data.back[mapPos] = 3;
            world.back.SetCell(mapPos, 1, TileMapIdToCoord(3), 1);
        }
    }
}
