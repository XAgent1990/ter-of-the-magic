using Godot;
using System;

namespace TeroftheMagic.Scripts.Utility;

public struct WorldData(ushort x, ushort y) {
    public Vector2I size = new(x, y);
    public ushort[] heightMap = new ushort[x];
    public WorldLayerData back = new(x, y);
    public WorldLayerData main = new(x, y);
    public WorldLayerData front = new(x, y);
}

public struct World(ushort x, ushort y) {
    public WorldData data = new(x, y);
    public TileMapLayer back;
    public TileMapLayer main;
    public TileMapLayer front;
}

public class WorldLayerData {

    private readonly ushort[,] arr;

    public WorldLayerData(ushort x, ushort y) => arr = new ushort[x, y];

    private WorldLayerData(ushort[,] arr) => this.arr = arr;

    public ushort this[Vector2I pos] {
        get => arr[pos.X, pos.Y];
        set => arr[pos.X, pos.Y] = value;
    }

    public ushort this[int x, int y] {
        get => arr[x, y];
        set => arr[x, y] = value;
    }

    public WorldLayerData Clone() => new((ushort[,])arr.Clone());
}