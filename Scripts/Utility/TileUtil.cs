using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TeroftheMagic.Scripts.Utility;

public abstract class TileUtil {
	public const byte TilePixelSize = 16;
	public static Vector2I TilePixelSizeV = new(TilePixelSize,TilePixelSize);
	private static readonly Dictionary<TileSetId, CompressedTexture2D> textures = new(){
		{ TileSetId.item, GD.Load<CompressedTexture2D>("res://Assets/ItemTileSet.png") },
		{ TileSetId.block, GD.Load<CompressedTexture2D>("res://Assets/BlockTileSet.png") },
		{ TileSetId.tree, GD.Load<CompressedTexture2D>("res://Assets/TreeTileSet.png") }
	};
	public static Vector2I TileMapIdToCoord(TileSetId sourceId, ushort id) =>
		new(--id % TileMapWidth(sourceId), id / TileMapWidth(sourceId));
	private static byte TileMapWidth(TileSetId sourceId) =>
		(byte)(textures[sourceId].GetWidth() / TilePixelSize);

	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum TileSetId { item, block, tree }
	public struct TileData(TileSetId sourceId, ushort id, byte alt = 0) {
		public string ItemId { get; set; }
		[JsonPropertyName("Type")]
		public TileSetId SourceId { get; set; } = sourceId;
		public ushort ID { get; set; } = id;
		public byte Alt { get; set; } = alt;
		public override readonly string ToString() =>
			$"{SourceId}:{ID}.{Alt}";
	}

	public static bool TryTileDataToSprite(TileData td, out CompressedTexture2D texture, out Vector2I pos) {
		pos = TilePixelSize * TileMapIdToCoord(td.SourceId, td.ID);
		return textures.TryGetValue(td.SourceId, out texture);
	}
}
