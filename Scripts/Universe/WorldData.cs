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

public class WorldData {
	public static Vector2I size;
	public const ushort chunkSize = 40;
	public static readonly Vector2I chunkSizeV = new(chunkSize, chunkSize);
	public static float BackLayerTint = .67f;
	public static ushort[] heightMap;
	public static WorldLayerData back;
	public static WorldLayerData main;
	public static WorldLayerData front;

	public static WorldLayerData TargetLayer(WorldLayer layer) {
		return layer switch {
			WorldLayer.back => back,
			WorldLayer.front => front,
			_ => main,
		};
	}

	public static void New(Vector2I chunks) {
		ushort x = (ushort)(chunks.X * chunkSize);
		ushort y = (ushort)(chunks.Y * chunkSize);
		size = new(x, y);
		back = new(x, y);
		main = new(x, y);
		front = new(x, y);
		heightMap = new ushort[x];
	}

	public static void Clear() {
		foreach (Node node in World.Entities?.GetChildren()) {
			if (node.Name == "Player")
				continue;
			node.QueueFree();
		}
		back?.Clear();
		main?.Clear();
	}

	public static void Generate() {
		GenerateHeightMap();
		back.Generate(WorldLayer.back);
		main.Generate(WorldLayer.main);
	}

	public static void GenerateHeightMap() {
		for (ushort x = 0; x < size.X; x++) {
			heightMap[x] = (ushort)ValueMap(
				0,
				1,
				PercentToWorldHeight(MinHeight),
				PercentToWorldHeight(MaxHeight),
				noise.GetNoise1D(x * HeightMod) + .5f
			);
		}
	}

	public static void SmoothWorld() {
		main.SmoothWorld();
	}

	public static void Load() {
		back.Load(WorldLayer.back);
		main.Load(WorldLayer.main);
	}
}