using Godot;
using System;
using System.Reflection;

namespace TeroftheMagic.Scripts.Multiplayer;

public partial class MultiplayerTest : Control {

	ENetMultiplayerPeer peer = new();

	public override void _Ready() {
		base._Ready();

		peer.CreateServer(1806, 2);
		Multiplayer.MultiplayerPeer = peer;

		Multiplayer.PeerConnected += Connected;
		Multiplayer.PeerDisconnected += Disconnected;
	}

	public void Connected(long id) {
		if (Multiplayer.IsServer())
			GD.Print($"Player with ID {id} has connected");
		else
			GD.Print($"Connected to Player with ID {id}");
	}

	public void Disconnected(long id) {
		GD.Print($"Player with ID {id} has disconnected");
	}
}
