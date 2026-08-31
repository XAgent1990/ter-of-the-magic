using Godot;
using System;
using System.Collections.Generic;
using TeroftheMagic.Utility;
using static TeroftheMagic.Utility.Functions;
using static TeroftheMagic.Utility.Exceptions;
using static TeroftheMagic.Utility.TileUtil;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace TeroftheMagic.Universe;

public class Block : Item {
	public const string Air = "totm:air";
	public const string Bedrock = "totm:bedrock";
	public const string Moss = "totm:moss";
	public static TileSetId GetType(string id, string variant = "") => Get(id).GetTileSetData(variant).SourceId;
}

[JsonConverter(typeof(BlockDataJsonConverter))]
public class BlockData(string id, string variant = "") {
	public string ID { get; set; } = id;
	public string Variant { get; set; } = variant;
	public override string ToString() {
		if (Variant == "") return ID;
		else return $"{ID}:{Variant}";
	}
	public static BlockData FromString(string s) {
		string[] split = s.Split(':');
		if (split.Length == 2) return new(s);
		else if (split.Length == 3) return new($"{split[0]}:{split[1]}", split[2]);
		else throw new ArgumentException($"Invalid Block-ID [{s}]");
	}
	// Maybe add a random variant ID here?
	// So like same variant, just slightly different texture
}

public class BlockDataJsonConverter : JsonConverter<BlockData> {
	public override BlockData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
		BlockData.FromString(reader.GetString());

	public override void Write(Utf8JsonWriter writer, BlockData value, JsonSerializerOptions options) =>
		writer.WriteStringValue(value.ToString());
}