using Godot;
using System;
using System.Diagnostics;
using TeroftheMagic.Scripts.Utility;

namespace TeroftheMagic.Scripts.Universe;

public partial class ItemDrop : RigidBody2D {
	[Export] public Sprite2D ItemSprite { get; set; }
	[Export] public Area2D FusionArea { get; set; }
	private ItemStack itemStack;
	public ItemStack ItemStack {
		get => itemStack;
		set {
			if (TileUtil.TryTileSetDataToSprite(value.Item.GetTileSetData(), out CompressedTexture2D texture, out Vector2I pos)) {
				ItemSprite.Texture = texture;
				ItemSprite.RegionRect = new(pos, TileUtil.TilePixelSizeV);
			}
			else {
				// Missing Texture?
			}
			itemStack = value;
		}
	}
	public string ID { get => ItemStack.Item.ID; }
	public byte Count {
		get => ItemStack.Count;
		set => itemStack.Count = value;
	}
	public byte StackSize { get => ItemStack.Item.StackSize; }
	private static readonly PackedScene Prefab = GD.Load<PackedScene>("res://Prefabs/ItemDrop.tscn");
	private const float jumpHeight = 10 * TileUtil.TilePixelSize;
	private double bobCounter = 0;
	private const float bobSpeed = 3.5f;
	private const float bobHeight = 2;
	private const double areaCooldown = 1;
	private double areaTimer = areaCooldown;
	private bool IsStationary { get => LinearVelocity.Length() < .1f; }
	// private static byte idCounter = 0;
	// private byte id;

	public ItemDrop() {
		// id = idCounter++;
	}

	public static ItemDrop Spawn(ItemStack itemStack, Vector2I mapPos) {
		ItemDrop itemDrop = Prefab.Instantiate<ItemDrop>();
		itemDrop.Position = World.GetWorldPosition(mapPos);
		// itemDrop.Position = new(mapPos.X + .5f, -mapPos.Y + .5f);
		// itemDrop.Position *= TileUtil.TilePixelSize;
		itemDrop.LinearVelocity = new((float)(Random.Shared.NextDouble() - .5d) * jumpHeight, -jumpHeight);
		itemDrop.ItemStack = itemStack;
		World.Entities.AddChild(itemDrop);
		return itemDrop;
	}

	public override void _Ready() {
		base._Ready();

		FusionArea.BodyEntered += OnEnter;
	}

	public override void _Process(double delta) {
		base._Process(delta);

		bobCounter += delta;
		ItemSprite.Position = new(0, (float)Math.Sin(bobCounter * bobSpeed) * bobHeight - bobHeight);
	}

	public override void _PhysicsProcess(double delta) {
		base._PhysicsProcess(delta);

		ProcessFusionArea(delta);
	}

	private void ProcessFusionArea(double delta) {
		if (!FusionArea.IsActive()) {
			if (ItemStack.IsFull)
				return;
			if (IsStationary) {
				if (areaTimer > 0) {
					areaTimer -= delta;
				}
				else {
					FusionArea.SetActive(true);
					// GD.Print($"{id}: Activating area");
				}
			}
			else areaTimer = areaCooldown;
		}
		else {
			if (!IsStationary || ItemStack.IsFull) {
				FusionArea.SetActive(false);
				areaTimer = areaCooldown;
				// GD.Print($"{id}: Deactivating area");
			}
		}
	}

	private void OnEnter(Node2D body) {
		// GD.Print($"{id}: Body entered");
		if (body == this)
			return;
		// GD.Print($"{id}: Different Body");
		if (body is not ItemDrop)
			return;
		// GD.Print($"{id}: Body is an ItemDrop");
		ItemDrop other = (ItemDrop)body;
		if (other.ID != ID)
			return;
		if (other.Count + Count <= StackSize) {
			Count += other.Count;
			// GD.Print($"{id}: Killing ItemStack {other.id}");
			other.QueueFree();
		}
		else {
			other.Count -= (byte)(StackSize - Count);
			Count = StackSize;
			FusionArea.SetActive(false);
			areaTimer = areaCooldown;
		}
	}
}
