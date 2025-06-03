using Godot;
using System;
using TeroftheMagic.Scripts.Utility;
using static TeroftheMagic.Scripts.Utility.Functions;

namespace TeroftheMagic.Scripts;

public partial class Game : Node2D {
	public static World world = new(100, 40);

	public override void _Ready() {
		// Called every time the node is added to the scene.
		// Initialization here.
		GD.Print("Hello from C# to Godot :)");

		GenerateNewWorld();
		LoadWorld();
	}

	public void GenerateNewWorld() {
		for (ushort y = 0; y < world.data.size.Y; y++) {
			for (ushort x = 0; x < world.data.size.X; x++) {
				switch (y) {
					case < 5:
						world.data.main[x, y] = 4;
						break;
					case < 20:
						world.data.main[x, y] = 3;
						break;
					case < 24:
						world.data.main[x, y] = 2;
						break;
					case < 25:
						world.data.main[x, y] = 1;
						break;
					default:
						world.data.main[x, y] = 0;
						break;
				}
			}
		}
	}

	public void LoadWorld() {
		world.back = GetNode<TileMapLayer>("World/BackLayer");
		world.main = GetNode<TileMapLayer>("World/MainLayer");
		world.front = GetNode<TileMapLayer>("World/FrontLayer");
		Vector2I pos = new();
		for (pos.Y = 0;pos.Y < world.data.size.Y; pos.Y++) {
			for (pos.X = 0; pos.X < world.data.size.X; pos.X++) {
				world.main.SetCell(pos, 1, TileMapCoord(pos));
			}
		}
	}
}
