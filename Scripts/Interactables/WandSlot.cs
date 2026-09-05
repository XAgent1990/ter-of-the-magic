using Godot;
using System;
using TeroftheMagic.Spells;
using TeroftheMagic.UI;
using TeroftheMagic.Universe;
using static TeroftheMagic.Utility.TileUtil;

namespace TeroftheMagic.Interactables;

public partial class WandSlot : ItemContainer {

	public override void _Ready() {
		base._Ready();

		MouseEntered += OnEnter;
		MouseExited += OnExit;
	}

	private bool Hovered { get; set; }

	public override void _Input(InputEvent @event) {
		base._Input(@event);

		if (!Visible || !Hovered) return;

		if (@event is InputEventMouseButton mouseButton) {
			MouseController MC = MouseController.Instance;
			if (mouseButton.ButtonMask == MouseButtonMask.Left ||
				mouseButton.ButtonMask == MouseButtonMask.Right) {
				if (MC.Empty)
					(MC.ItemStack, ItemStack) = (ItemStack, new());
				else if (MC.ItemStack.Item is SpellComponent)
					(MC.ItemStack, ItemStack) = (ItemStack, MC.ItemStack);
				GetViewport().SetInputAsHandled();
			}
		}
	}

	private void OnEnter() => Hovered = true;

	private void OnExit() => Hovered = false;
}
