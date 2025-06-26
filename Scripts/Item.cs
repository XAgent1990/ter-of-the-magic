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
	private byte count;
	public byte Count {
		readonly get => count;
		set {
			if (value > Item.StackSize)
				throw new StackSizeViolation("Tried to set count too large");
			if (value < 0)
				throw new StackSizeViolation("Tried to set negative count");
			count = value;
		}
	}

	public ItemStack(Item item, byte count = 1) {
		if (count == 0) return;
		Item = item;
		Count = count;
	}
	public Item Item { get; set; }
}

public class Drops {
	public float Chance { get; set; }
	protected struct Drop {
		byte Weight { get; set; }
		string ID { get; set; }
	}
}