using Godot;
using System;
using TeroftheMagic.UI;
using TeroftheMagic.Universe;
using static TeroftheMagic.Utility.TileUtil;

namespace TeroftheMagic.Interactables;

public partial class InventoryCell : ItemContainer {
	
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
			if (mouseButton.ButtonMask == MouseButtonMask.Left) {
				if (MC.ID != ID)
					(MC.ItemStack, ItemStack) = (ItemStack, MC.ItemStack);
				else if (ID != "") {
					if (Count + MC.Count <= StackSize) {
						Count += MC.Count;
						MC.ItemStack = new();
					}
					else {
						MC.Count -= (byte)(StackSize - Count);
						Count = StackSize;
					}
				}
				GetViewport().SetInputAsHandled();
			}
			else if (mouseButton.ButtonMask == MouseButtonMask.Right) {
				if (MC.Count == 0) {
					if (Count > 0) {
						byte half = (byte)Mathf.Ceil(Count / 2.0f);
						MC.ItemStack = new(Item.Get(ID), half);
						Count -= half;
					}
				}
				else if (Count == 0) {
					byte half = (byte)Mathf.Ceil(MC.Count / 2.0f);
					ItemStack = new(Item.Get(MC.ID), half);
					MC.Count -= half;
				}
				else if (MC.ID == ID) {
					byte half = (byte)Mathf.Ceil(MC.Count / 2.0f);
					if (Count + half <= StackSize) {
						Count += half;
						MC.Count -= half;
					}
					else {
						MC.Count -= (byte)(StackSize - Count);
						Count = StackSize;
					}
				}
				GetViewport().SetInputAsHandled();
			}
		}
	}

	private void OnEnter() => Hovered = true;

	private void OnExit() => Hovered = false;
}
