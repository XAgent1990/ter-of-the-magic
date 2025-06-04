using Godot;
using System;

namespace TeroftheMagic.Scripts.Utility;

public struct WorldData(ushort x, ushort y) {
    public Vector2I size = new(x, y);
    public ushort[,] back = new ushort[x, y];
    public ushort[,] main = new ushort[x, y];
    public ushort[,] front = new ushort[x, y];
}

public struct World(ushort x, ushort y) {
    public WorldData data = new(x, y);
    public TileMapLayer back;
    public TileMapLayer main;
    public TileMapLayer front;
}