using Godot;
using System;
using System.Collections.Generic;
using System.Numerics;

public partial class Player : Entity {

	[Export] int StartInventorySize = 20;
	public List<Item> inventoryList = new List<Item>();
	public ItemSlot[] inventoryArray = new ItemSlot[100];

	public override void _Ready() {
		init();
	}

	public void init() {
		for (int i = 0; i < StartInventorySize; i++) {
			ItemSlot slot = inventoryArray[i] = new();
			slot.Visible = true;
			slot.Locked = false;
		}
	}
}

public class ItemSlot {
	public bool Favorited = false;
	public bool Locked = true;
	public bool Visible = false;
	public Vector2I InventoryPos;//do i need this?
	public Item item;
}

public abstract class Item {
	public string name;
	public string Description;
	public int ID;
	public int stackSize;
	public Texture2D icon;
	public abstract void OnUse(Godot.Vector2 pos, Godot.Vector2 dir);
}