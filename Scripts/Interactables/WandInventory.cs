using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using TeroftheMagic.Universe;

namespace TeroftheMagic.Interactables;

public partial class WandInventory : Inventory {
	public int SlotCount { get => GetChildCount(); }
	private readonly List<WandSlot> slots = [];
	public List<WandSlot> Slots {
		get {
			if (!(slots?.Count > 0)) {
				foreach (Node node in GetChildren()) {
					slots.Add((WandSlot)node);
				}
			}
			return slots;
		}
	}
}
