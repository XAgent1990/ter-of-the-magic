using Godot;
using System;
using static TeroftheMagic.Scripts.Game;

namespace TeroftheMagic.Scripts.Utility;

public abstract class Functions {
	public static Vector2I TileMapIdToCoord(ushort id, byte TileMapWidth = 2) => new(--id % TileMapWidth, id / TileMapWidth);
	public static byte TileMapWidth(TileSetId sourceId) {
		return sourceId switch {
			TileSetId.main => 2,
			TileSetId.tree => 3,
			_ => 0,
		};
	}

	public static void UpdateCell(WorldLayer layer, Vector2I pos) {
		// switch (layer) {
		//     case WorldLayer.back:
		//         World.Back.UpdateCell(pos, WorldData.back[pos]);
		//         break;
		//     case WorldLayer.main:
		//         World.Main.UpdateCell(pos, WorldData.main[pos]);
		//         break;
		//     case WorldLayer.front:
		//         World.Front.UpdateCell(pos, WorldData.front[pos]);
		//         break;
		// }
	}

	public static void UpdateCell(Vector2I pos) {
		// World.Back.UpdateCell(pos, WorldData.back[pos]);
		// World.Main.UpdateCell(pos, WorldData.main[pos]);
		// World.front.UpdateCell(pos, WorldData.front[pos]);
	}

	public static ushort PercentToWorldHeight(byte percent) => (ushort)(percent * WorldData.size.Y / 100);

	public static bool IsOutOfBounds(Vector2I pos) => IsOutOfBounds(pos.X, pos.Y);
	public static bool IsOutOfBounds(int x, int y) => x < 0 || x >= WorldData.size.X || y < 0 || y >= WorldData.size.Y;
	public static bool IsOnEdge(Vector2I pos) => IsOnEdge(pos.X, pos.Y);
	public static bool IsOnEdge(int x, int y) => x == 0 || x == WorldData.size.X - 1 || y == 0 || y == WorldData.size.Y - 1;
	public static bool IsWood(Vector2I pos) {
		ushort id = WorldData.main[pos].id;
		return id == 2 || id == 3 || id == 5 || id == 6 || id == 8 || id == 9;
	}
	public static bool IsBedrock(Vector2I pos) {
		TileData td = WorldData.main[pos];
		return td.sourceId == TileSetId.main && td.id == 5;
	}

	public static float ValueMap(float fromMin, float fromMax, float toMin, float toMax, float value) {
		value -= fromMin;
		value /= fromMax - fromMin;
		value *= toMax - toMin;
		value += toMin;
		return value;
	}
}
