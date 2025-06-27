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

public enum WorldLayer { back, main, front }

public abstract class World {
	public static Node2D Back;
	public static Node2D Main;
	public static Node2D Front;
	public static Node2D Entities;
	public static Node2D Borders;
	public static void New(Vector2I chunks) {
		WorldData.Clear();
		WorldData.New(chunks);
		WorldData.Generate();
		SetBorders();
	}

	public static void SetBorders() {
		Borders.GetChild<Node2D>(1).Position = new(WorldData.size.X * TilePixelSize, 0);
		Borders.GetChild<Node2D>(2).Position = new(0, -WorldData.size.Y * TilePixelSize);
	}

	public static string GetMaterial(Vector2I pos) {
		if (initGen && pos.Y < 5)
			return pos.GetHashCode().Mod(5) >= pos.Y ? Block.Bedrock : "totm:deepslate";
		else if (pos.Y < WorldData.heightMap[pos.X] / 4)
			return "totm:deepslate";
		else if (pos.Y < WorldData.heightMap[pos.X] * 3.5f / 4)
			return "totm:stone";
		else if (pos.Y < WorldData.heightMap[pos.X])
			return "totm:dirt";
		return Block.Air;
	}
	public static bool IsCave(Vector2I pos) {
		Vector2I max = WorldData.size;
		string id = GetMaterial(pos);
		if (id == Block.Bedrock)
			return false;
		if (IsOnEdge(pos) && id != "totm:dirt")
			return false;
		byte r = (byte)Math.Round((noise.GetNoise2D(pos.X / CaveMod, pos.Y / CaveMod) + .5f) * 100);
		// float r = random.Rand(0, 100);
		// r /= 1 + y / 600f;
		return r > CaveThreshold;
	}

	public static void SmoothWorld() {
		WorldData.SmoothWorld();
	}

	public static byte SurroundingGround(Vector2I refpos) {
		byte count = 0;
		Vector2I pos = new();
		for (pos.X = refpos.X - 1; pos.X <= refpos.X + 1; pos.X++) {
			for (pos.Y = refpos.Y - 1; pos.Y <= refpos.Y + 1; pos.Y++) {
				if (pos.X == refpos.X && pos.Y == refpos.Y)
					continue;
				if (IsOutOfBounds(pos))
					count++;
				else if (!IsAir(WorldLayer.main, pos))
					count++;
			}
		}
		return count;
	}

	public static void BreakBlock(WorldLayer layer, Vector2I pos) {
		Logger.StartTimer("World.BreakBlock");
		WorldData.TargetLayer(layer).BreakBlock(pos);
		Logger.StopTimer("World.BreakBlock");
		if (layer == WorldLayer.main)
			SendBlockUpdates(pos);
	}

	public static void PlaceBlock(WorldLayer layer, Vector2I pos, BlockData bd) {
		Logger.StartTimer("World.PlaceBlock");
		WorldData.TargetLayer(layer).PlaceBlock(pos, bd);
		Logger.StopTimer("World.PlaceBlock");
		if (layer == WorldLayer.main)
			SendBlockUpdates(pos);
	}

	public static async void SendBlockUpdates(Vector2I pos) {
		await Task.Delay(TimeSpan.FromMilliseconds(tickMs));
		WorldData.main.UpdateBlock(new(pos.X - 1, pos.Y));
		WorldData.main.UpdateBlock(new(pos.X + 1, pos.Y));
		WorldData.main.UpdateBlock(new(pos.X, pos.Y - 1));
		WorldData.main.UpdateBlock(new(pos.X, pos.Y + 1));
	}

	public static void Load() {
		WorldData.Load();
	}
}