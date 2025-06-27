using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using TeroftheMagic.Scripts.Universe;
using static TeroftheMagic.Scripts.Game;
using static TeroftheMagic.Scripts.Utility.TileUtil;

namespace TeroftheMagic.Scripts.Utility;

public abstract class Functions {

	public static ushort PercentToWorldHeight(byte percent) => (ushort)(percent * WorldData.size.Y / 100);

	public static bool IsOutOfBounds(Vector2I pos) => IsOutOfBounds(pos.X, pos.Y);
	public static bool IsOutOfBounds(int x, int y) => x < 0 || x >= WorldData.size.X || y < 0 || y >= WorldData.size.Y;
	public static bool IsOnEdge(Vector2I pos) => IsOnEdge(pos.X, pos.Y);
	public static bool IsOnEdge(int x, int y) => x == 0 || x == WorldData.size.X - 1 || y == 0 || y == WorldData.size.Y - 1;
	public static bool IsWood(Vector2I pos) => WorldData.main[pos].ID == "totm:log";
	public static bool IsBedrock(WorldLayer layer, Vector2I pos) =>
		WorldData.TargetLayer(layer)[pos].ID == Block.Bedrock;
	public static bool IsAir(WorldLayer layer, Vector2I pos) =>
		WorldData.TargetLayer(layer)[pos].ID == Block.Air;

	public static bool IsUnbreakable(WorldLayer layer, Vector2I pos) =>
		IsBedrock(layer, pos) || IsAir(layer, pos);

	public static float ValueMap(float fromMin, float fromMax, float toMin, float toMax, float value) {
		value -= fromMin;
		value /= fromMax - fromMin;
		value *= toMax - toMin;
		value += toMin;
		return value;
	}

	static readonly JsonSerializerOptions options = new() {
		WriteIndented = true
	};
	public static void WriteJson(string filePath, object obj) =>
		File.WriteAllText(filePath, JsonSerializer.Serialize(obj, options));

	public static T LoadJson<T>(string filePath) =>
		JsonSerializer.Deserialize<T>(File.ReadAllText(filePath));
}
