using Godot;
using System;
using TeroftheMagic.Scripts;
using TeroftheMagic.Scripts.Universe;
using TeroftheMagic.Scripts.Utility;
using Logger = TeroftheMagic.Scripts.Utility.Logger;

public partial class TileMapLayerController : TileMapLayer {

	public WorldChunk Chunk { get; set; }
	private Vector2I center = Vector2I.Zero;
	private Vector2I Center {
		get {
			if (center == Vector2.Zero)
				center = Chunk.origin + WorldData.chunkSizeV / 2;
			return center;
		}
	}

	private static readonly byte ttu = 12;
	private byte ppCounter = ttu;

	public override void _Ready() {
		base._Ready();

		center = Chunk.origin + WorldData.chunkSizeV / 2;
	}

	public override void _Input(InputEvent @event) {
		base._Input(@event);
	}


	public override void _PhysicsProcess(double delta) {
		base._Process(delta);

		ppCounter++;
		if (ppCounter >= ttu) {
			Vector2 pos = Game.Player.Position;
			pos.Y *= -1;
			float distance = (pos / TileUtil.TilePixelSizeV - Center).Length();
			// GD.Print($"center: {Center}, distance:{distance}");
			Enabled = distance <= Game.RenderDistance;
			ppCounter -= ttu;
		}
	}

}
