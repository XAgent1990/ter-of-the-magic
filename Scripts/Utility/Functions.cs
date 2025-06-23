using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using static TeroftheMagic.Scripts.Game;
using static TeroftheMagic.Scripts.Utility.TileUtil;
using TileData = TeroftheMagic.Scripts.Utility.TileUtil.TileData;

namespace TeroftheMagic.Scripts.Utility;

public abstract class Functions {

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
		return td.sourceId == TileSetId.block && td.id == 5;
	}
	public static bool IsAir(Vector2I pos) => WorldData.main[pos].id == 0;

	public static float ValueMap(float fromMin, float fromMax, float toMin, float toMax, float value) {
		value -= fromMin;
		value /= fromMax - fromMin;
		value *= toMax - toMin;
		value += toMin;
		return value;
	}

	public static void WriteJson(string filePath, object obj) =>
		File.WriteAllText(filePath, JsonSerializer.Serialize(obj));

	public static T LoadJson<T>(string filePath) =>
		JsonSerializer.Deserialize<T>(File.ReadAllText(filePath));
}
