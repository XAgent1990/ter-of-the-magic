using Godot;
using System;

namespace TeroftheMagic.Scripts.Utility;

public enum WorldLayer { back, main, front }

public enum TileSetId{ main, tree }

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

public struct TileData(TileSetId sourceId, ushort id, byte alt = 0) {
    public TileSetId sourceId = sourceId;
    public ushort id = id;
    public byte alt = alt;
}

public class WorldLayerData {

    private readonly TileData[,] arr;

    public WorldLayerData(ushort x, ushort y) => arr = new TileData[x, y];

    private WorldLayerData(TileData[,] arr) => this.arr = arr;

    public void SetTileData(Vector2I pos, TileData td) => arr[pos.X, pos.Y] = td;
    public void SetTileData(int x, int y, TileData td) => arr[x, y] = td;
    public TileData GetTileData(Vector2I pos) => arr[pos.X, pos.Y];
    public TileData GetTileData(ushort x, ushort y) => arr[x, y];

    public ushort this[Vector2I pos] {
        get => arr[pos.X, pos.Y].id;
        set => arr[pos.X, pos.Y].id = value;
    }

    public ushort this[int x, int y] {
        get => arr[x, y].id;
        set => arr[x, y].id = value;
    }

    public WorldLayerData Clone() => new((TileData[,])arr.Clone());
}