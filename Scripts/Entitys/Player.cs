using Godot;
using System;
using System.Collections.Generic;

public partial class Player : Entity {

    [Export] int StartInventorySize = 20;
    public List<Item> inventoryList = new List<Item>();
    public ItemSlot[] inventoryArray = new ItemSlot[100];

    public override void _Ready() {
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
    public Item item;
}

public struct Item {
    public string name;
    public int ID;
    public int stackSize;
}