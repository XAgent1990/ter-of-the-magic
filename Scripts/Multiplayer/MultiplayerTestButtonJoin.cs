using Godot;
using System;

namespace TeroftheMagic.Scripts.Multiplayer;

public partial class MultiplayerTestButtonJoin : Button {
	public override void _Ready() {
		base._Ready();

		Pressed += OnPressed;
	}

	public static void OnPressed() {
		MultiplayerTest.Instance.JoinGame("127.0.0.1");
	}
}
