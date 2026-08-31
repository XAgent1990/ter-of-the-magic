using Godot;
using System;

namespace TeroftheMagic.UI;

public partial class InventoryLayoutController : PanelContainer {
	public override void _Ready() {
		base._Ready();
		Resized += OnResize;
		GetViewport().SizeChanged += OnResize;
		CallDeferred("call_deferred", "OnResize");
	}

	public void OnResize() {
		int x = GetWindow().Size.X;
		if (Size.X > x) {
			float f = x / Size.X;
			Scale = new(f, f);
		}
		else {
			Scale = Vector2.One;
		}
		//GD.Print($"Rescaled to {Scale}");
	}
}
