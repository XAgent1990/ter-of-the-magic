using Godot;
using System;
using System.Collections.Generic;
using TeroftheMagic.Scripts.Utility;
using static TeroftheMagic.Scripts.Utility.Functions;
using static TeroftheMagic.Scripts.Utility.Exceptions;
using TileData = TeroftheMagic.Scripts.Utility.TileUtil.TileData;

namespace TeroftheMagic.Scripts;

public class Item {
	public readonly string id;
	public readonly string name;
	public readonly string description;
	public readonly byte stackSize;
	public readonly TileData tileData;

	private static readonly List<Item> Registry = LoadJson<List<Item>>("data/ItemRegistry.json");
	public static Item Get(string id) {
		if (Registry.TryFind(item => item.id == id, out Item item))
			return item;
		else
			throw new NotAnItem($"No Item with id '{id}'");
	}
}

public struct ItemStack {
	public ItemStack() {
	}
	private byte count = 0;
	public byte Count {
		readonly get => count;
		set {
			if (value > item.stackSize)
				throw new StackSizeViolation("Tried to set count too large");
			if (value < 0)
				throw new StackSizeViolation("Tried to set negative count");
			count = value;
		}
	}
	public Item item;
}