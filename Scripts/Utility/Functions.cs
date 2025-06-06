using Godot;
using System;
using static TeroftheMagic.Scripts.Game;

namespace TeroftheMagic.Scripts.Utility;

public abstract class Functions {
    private static byte TileMapWidth = 2;

    private static Vector2I TileMapIdToCoord(ushort id) => new(--id % TileMapWidth, id / TileMapWidth);
    public static Vector2I TileMapCoord(ref ushort[,] arr, Vector2I pos) => TileMapIdToCoord(arr[pos.X, pos.Y]);

    public static ushort PercentToWorldHeight(byte percent) => (ushort)(percent * world.data.size.Y / 100);

    public static float ValueMap(float fromMin, float fromMax, float toMin, float toMax, float value) {
        value -= fromMin;
        value /= fromMax - fromMin;
        value *= toMax - toMin;
        value += toMin;
        return value;
    }
}