using Godot;
using System;
using System.Collections.Generic;
using TeroftheMagic.Scripts.Utility;
using static TeroftheMagic.Scripts.Utility.Functions;
using static TeroftheMagic.Scripts.Utility.Exceptions;
using TileData = TeroftheMagic.Scripts.Utility.TileUtil.TileData;
using System.Text.Json.Serialization;

namespace TeroftheMagic.Scripts;

public class Item {
	public Item() { }
	[JsonIgnore]
	public string ID { get => TileData.ItemId; }
	public string Name { get; set; }
	public string Description { get; set; }
	public byte StackSize { get; set; }
	public TileData TileData { get; set; }
	public static Item singleton = new() {
		TileData = new(TileUtil.TileSetId.block, 1, 1) { ItemId = "testid" },
		Name = "derp",
		Description = "hello!",
		StackSize = 11
	};

	private static readonly List<Item> Registry = LoadJson<List<Item>>("Data/ItemRegistry.json");
	static Item() {
		GD.Print(Registry.AsString());
		WriteJson("Data/TestList.json", Registry);
	}
	public static Item Get(string id) {
		if (Registry.TryFind(item => item.ID == id, out Item item))
			return item;
		else
			throw new NotAnItem($"No Item with id '{id}'");
	}

	public override string ToString() => Name;
}

public struct ItemStack {
	public ItemStack() {
	}
	private byte count = 0;
	public byte Count {
		readonly get => count;
		set {
			if (value > item.StackSize)
				throw new StackSizeViolation("Tried to set count too large");
			if (value < 0)
				throw new StackSizeViolation("Tried to set negative count");
			count = value;
		}
	}
	public Item item;
}

public class Drops {
	public float Chance { get; set; }
	private struct Drop {
		byte Weight { get; set; }
		string ID { get; set; }

	}
}