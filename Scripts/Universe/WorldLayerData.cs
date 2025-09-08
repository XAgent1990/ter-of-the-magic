using Godot;
using System;
using System.Threading.Tasks;
using TeroftheMagic.Scripts.Utility;
using static TeroftheMagic.Scripts.Game;
using static TeroftheMagic.Scripts.Utility.Functions;
using static TeroftheMagic.Scripts.Utility.Extensions;
using static TeroftheMagic.Scripts.Utility.TileUtil;
using Logger = TeroftheMagic.Scripts.Utility.Logger;

namespace TeroftheMagic.Scripts.Universe;

public class WorldLayerData {

	public readonly WorldChunk[,] chunks;

	public WorldLayerData(ushort x, ushort y) => chunks = new WorldChunk[x / WorldData.chunkSize, y / WorldData.chunkSize];

	private WorldLayerData(WorldChunk[,] chunks) => this.chunks = chunks;

	public ref BlockData this[Vector2I pos] {
		get {
			Vector2I chunk = pos / WorldData.chunkSize;
			Vector2I chunkPos = pos % WorldData.chunkSize;
			return ref chunks[chunk.X, chunk.Y][chunkPos];
		}
	}

	public WorldLayerData Clone() => new((WorldChunk[,])chunks.Clone());

	public void Clear() {
		ushort cs = WorldData.chunkSize;
		ushort chunkCX = (ushort)(WorldData.size.X / cs);
		ushort chunkCY = (ushort)(WorldData.size.Y / cs);
		for (ushort cx = 0; cx < chunkCX; cx++) {
			for (ushort cy = 0; cy < chunkCY; cy++) {
				chunks[cx, cy].Clear();
			}
		}
	}

	public void Generate(WorldLayer layer) {
		ushort cs = WorldData.chunkSize;
		ushort chunkCX = (ushort)(WorldData.size.X / cs);
		ushort chunkCY = (ushort)(WorldData.size.Y / cs);
		for (ushort cx = 0; cx < chunkCX; cx++) {
			for (ushort cy = 0; cy < chunkCY; cy++) {
				chunks[cx, cy] = new(new(cx * cs, cy * cs), layer);
				chunks[cx, cy].Generate();
			}
		}
	}

	public void SmoothWorld() {
		ushort cs = WorldData.chunkSize;
		ushort chunkCX = (ushort)(WorldData.size.X / cs);
		ushort chunkCY = (ushort)(WorldData.size.Y / cs);
		for (ushort cx = 0; cx < chunkCX; cx++) {
			for (ushort cy = 0; cy < chunkCY; cy++) {
				chunks[cx, cy].SmoothChunk();
			}
		}
	}

	public void BreakBlock(Vector2I pos) {
		Logger.StartTimer("WorldLayerData.BreakBlock");
		ushort cs = WorldData.chunkSize;
		Vector2I cPos = pos / cs;
		Vector2I cOff = pos % cs;
		chunks[cPos.X, cPos.Y].BreakBlock(cOff);
		Logger.StopTimer("WorldLayerData.BreakBlock");
	}

	public void PlaceBlock(Vector2I pos, BlockData bd) {
		Logger.StartTimer("WorldLayerData.PlaceBlock");
		ushort cs = WorldData.chunkSize;
		Vector2I cPos = pos / cs;
		Vector2I cOff = pos % cs;
		chunks[cPos.X, cPos.Y].PlaceBlock(cOff, bd);
		Logger.StopTimer("WorldLayerData.PlaceBlock");
	}

	public void UpdateBlock(Vector2I pos) {
		if (IsOutOfBounds(pos))
			return;
		ushort cs = WorldData.chunkSize;
		Vector2I cPos = pos / cs;
		Vector2I cOff = pos % cs;
		chunks[cPos.X, cPos.Y].UpdateBlock(cOff);
	}

	public void Load(WorldLayer layer) {
		ushort cs = WorldData.chunkSize;
		ushort chunkCX = (ushort)(WorldData.size.X / cs);
		ushort chunkCY = (ushort)(WorldData.size.Y / cs);
		for (ushort cx = 0; cx < chunkCX; cx++) {
			for (ushort cy = 0; cy < chunkCY; cy++) {
				chunks[cx, cy].Load(layer);
			}
		}
	}

	public Vector2 GetWorldPosition(Vector2I mapPos) {
		if (IsOutOfBounds(mapPos))
			return Vector2.Zero;
		ushort cs = WorldData.chunkSize;
		Vector2I cPos = mapPos / cs;
		Vector2I cOff = mapPos % cs;
		return chunks[cPos.X, cPos.Y].GetWorldPosition(cOff);
	}
}