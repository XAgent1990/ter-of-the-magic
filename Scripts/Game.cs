using Godot;
using System;
using TeroftheMagic.Scripts.Utility;
using static TeroftheMagic.Scripts.Utility.Functions;

namespace TeroftheMagic.Scripts;

public partial class Game : Node2D {
    public static World world = new(1000, 500);
    [Export] public ushort minHeight = 375;
    [Export] public ushort maxHeight = 425;
    [Export(PropertyHint.Range, "0,20")] public byte smoothIterations = 1;
    private const int seed = 69;
    private static Random random = new(seed);
    private static readonly ushort[] heightMap = new ushort[world.data.size.X];
    private static readonly FastNoiseLite noise = new();
    private static readonly float heightMod = .5f;
    private static readonly float caveMod = 6f;

    public override void _Ready() {
        // Called every time the node is added to the scene.
        // Initialization here.
        GD.Print("Hello from C# to Godot :)");
        // noise.Seed = (int)Time.GetUnixTimeFromSystem();
        noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
        GenerateNewWorld();
        SmoothWorld();
        LoadWorld();
    }

    public void GenerateNewWorld() {
        GenerateHeightMap();
        for (ushort x = 0; x < world.data.size.X; x++) {
            for (ushort y = 0; y < heightMap[x]; y++) {
                ushort material = GetMaterial(x, y);
                if (!IsCave(x, y))
                    world.data.main[x, y] = material;
                world.data.back[x, y] = material;
            }
        }
    }

    public void GenerateHeightMap() {
        for (ushort x = 0; x < world.data.size.X; x++) {
            heightMap[x] = (ushort)ValueMap(0, 1, minHeight, maxHeight, noise.GetNoise1D(heightMod * x) + .5f);
            // GD.Print($"{noise.GetNoise1D(heightMod * x) + .5f} | {heightMap[x]}");
        }
    }

    public static ushort GetMaterial(ushort x, ushort y) {
        if (y < heightMap[x] / 4)
            return 4;
        else if (y < heightMap[x] * 3.5f / 4)
            return 3;
        else if (y < heightMap[x])
            return 2;
        return 0;
    }

    public static bool IsCave(float x, float y) {
        Vector2I max = world.data.size;
        if (x == 0 || x == max.X - 1 || y == 0 || y == max.Y - 1)
            return false;
        // float r = (noise.GetNoise2D(caveMod * x, caveMod * y) + .5f) * 100;
        float r = random.Next(0, 100);
        r /= 1 + y / 1000f;
        return r > 45;
    }

    public void SmoothWorld() {
        ushort[,] temp = (ushort[,])world.data.main.Clone();
        for (int i = 0; i < smoothIterations; i++) {
            for (ushort x = 1; x < world.data.size.X - 1; x++) {
                for (ushort y = 1; y < heightMap[x]; y++) {
                    temp[x, y] = SurroundingGround(x, y) switch {
                        < 4 => 0,
                        > 4 => GetMaterial(x, y),
                        _ => world.data.main[x, y],
                    };
                }
            }
            world.data.main = (ushort[,])temp.Clone();
        }
    }

    public static byte SurroundingGround(ushort x, ushort y) {
        Vector2I max = world.data.size;
        if (x == 0 || x == max.X - 1 || y == 0 || y == max.Y - 1)
            return 4;
        byte count = 0;
        for (int X = x - 1; X <= x + 1; X++) {
            for (int Y = y - 1; Y <= y + 1; Y++) {
                if (X == x && Y == y)
                    continue;
                if (world.data.main[X, Y] != 0)
                    count++;
            }
        }
        return count;
    }

    public void LoadWorld() {
        world.back = GetNode<TileMapLayer>("World/BackLayer");
        world.main = GetNode<TileMapLayer>("World/MainLayer");
        world.front = GetNode<TileMapLayer>("World/FrontLayer");
        Vector2I pos = new();
        for (pos.Y = 0; pos.Y < world.data.size.Y; pos.Y++) {
            for (pos.X = 0; pos.X < world.data.size.X; pos.X++) {
                world.main.SetCell(pos, 1, TileMapCoord(ref world.data.main, pos));
                world.back.SetCell(pos, 1, TileMapCoord(ref world.data.back, pos), 1);
            }
        }
    }
}
