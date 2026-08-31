using Godot;
using System;

namespace TeroftheMagic.Multiplayer;

public partial class MultiplayerTestButton : Button {
	
	public override void _Ready() {
		base._Ready();
		Pressed += OnPressed;
	}

	public void OnPressed() {
		Rpc(MethodName.ChangeColor);
		GD.Print($"{Multiplayer.GetUniqueId()}: Button Pressed");
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void ChangeColor() {
		GD.Print($"{Multiplayer.GetUniqueId()}: Color Changed");
		GetTree().Root.GetNode("MultiplayerTest").PrintTree();
		Node current = this;
		GD.Print($"{Multiplayer.GetUniqueId()}: {current.GetPath()}");
		Node parent = current.GetParent();
		GD.Print($"{Multiplayer.GetUniqueId()}: {parent.GetPath()}");
		((ColorRect)parent).Color = Color.Color8((byte)new Random().Next(256),
												 (byte)new Random().Next(256),
												 (byte)new Random().Next(256));
	}
}
