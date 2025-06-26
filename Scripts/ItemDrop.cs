using Godot;
using System;
using TeroftheMagic.Scripts.Utility;

namespace TeroftheMagic.Scripts;

public partial class ItemDrop : RigidBody2D {
	[Export] public Sprite2D ItemSprite { get; set; }
	private ItemStack itemStack;
	public ItemStack ItemStack {
		get => itemStack;
		set {
			if (TileUtil.TryTileDataToSprite(value.Item.TileData, out CompressedTexture2D texture, out Vector2I pos)) {
				ItemSprite.Texture = texture;
				ItemSprite.RegionRect = new(pos, TileUtil.TilePixelSizeV);
			}
			else {
				// Missing Texture?
			}
			itemStack = value;
		}
	}
	private static PackedScene Prefab = GD.Load<PackedScene>("res://Prefabs/ItemDrop.tscn");
	private double bobCounter = 0;
	private static float bobSpeed = 3.5f;
	private static float bobHeight = 2;

	private ItemDrop() { }

	public static ItemDrop Spawn(ItemStack itemStack, Vector2 pos) {
		ItemDrop itemDrop = Prefab.Instantiate<ItemDrop>();
		itemDrop.Position = pos;
		itemDrop.LinearVelocity = new((float)Random.Shared.NextDouble() * 2 - 1, 1);
		itemDrop.ItemStack = itemStack;
		World.Entities.AddChild(itemDrop);
		return itemDrop;
	}

	public override void _Ready() {
		base._Ready();

		// if (TileUtil.TryTileDataToSprite()) {

		// }
		// else {

		// }

	}

	public override void _Process(double delta) {
		base._Process(delta);

		bobCounter += delta;
		ItemSprite.Position = new(0, (float)Math.Sin(bobCounter * bobSpeed) * bobHeight - bobHeight);
	}
}
