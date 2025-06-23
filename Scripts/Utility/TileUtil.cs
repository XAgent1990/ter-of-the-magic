using Godot;
using System;
using System.Collections.Generic;

namespace TeroftheMagic.Scripts.Utility;

public abstract class TileUtil {
	private const byte tilePixelSize = 16;
	private static readonly Dictionary<TileSetId, CompressedTexture2D> textures = new(){
		{ TileSetId.item, GD.Load<CompressedTexture2D>("res://Assets/ItemTileSet.png") },
		{ TileSetId.block, GD.Load<CompressedTexture2D>("res://Assets/BlockTileSet.png") },
		{ TileSetId.tree, GD.Load<CompressedTexture2D>("res://Assets/TreeTileSet.png") }
	};
	public static Vector2I TileMapIdToCoord(ushort id, byte TileMapWidth) => new(--id % TileMapWidth, id / TileMapWidth);
	public static byte TileMapWidth(TileSetId sourceId) =>
		(byte)(textures[sourceId].GetWidth() / tilePixelSize);

	public enum TileSetId { item, block, tree }
	public struct TileData(TileSetId sourceId, ushort id, byte alt = 0) {
		public TileSetId sourceId = sourceId;
		public ushort id = id;
		public byte alt = alt;
		public override readonly string ToString() =>
			$"{sourceId}:{id}.{alt}";
	}
}
