using Godot;
using System;
using System.Collections.Generic;
using TeroftheMagic.Scripts.Utility;
using static TeroftheMagic.Scripts.Utility.Functions;
using static TeroftheMagic.Scripts.Utility.Exceptions;
using static TeroftheMagic.Scripts.Utility.TileUtil;

namespace TeroftheMagic.Scripts.Universe;

public class Item {
	public Item() {
		VariantTileSetData = [];
		VariantTileSetData.Add("test", new(TileSetId.block, 1, 0));
	}
	// public static Item singleton = new();
	public string ID { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public byte StackSize { get; set; }
	public TileSetData TileSetData { get; set; }
	public Dictionary<string, TileSetData> VariantTileSetData { get; set; }
	public List<Drops> ItemDrops { get; set; }

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

	public TileSetData GetTileSetData(string variant = "") {
		if (variant != "" && VariantTileSetData.TryGetValue(variant, out TileSetData td))
			return td;
		else
			return TileSetData;
	}

	public List<ItemStack> GetItemDrops() {
		List<ItemStack> IS = [];
		foreach (Drops drops in ItemDrops) {
			if (Random.Shared.NextDouble() >= drops.Chance)
				continue;
			int targetWeight = Random.Shared.Next(drops.TotalWeight);
			foreach (Drops.Drop drop in drops.Items) {
				if (drop.Weight <= targetWeight) {
					targetWeight -= drop.Weight;
					continue;
				}
				else {
					if (drop.Count <= 0) break;
					Item item = Get(drop.ID);
					if (item.StackSize <= 0) break;
					ushort totalCount = drop.Count;
					while (totalCount > item.StackSize) {
						IS.Add(new(item, item.StackSize));
						totalCount -= item.StackSize;
					}
					IS.Add(new(item, (byte)totalCount));
					break;
				}
			}
		}
		return IS;
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

	public bool IsFull { get => Count == Item.StackSize; }

	public ItemStack(Item item, byte count = 1) {
		if (count == 0) return;
		if (item.StackSize <= 0)
			throw new StackSizeViolation("Tried to create ItemStack with zero stack size Item");
		Item = item;
		Count = count;
	}
	public Item Item { get; set; }
}

public struct Drops() {
	public float Chance { get; set; } = 1;
	public List<Drop> Items { get; set; }
	private int totalWeight = -1;

	public int TotalWeight {
		get {
			if (totalWeight == -1) {
				totalWeight = 0;
				foreach (Drop drop in Items)
					totalWeight += drop.Weight;
			}
			return totalWeight;
		}
	}
	public struct Drop {
		public Drop() { }
		public byte Weight { get; set; } = 1;
		public string ID { get; set; }
		public ushort Count { get; set; } = 1;
	}
}