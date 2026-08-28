using Godot;
using System;

namespace TeroftheMagic.Scripts.Multiplayer;

public partial class MultiplayerTestButtonCreate : Button
{
	public override void _Ready() {
		base._Ready();

		Pressed += OnPressed;
	}

	public static void OnPressed() {
		MultiplayerTest.Instance.CreateGame();
	}
}
