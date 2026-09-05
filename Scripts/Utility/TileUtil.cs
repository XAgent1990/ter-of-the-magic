using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TeroftheMagic.Utility;

public abstract class TileUtil {
	public const byte TilePixelSize = 16;
	public static Vector2I TilePixelSizeV = new(TilePixelSize,TilePixelSize);

	private static readonly TileSet WorldTileSet = GD.Load<TileSet>("res://GameAssets/WorldTileSet.tres");

	public static Vector2I TileMapIdToCoord(TileSetId sourceId, ushort id) =>
		WorldTileSet.GetSource((int)sourceId).GetTileId(--id);

	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum TileSetId { item, block, tree, plants }
	public struct TileSetData(TileSetId sourceId, ushort id, byte alt = 0) {
		[JsonPropertyName("Type")]
		public TileSetId SourceId { get; set; } = sourceId;
		public ushort ID { get; set; } = id;
		public byte Alt { get; set; } = alt;
		public static bool operator ==(TileSetData left, TileSetData right) =>
			left.SourceId == right.SourceId && left.ID == right.ID && left.Alt == right.Alt;
		public static bool operator !=(TileSetData left, TileSetData right) => !(left == right);
		public override readonly string ToString() =>
			$"{SourceId}:{ID}.{Alt}";

		public override readonly bool Equals(object obj) => throw new NotImplementedException();
		public override readonly int GetHashCode() => throw new NotImplementedException();
	}

	public static bool TryTileSetDataToSprite(TileSetData td, out Texture2D texture, out Vector2I pos) {
		pos = TilePixelSize * TileMapIdToCoord(td.SourceId, td.ID);
		
		texture = ((TileSetAtlasSource)WorldTileSet.GetSource((int)td.SourceId)).Texture;

		return texture!=null;
	}
}
