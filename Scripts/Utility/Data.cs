using Godot;
using System;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using TeroftheMagic.Scripts.Universe;

namespace TeroftheMagic.Scripts.Utility;

[JsonConverter(typeof(RangeIJsonConverter))]
public struct RangeI(int from, int to) {
	public int From { get; set; } = from;
	public int To { get; set; } = to;
}

public class RangeIJsonConverter : JsonConverter<RangeI> {
	public override RangeI Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
		string value = reader.GetString();
		float[] arr = value.SplitFloats("..");
		return new((int)arr[0], (int)arr[1]);
	}

	public override void Write(Utf8JsonWriter writer, RangeI value, JsonSerializerOptions options) =>
		writer.WriteStringValue($"{value.From}..{value.To}");
}

public interface IEntity {
	public Vector2I MapPos { get => World.GetMapPosition(((Node2D)this).GlobalPosition); }
}