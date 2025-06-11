using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TeroftheMagic.Scripts;
using TeroftheMagic.Scripts.Utility;
using static TeroftheMagic.Scripts.Game;
using static TeroftheMagic.Scripts.Utility.Functions;

namespace TeroftheMagic.Scripts;

public enum WorldLayer { back, main, front }

public enum TileSetId { main, tree }

public class World {
    public static Node2D Back;
    public static Node2D Main;
    public static Node2D Front;
    public static void New(ushort x, ushort y) {
        WorldData.New(x, y);
        WorldData.Generate();
    }
    public static void Clear() {
        WorldData.Clear();
    }

    public static ushort GetMaterial(Vector2I pos) {
        if (initGen && pos.Y < 5)
            return (ushort)(random.Next(0, 5) >= pos.Y ? 5 : 4);
        else if (pos.Y < WorldData.heightMap[pos.X] / 4)
            return 4;
        else if (pos.Y < WorldData.heightMap[pos.X] * 3.5f / 4)
            return 3;
        else if (pos.Y < WorldData.heightMap[pos.X])
            return 2;
        return 0;
    }
    public static bool IsCave(Vector2I pos) {
        Vector2I max = WorldData.size;
        ushort id = GetMaterial(pos);
        if (id == 5) // Bedrock
            return false;
        if (IsOnEdge(pos) && id != 2)
            return false;
        byte r = (byte)Math.Round((noise.GetNoise2D(pos.X / CaveMod, pos.Y / CaveMod) + .5f) * 100);
        // float r = random.Next(0, 100);
        // r /= 1 + y / 600f;
        return r > CaveThreshold;
    }

    public static void SmoothWorld() {
        WorldData.SmoothWorld();
    }

    public static byte SurroundingGround(Vector2I pos) => SurroundingGround((ushort)pos.X, (ushort)pos.Y);
    public static byte SurroundingGround(ushort x, ushort y) {
        byte count = 0;
        Vector2I pos = new();
        for (pos.X = x - 1; pos.X <= x + 1; pos.X++) {
            for (pos.Y = y - 1; pos.Y <= y + 1; pos.Y++) {
                if (pos.X == x && pos.Y == y)
                    continue;
                if (IsOutOfBounds(pos))
                    count++;
                else if (WorldData.main[pos].id != 0)
                    count++;
            }
        }
        return count;
    }

    public static void Load() {
        WorldData.Load();
    }
}

public class WorldData {
    public static Vector2I size;
    public const ushort chunkSize = 100;
    public static ushort[] heightMap;
    public static WorldLayerData back;
    public static WorldLayerData main;
    public static WorldLayerData front;

    public static void New(ushort x, ushort y) {
        size = new(x, y);
        back = new(x, y);
        main = new(x, y);
        front = new(x, y);
        heightMap = new ushort[x];
    }

    public static void Clear() {
        back.Clear();
        main.Clear();
    }

    public static void Generate() {
        GenerateHeightMap();
        back.Generate(WorldLayer.back);
        main.Generate(WorldLayer.main);
    }

    public static void GenerateHeightMap() {
        for (ushort x = 0; x < size.X; x++) {
            heightMap[x] = (ushort)ValueMap(
                0,
                1,
                PercentToWorldHeight(MinHeight),
                PercentToWorldHeight(MaxHeight),
                noise.GetNoise1D(x * HeightMod) + .5f
            );
        }
    }

    public static void SmoothWorld() {
        main.SmoothWorld();
    }

    public static void Load() {
        back.Load(WorldLayer.back);
        main.Load(WorldLayer.main);
    }
}

public class WorldLayerData {

    private readonly WorldChunk[,] chunks;

    public WorldLayerData(ushort x, ushort y) => chunks = new WorldChunk[x / WorldData.chunkSize, y / WorldData.chunkSize];

    private WorldLayerData(WorldChunk[,] chunks) => this.chunks = chunks;

    public ref TileData this[Vector2I pos] {
        get {
            Vector2I chunk = pos / WorldData.chunkSize;
            Vector2I chunkPos = pos % WorldData.chunkSize;
            return ref chunks[chunk.X, chunk.Y][chunkPos];
        }
    }

    public WorldLayerData Clone() => new((WorldChunk[,])chunks.Clone());

    public void Clear() {
        ushort cs = WorldData.chunkSize;
        ushort chunkCX = (ushort)(WorldData.size.X / cs);
        ushort chunkCY = (ushort)(WorldData.size.Y / cs);
        for (ushort cx = 0; cx < chunkCX; cx++) {
            for (ushort cy = 0; cy < chunkCY; cy++) {
                chunks[cx, cy].Clear();
            }
        }
    }

