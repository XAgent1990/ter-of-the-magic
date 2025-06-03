using Godot;
using System;
using static TeroftheMagic.Scripts.Game;

namespace TeroftheMagic.Scripts.Utility;

public abstract class Functions {
    private static byte TileMapWidth = 2;

    private static Vector2I TileMapIdToCoord(ushort id) => new(--id % TileMapWidth, id / TileMapWidth);
    public static Vector2I TileMapCoord(Vector2I pos) => TileMapIdToCoord(Game.world.data.main[pos.X, pos.Y]);
}