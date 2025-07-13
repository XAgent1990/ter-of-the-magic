using Godot;
using Godot.Collections;
using System;
using TeroftheMagic.Scripts;
using TeroftheMagic.Scripts.Universe;
using TeroftheMagic.Scripts.Utility;
using Logger = TeroftheMagic.Scripts.Utility.Logger;

namespace TeroftheMagic.Scripts;

public partial class TileMapLayerController : TileMapLayer {

	[Export] public Area2D ScanArea;
	public WorldChunk Chunk { get; set; }
	private Vector2I center = Vector2I.Zero;
	public Vector2I Center {
		get {
			if (center == Vector2.Zero)
				center = Chunk.origin + WorldData.chunkSizeV / 2;
			return center;
		}
	}
	public Node2D LayerNode {
		get => Chunk.layer switch {
			WorldLayer.back => World.Back,
			WorldLayer.main => World.Main,
			_ => null,
		};
	}
	private Array<Node2D> Entities = [];

	public override void _Ready() {
		base._Ready();

		center = Chunk.origin + WorldData.chunkSizeV / 2;
		if (Chunk.layer == WorldLayer.back) {
			ScanArea.QueueFree();
			ScanArea = null;
		}
	}

	public override void _Input(InputEvent @event) {
		base._Input(@event);
	}

	public void Unrender() {
		if (ScanArea is not null) {
			foreach (Node2D node in Entities = ScanArea.GetOverlappingBodies()) {
				if (node is PlayerMovement)
					continue;
				node.GetParent().RemoveChild(node);
				AddChild(node);
			}
		}
		GetParent().RemoveChild(this);
		Enabled = false;
		// GD.Print($"Chunk at {Center} unrendered");
	}

	public void Render() {
		Enabled = true;
		LayerNode.AddChild(this);
		if (ScanArea is not null) {
			foreach (Node2D node in Entities) {
				node.GetParent().RemoveChild(node);
				World.Entities.AddChild(node);
			}
		}
		// GD.Print($"Chunk at {Center} rendered");
	}

}
