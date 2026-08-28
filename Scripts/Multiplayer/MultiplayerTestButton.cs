using Godot;
using System;

namespace TeroftheMagic.Scripts.Multiplayer;

public partial class MultiplayerTestButton : Button {
	ColorRect cr;
	
	public override void _Ready() {
		base._Ready();

		cr = GetParent<ColorRect>();
		Pressed += OnPressed;
	}

	public void OnPressed() {
		Rpc(MethodName.ChangeColor);
		GD.Print("Button Pressed");
	}

	[Rpc]
	public void ChangeColor() {
		GD.Print("Color Changed");
		cr.Color = Color.Color8((byte)Game.random.Next(256),
								(byte)Game.random.Next(256),
								(byte)Game.random.Next(256));
	}
}
