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

public class WorldChunk(Vector2I origin, WorldLayer layer) {
	public static PackedScene TMLPrefab = GD.Load<PackedScene>("res://Prefabs/TileMapLayer.tscn");
	private BlockData[,] chunk = new BlockData[WorldData.chunkSize, WorldData.chunkSize];
	public Vector2I origin = origin;
	public WorldLayer layer = layer;
	public TileMapLayer TML;

	public ref BlockData this[Vector2I pos] {
		get => ref chunk[pos.X, pos.Y];
	}

	public void Clear() {
		TML.QueueFree();
	}

	public void Generate() {
		GenTasks.Add(Task.Run(() => {
			Vector2I cOff = new();
			for (cOff.X = 0; cOff.X < WorldData.chunkSize; cOff.X++) {
				for (cOff.Y = 0; cOff.Y < WorldData.chunkSize; cOff.Y++) {
					Vector2I pos = origin + cOff;
					if (pos.Y > WorldData.heightMap[pos.X]) {
						chunk[cOff.X, cOff.Y] = new(Block.Air);
						continue;
					}
					if (layer == WorldLayer.back || !World.IsCave(pos))
						chunk[cOff.X, cOff.Y] = new(World.GetMaterial(pos));
					else
						chunk[cOff.X, cOff.Y] = new(Block.Air);
				}
			}
		}));
	}

	public void SmoothChunk() {
		GenTasks.Add(Task.Run(() => {
			BlockData[,] temp = new BlockData[WorldData.chunkSize, WorldData.chunkSize];
			Vector2I off = new();
			for (off.X = 0; off.X < WorldData.chunkSize; off.X++) {
				for (off.Y = 0; off.Y < WorldData.chunkSize; off.Y++) {
					BlockData bd = chunk[off.X, off.Y];
					if (bd.ID == Block.Bedrock) { // Bedrock
						temp[off.X, off.Y] = bd;
						continue;
					}
					Vector2I pos = origin + off;
					temp[off.X, off.Y] = World.SurroundingGround(pos) switch {
						< 4 => new(Block.Air),
						> 4 => new(World.GetMaterial(pos)),
						_ => new(bd.ID),
					};
				}
			}
			// GD.Print(chunk.AsString());
			// GD.Print(temp.AsString());
			chunk = temp;
			// GD.Print(chunk.AsString());
		}));
	}

	public void BreakBlock(Vector2I cOff) {
		Logger.StartTimer("WorldChunk.BreakBlock");
		chunk[cOff.X, cOff.Y].ID = Block.Air;
		TML.UpdateCell(cOff);
		Logger.StopTimer("WorldChunk.BreakBlock");
	}

	public void PlaceBlock(Vector2I cOff, BlockData bd) {
		Logger.StartTimer("WorldChunk.PlaceBlock");
		chunk[cOff.X, cOff.Y] = bd;
		TML.UpdateCell(cOff, Item.Get(bd.ID).GetTileSetData(bd.Variant));
		Logger.StopTimer("WorldChunk.PlaceBlock");
	}

	public void UpdateBlock(Vector2I cOff) {
		Vector2I mapPos = origin + cOff;
		BlockData bd = chunk[cOff.X, cOff.Y];
		if (bd.ID == Block.Air)
			return;
		switch (Block.GetType(bd.ID, bd.Variant)) {
			case TileSetId.tree:
				UpdateTree(bd, mapPos);
				break;
			default:
				return;
		}
	}

	private static void UpdateTree(BlockData bd, Vector2I mapPos) {
		WorldLayerData wld = WorldData.TargetLayer(WorldLayer.main);
		// GD.Print("Update " + bd.ID);
		switch (bd.ID) {
			case "totm:log":
				if (wld[mapPos + Down].ID == Block.Air)
					World.BreakBlock(WorldLayer.main, mapPos);
				break;
			case "totm:branch":
				switch (bd.Variant) {
					case "left":
						if (wld[mapPos + Right].ID == Block.Air)
							World.BreakBlock(WorldLayer.main, mapPos);
						break;
					case "right":
						if (wld[mapPos + Left].ID == Block.Air)
							World.BreakBlock(WorldLayer.main, mapPos);
						break;
				}
				break;
			case "totm:leaves":
				switch (bd.Variant) {
					case "" or "top" or "bottom":
						if (wld[mapPos + Down].ID == Block.Air)
							World.BreakBlock(WorldLayer.main, mapPos);
						break;
					case "left" or "topleft" or "bottomleft":
						if (wld[mapPos + Right].ID == Block.Air)
							World.BreakBlock(WorldLayer.main, mapPos);
						break;
					case "right" or "topright" or "bottomright":
						if (wld[mapPos + Left].ID == Block.Air)
							World.BreakBlock(WorldLayer.main, mapPos);
						break;
				}
				break;
		}
	}

	public void Load(WorldLayer layer) {
		// if (layer == WorldLayer.main)
		// 	GD.Print(chunk.AsString());
		TML = TMLPrefab.Instantiate<TileMapLayer>();
		switch (layer) {
			case WorldLayer.back:
				TML.CollisionEnabled = false;
				float blt = WorldData.BackLayerTint;
				TML.Modulate = new(blt, blt, blt);
				World.Back.AddChild(TML);
				break;
			case WorldLayer.main:
				World.Main.AddChild(TML);
				break;
		}
		TML.Position = new(origin.X * 16, origin.Y * -16);
		GenTasks.Add(Task.Run(() => {
			Vector2I pos = new();
			for (pos.X = 0; pos.X < WorldData.chunkSize; pos.X++) {
				for (pos.Y = 0; pos.Y < WorldData.chunkSize; pos.Y++) {
					// GD.Print($"{pos.X},{pos.Y}: {chunk[pos.X, pos.Y].ID}");
					BlockData bd = chunk[pos.X, pos.Y];
					TML.UpdateCell(pos, Item.Get(bd.ID).GetTileSetData(bd.Variant));
				}
			}
		}));
	}
}