using Godot;
using System;
using System.Threading.Tasks;
using TeroftheMagic.Scripts.Utility;
using static TeroftheMagic.Scripts.Utility.Functions;
using TileData = TeroftheMagic.Scripts.Utility.TileData;

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

    private const byte treeChance = 10;
    private const byte minTreeHeight = 7;
    private const byte maxTreeHeight = 23;
    private const byte branchChance = 20;

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
                    world.data.main.SetTileData(x, y, new(TileSetId.main, material));
                world.data.back.SetTileData(x, y, new(TileSetId.main, material, 1));
            }
        }
        initGen = false;
        SmoothWorld();
        GrowMoss();
        PlantTrees();
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

    private static void SmoothWorld() {
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

    public static byte SurroundingGround(Vector2I pos) => SurroundingGround((ushort)pos.X, (ushort)pos.Y);
    public static byte SurroundingGround(ushort x, ushort y) {
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

    private static void GrowMoss() {
        Vector2I pos = new();
        for (pos.X = 0; pos.X < world.data.size.X; pos.X++) {
            for (pos.Y = 0; pos.Y < world.data.heightMap[pos.X]; pos.Y++) {
                if (world.data.main[pos] == 2 && SurroundingGround(pos) < 8)
                    world.data.main[pos] = 1;
            }
        }
    }

    private static void PlantTrees() {
        for (ushort x = 1; x < world.data.size.X - 1; x++) {
            Vector2I idPos = new(x, world.data.size.Y - 1);
            while (world.data.main[idPos] == 0) {
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
        if (world.data.main[temp.X - 1, temp.Y] != 1 ||
            world.data.main[temp.X, temp.Y] != 1 ||
            world.data.main[temp.X + 1, temp.Y] != 1)
            return false;

        byte height = (byte)random.Next(minTreeHeight, maxTreeHeight + 1);
        // Check for space
        for (temp.Y += 1; temp.Y < pos.Y + height; temp.Y++) {
            if (world.data.main[temp.X - 1, temp.Y] != 0 ||
                world.data.main[temp.X, temp.Y] != 0 ||
                world.data.main[temp.X + 1, temp.Y] != 0)
                return false;
        }

        PlantTree(pos, height);

        return true;
    }

    private static void PlantTree(Vector2I pos, byte height) {
        // Place Stump
        world.data.main.SetTileData(pos, new(TileSetId.tree, 9));

        // Place Logs
        bool left, right, lastLeft = false, lastRight = false;
        for (int y = 1; y < height - 3; y++) {
            pos.Y++;
            left = !lastLeft && random.Next(100) < branchChance;
            right = !lastRight && random.Next(100) < branchChance;
            lastLeft = left;
            lastRight = right;
            if (!left && !right) {
                world.data.main.SetTileData(pos, new(TileSetId.tree, 6));
            }
            else if (left && !right) {
                world.data.main.SetTileData(pos, new(TileSetId.tree, 3));
                world.data.main.SetTileData(pos.X - 1, pos.Y, new(TileSetId.tree, 7));
            }
            else if (!left && right) {
                world.data.main.SetTileData(pos, new(TileSetId.tree, 3, 1));
                world.data.main.SetTileData(pos.X + 1, pos.Y, new(TileSetId.tree, 7, 1));
            }
            else if (left && right) {
                world.data.main.SetTileData(pos, new(TileSetId.tree, 8));
                world.data.main.SetTileData(pos.X - 1, pos.Y, new(TileSetId.tree, 7));
                world.data.main.SetTileData(pos.X + 1, pos.Y, new(TileSetId.tree, 7, 1));
            }
        }

        // Place Crown
        pos.Y++;
        world.data.main.SetTileData(pos, new(TileSetId.tree, 5));
        world.data.main.SetTileData(pos.X - 1, pos.Y, new(TileSetId.tree, 4));
        world.data.main.SetTileData(pos.X + 1, pos.Y, new(TileSetId.tree, 4, 1));
        pos.Y++;
        world.data.main.SetTileData(pos, new(TileSetId.tree, 2));
        world.data.main.SetTileData(pos.X - 1, pos.Y, new(TileSetId.tree, 1));
        world.data.main.SetTileData(pos.X + 1, pos.Y, new(TileSetId.tree, 1, 1));
        pos.Y++;
        world.data.main.SetTileData(pos, new(TileSetId.tree, 1, 2));
        world.data.main.SetTileData(pos.X - 1, pos.Y, new(TileSetId.tree, 4, 2));
        world.data.main.SetTileData(pos.X + 1, pos.Y, new(TileSetId.tree, 4, 3));
    }

    public static void LoadWorld() {
        Vector2I pos = new();
        for (pos.Y = 0; pos.Y < world.data.size.Y; pos.Y++) {
            for (pos.X = 0; pos.X < world.data.size.X; pos.X++) {
                if (world.data.back[pos] != 0)
                    UpdateCell(WorldLayer.back, pos);
                if (world.data.main[pos] != 0)
                    UpdateCell(WorldLayer.main, pos);
            }
        }
    }

    public void SpawnPlayer() {
        Node2D player = GetNode<Node2D>("Player");
        Vector2I idPos = new(
            world.data.size.X / 2,
            world.data.size.Y - 1
        );
        while (world.data.main[idPos] == 0)
            idPos.Y--;
        Vector2 pos = idPos * 16;
        pos.Y = -pos.Y;
        player.Position = pos;
    }

    public static void BreakBlock(bool backLayer = false) {
        Vector2I mapPos = world.main.LocalToMap(world.main.GetLocalMousePosition());
        mapPos.Y *= -1;
        if (IsOutOfBounds(mapPos) || IsBedrock(mapPos))
            return;
        if (!backLayer) {
            if (world.data.main[mapPos] == 0)
                return;
            TileData td = world.data.main.GetTileData(mapPos);
            if (td.sourceId == TileSetId.tree && (td.id == 2 || td.id == 3 || td.id == 5 || td.id == 6 || td.id == 8 || td.id == 9)) {
                BreakTree(mapPos);
                return;
            }
            world.data.main[mapPos] = 0;
            UpdateCell(WorldLayer.main, mapPos);
            if (world.data.main.GetTileData(mapPos).sourceId != TileSetId.main)
                return;
            mapPos.Y++;
            if (world.data.main.GetTileData(mapPos).sourceId == TileSetId.tree) {
                BreakTree(mapPos);
            }
        }
        else {
            if (world.data.main[mapPos] != 0)
                return;
            world.data.back[mapPos] = 0;
            UpdateCell(WorldLayer.back, mapPos);
        }
    }

    public static async void BreakTree(Vector2I pos) {
        TileData td = world.data.main.GetTileData(pos);
        while (!IsOutOfBounds(pos) && td.sourceId == TileSetId.tree && td.id != 0) {
            Vector2I temp = pos;
            world.data.main[temp] = 0;
            UpdateCell(WorldLayer.main, temp);
            temp.X--;
            if (world.data.main.GetTileData(temp).sourceId == TileSetId.tree) {
                world.data.main[temp] = 0;
                UpdateCell(WorldLayer.main, temp);
            }
            temp.X += 2;
            if (world.data.main.GetTileData(temp).sourceId == TileSetId.tree) {
                world.data.main[temp] = 0;
                UpdateCell(WorldLayer.main, temp);
            }
            pos.Y++;
            td = world.data.main.GetTileData(pos);
            ushort id = world.data.main[pos];
            if (!(id == 1 || id == 2 || id == 5))
                await Task.Delay(TimeSpan.FromMilliseconds(25));
        }
    }

    public static void PlaceBlock(bool backLayer) {
        Vector2I mapPos = world.main.LocalToMap(world.main.GetLocalMousePosition());
        mapPos.Y *= -1;
        if (IsOutOfBounds(mapPos) || IsBedrock(mapPos))
            return;
        if (!backLayer) {
            if (world.data.main[mapPos] != 0)
                return;
            world.data.main.SetTileData(mapPos, new(TileSetId.main, 3));
            UpdateCell(WorldLayer.main, mapPos);
        }
        else {
            if (world.data.back[mapPos] != 0)
                return;
            world.data.main.SetTileData(mapPos, new(TileSetId.main, 3, 1));
            UpdateCell(WorldLayer.back, mapPos);
        }
    }
}
