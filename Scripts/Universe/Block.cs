using Godot;
using System;
using System.Collections.Generic;
using TeroftheMagic.Scripts.Utility;
using static TeroftheMagic.Scripts.Utility.Functions;
using static TeroftheMagic.Scripts.Utility.Exceptions;
using static TeroftheMagic.Scripts.Utility.TileUtil;

namespace TeroftheMagic.Scripts.Universe;

public class Block : Item {
	public const string Air = "totm:air";
	public const string Bedrock = "totm:bedrock";
	public static TileSetId GetType(string id, string variant = "") => Get(id).GetTileSetData(variant).SourceId;
}

public struct BlockData(string id, string variant = "") {
	public string ID { get; set; } = id;
	public string Variant { get; set; } = variant;
	public override readonly string ToString() => $"{ID}:{Variant}";
	// Maybe add a random variant ID here?
	// So like same variant, just slightly different texture
}