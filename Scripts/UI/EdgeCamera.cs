using Godot;
using System;
using static TeroftheMagic.Scripts.Utility.Extensions;
using static TeroftheMagic.Scripts.Universe.World;
using static TeroftheMagic.Scripts.Game;

namespace TeroftheMagic.Scripts.UI;

public partial class EdgeCamera : Camera2D {

	public override void _Ready() {
		base._Ready();

		GetParent<SubViewport>().World2D = GetTree().Root.World2D;
	}

	public override void _PhysicsProcess(double delta) {
		base._PhysicsProcess(delta);

		GlobalPosition = Player.GlobalPosition;
		if(GetChunkID(GlobalPosition).X < WorldChunks.X / 2)
			GlobalPosition += Right * (int)WorldWidthPx;
		else
			GlobalPosition += Left * (int)WorldWidthPx;
	}
}
