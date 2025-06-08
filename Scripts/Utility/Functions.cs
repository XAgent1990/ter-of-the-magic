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
		switch (layer) {
			case WorldLayer.back:
				world.back.UpdateCell(pos, world.data.back.GetTileData(pos));
				break;
			case WorldLayer.main:
				world.main.UpdateCell(pos, world.data.main.GetTileData(pos));
				break;
			case WorldLayer.front:
				world.front.UpdateCell(pos, world.data.front.GetTileData(pos));
				break;
		}
	}

	public static void UpdateCell(Vector2I pos) {
		world.back.UpdateCell(pos, world.data.back.GetTileData(pos));
		world.main.UpdateCell(pos, world.data.main.GetTileData(pos));
		// world.front.UpdateCell(pos, world.data.front.GetTileData(pos));
	}

	public static ushort PercentToWorldHeight(byte percent) => (ushort)(percent * world.data.size.Y / 100);

	public static bool IsOutOfBounds(Vector2I pos) => pos.X < 0 || pos.X >= world.data.size.X || pos.Y < 0 || pos.Y >= world.data.size.Y;
	public static bool IsOutOfBounds(int x, int y) => x < 0 || x >= world.data.size.X || y < 0 || y >= world.data.size.Y;
	public static bool IsOnEdge(Vector2I pos) => pos.X == 0 || pos.X == world.data.size.X - 1 || pos.Y == 0 || pos.Y == world.data.size.Y - 1;
	public static bool IsOnEdge(int x, int y) => x == 0 || x == world.data.size.X - 1 || y == 0 || y == world.data.size.Y - 1;
	public static bool IsBedrock(Vector2I pos) {
		TileData td = world.data.main.GetTileData(pos);
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