    public void Generate(WorldLayer layer) {
        ushort cs = WorldData.chunkSize;
        ushort chunkCX = (ushort)(WorldData.size.X / cs);
        ushort chunkCY = (ushort)(WorldData.size.Y / cs);
        for (ushort cx = 0; cx < chunkCX; cx++) {
            for (ushort cy = 0; cy < chunkCY; cy++) {
                chunks[cx, cy] = new(new(cx * cs, cy * cs), layer);
                chunks[cx, cy].Generate();
            }
        }
    }

    public void SmoothWorld() {
        ushort cs = WorldData.chunkSize;
        ushort chunkCX = (ushort)(WorldData.size.X / cs);
        ushort chunkCY = (ushort)(WorldData.size.Y / cs);
        for (ushort cx = 0; cx < chunkCX; cx++) {
            for (ushort cy = 0; cy < chunkCY; cy++) {
                chunks[cx, cy].SmoothChunk();
            }
        }
    }

    public void Load(WorldLayer layer) {
        ushort cs = WorldData.chunkSize;
        ushort chunkCX = (ushort)(WorldData.size.X / cs);
        ushort chunkCY = (ushort)(WorldData.size.Y / cs);
        for (ushort cx = 0; cx < chunkCX; cx++) {
            for (ushort cy = 0; cy < chunkCY; cy++) {
                chunks[cx, cy].Load(layer);
            }
        }
    }
}

public class WorldChunk(Vector2I origin, WorldLayer layer) {
    public static PackedScene TMLPrefab = GD.Load<PackedScene>("res://Prefabs/TileMapLayer.tscn");
    private TileData[,] chunk = new TileData[WorldData.chunkSize, WorldData.chunkSize];
    public Vector2I origin = origin;
    public WorldLayer layer = layer;
    public TileMapLayer TML;

    public ref TileData this[Vector2I pos] {
        get => ref chunk[pos.X, pos.Y];
    }

    public void Clear() {
        TML.Clear();
    }

    public void Generate() {
        GenTasks.Add(Task.Run(() => {
            Vector2I cOff = new();
            for (cOff.X = 0; cOff.X < WorldData.chunkSize; cOff.X++) {
                for (cOff.Y = 0; cOff.Y < WorldData.chunkSize; cOff.Y++) {
                    Vector2I pos = origin + cOff;
                    if (pos.Y > WorldData.heightMap[pos.X])
                        continue;
                    ushort id = World.GetMaterial(pos);
                    switch (layer) {
                        case WorldLayer.back:
                            chunk[cOff.X, cOff.Y] = new(TileSetId.main, id, 1);
                            break;
                        case WorldLayer.main:
                            if (!World.IsCave(pos))
                                chunk[cOff.X, cOff.Y] = new(TileSetId.main, id);
                            break;
                        default:
                            break;
                    }
                }
            }
        }));
    }

    public void SmoothChunk() {
        GenTasks.Add(Task.Run(() => {
            TileData[,] temp = new TileData[WorldData.chunkSize, WorldData.chunkSize];
            Vector2I off = new();
            for (off.X = 0; off.X < WorldData.chunkSize; off.X++) {
                for (off.Y = 0; off.Y < WorldData.chunkSize; off.Y++) {
                    if (chunk[off.X, off.Y].id == 5) { // Bedrock
                        temp[off.X, off.Y].id = 5;
                        continue;
                    }
                    Vector2I pos = origin + off;
                    temp[off.X, off.Y].id = World.SurroundingGround(pos) switch {
                        < 4 => 0,
                        > 4 => World.GetMaterial(pos),
                        _ => chunk[off.X, off.Y].id,
                    };
                }
            }
            chunk = temp.Clone() as TileData[,];
        }));
    }

    public void Load(WorldLayer layer) {
        TML = TMLPrefab.Instantiate<TileMapLayer>();
        switch (layer) {
            case WorldLayer.back:
                World.Back.AddChild(TML);
                break;
            case WorldLayer.main:
                World.Main.AddChild(TML);
                break;
        }
        TML.Position = new(origin.X * 16, origin.Y * -16);
        GenTasks.Add(Task.Run(() => {
            Vector2I pos = new();
            for (pos.X = 0; pos.X < WorldData.chunkSize; pos.X++) {
                for (pos.Y = 0; pos.Y < WorldData.chunkSize; pos.Y++) {
                    TML.UpdateCell(pos, chunk[pos.X, pos.Y]);
                }
            }
        }));
    }
}

public struct TileData(TileSetId sourceId, ushort id, byte alt = 0) {
    public TileSetId sourceId = sourceId;
    public ushort id = id;
    public byte alt = alt;
}