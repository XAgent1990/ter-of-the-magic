using Godot;
using System;
using System.Collections.Generic;
using System.Numerics;
using TeroftheMagic.Scripts;
using TeroftheMagic.Scripts.Universe;

public partial class Player : Entity {

	[Export] int StartInventorySize = 20;
	[Export] public Area2D PickupArea { get; set; }
	// public List<Item> inventoryList = new List<Item>();
	// public ItemSlot[] inventoryArray = new ItemSlot[100];

	public override void _Ready() {
		init();

		PickupArea.BodyEntered += OnEnter;
	}

	public void init() {
		for (int i = 0; i < StartInventorySize; i++) {
			// ItemSlot slot = inventoryArray[i] = new();
			// slot.Visible = true;
			// slot.Locked = false;
		}
	}

	private void OnEnter(Node2D body) {
		if (body is not ItemDrop)
			return;
		ItemDrop itemDrop = (ItemDrop)body;
		if (Game.PlayerInventory.TryAdd(itemDrop.ItemStack, out byte remaining))
			itemDrop.QueueFree();
		else
			itemDrop.Count = remaining;
	}
}

// public class ItemSlot {
// 	public bool Favorited = false;
// 	public bool Locked = true;
// 	public bool Visible = false;
// 	public Vector2I InventoryPos;//do i need this?
// 	public Item item;
// }