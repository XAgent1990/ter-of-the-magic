using Godot;
using System;
using static TeroftheMagic.Scripts.Game;

namespace TeroftheMagic.Scripts.Utility;

public abstract class Functions {
    private static byte TileMapWidth = 2;

    public static Vector2I TileMapIdToCoord(ushort id) => new(--id % TileMapWidth, id / TileMapWidth);

    public static ushort PercentToWorldHeight(byte percent) => (ushort)(percent * world.data.size.Y / 100);

    public static bool IsOutOfBounds(Vector2I pos) => pos.X < 0 || pos.X >= world.data.size.X || pos.Y < 0 || pos.Y >= world.data.size.Y;
    public static bool IsOutOfBounds(int x, int y) => x < 0 || x >= world.data.size.X || y < 0 || y >= world.data.size.Y;
    public static bool IsOnEdge(Vector2I pos) => pos.X == 0 || pos.X == world.data.size.X-1 || pos.Y == 0 || pos.Y == world.data.size.Y-1;
    public static bool IsOnEdge(int x, int y) => x == 0 || x == world.data.size.X-1 || y == 0 || y == world.data.size.Y-1;


    public static float ValueMap(float fromMin, float fromMax, float toMin, float toMax, float value) {
        value -= fromMin;
        value /= fromMax - fromMin;
        value *= toMax - toMin;
        value += toMin;
        return value;
    }
}