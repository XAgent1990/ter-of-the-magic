using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using TeroftheMagic.Universe;

namespace TeroftheMagic.Interactables;

public partial class PlayerInventory : Inventory {
	public static PlayerInventory Instance;
	public int SlotCount { get => GetChildCount(); }
	private readonly List<InventoryCell> slots = [];
	public override void _Ready() {
		base._Ready();
		Instance = this;
	}
	public List<InventoryCell> Slots {
		get {
			if (!(slots?.Count > 0)) {
				foreach (Node node in GetChildren()) {
					slots.Add((InventoryCell)node);
				}
			}
			return slots;
		}
	}

	public bool TryAdd(ItemStack itemStack, out byte remaining) {
		remaining = itemStack.Count;
		if (SlotCount <= 0)
			return false;
		foreach (InventoryCell ic in Slots) {
			if (ic.ID == "") {
				ic.ItemStack = itemStack;
				return true;
			}
			else {
				if (ic.ID != itemStack.ID)
					continue;
				if (ic.Count + remaining <= ic.StackSize) {
					ic.Count += remaining;
					return true;
				}
				else {
					remaining -= (byte)(ic.StackSize - ic.Count);
					ic.Count = ic.StackSize;
				}
			}
		}
		return remaining == 0;
	}
}
