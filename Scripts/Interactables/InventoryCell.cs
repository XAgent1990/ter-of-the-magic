using Godot;
using System;
using TeroftheMagic.Scripts.UI;
using TeroftheMagic.Scripts.Universe;
using static TeroftheMagic.Scripts.Utility.TileUtil;

namespace TeroftheMagic.Scripts.Interactables;

public partial class InventoryCell : Control {
	[Export] private TextureRect Texture;
	private AtlasTexture AtlasTexture;
	[Export] private Label CountLabel;
	public bool Empty { get => ItemStack.Count == 0; }
	public string ID { get => ItemStack.ID; }
	public byte Count {
		get => itemStack.Count;
		set {
			if (value > 0) {
				CountLabel.Text = value.ToString();
				itemStack.Count = value;
			}
			else {
				CountLabel.Text = "";
				ItemStack = new();
			}
		}
	}
	public byte StackSize { get => itemStack.StackSize; }
	private ItemStack itemStack;
	public ItemStack ItemStack {
		get => itemStack;
		set {
			if (value.Count > 0 && TryTileSetDataToSprite(value.Item.GetTileSetData(), out CompressedTexture2D texture, out Vector2I pos)) {
				SetTexture(texture, pos);
				CountLabel.Text = value.Count.ToString();
			}
			else {
				SetTexture(null, Vector2I.Zero);
				CountLabel.Text = "";
			}
			itemStack = value;
		}
	}

	public override void _Ready() {
		base._Ready();

		Texture.Texture = new AtlasTexture();
		AtlasTexture = (AtlasTexture)Texture.Texture;
		MouseEntered += OnEnter;
		MouseExited += OnExit;
	}

	private void SetTexture(CompressedTexture2D texture, Vector2I pos) {
		AtlasTexture.Atlas = texture;
		// GD.Print($"Texture set to {texture.ResourcePath} at {pos}");
		if (texture is not null)
			AtlasTexture.Region = new(pos, TilePixelSizeV);
	}

	private bool Hovered { get; set; }

	public override void _Input(InputEvent @event) {
		base._Input(@event);

		if (!Visible || !Hovered) return;

		if (@event is InputEventMouseButton mouseButton) {
			MouseController MC = MouseController.Instance;
			if (mouseButton.ButtonMask == MouseButtonMask.Left) {
				if (MC.ID != ID)
					(MC.ItemStack, ItemStack) = (ItemStack, MC.ItemStack);
				else if (ID != ""){
					if (Count + MC.Count <= StackSize) {
						Count += MC.Count;
						MC.ItemStack = new();
					}
					else {
						MC.Count -= (byte)(StackSize - Count);
						Count = StackSize;
					}
				}
				GetViewport().SetInputAsHandled();
			}
			else if (mouseButton.ButtonMask == MouseButtonMask.Right) {
				if (MC.Count == 0) {
					if (Count > 0) {
						byte half = (byte)Mathf.Ceil(Count / 2.0f);
						MC.ItemStack = new(Item.Get(ID), half);
						Count -= half;
					}
				}
				else if (Count == 0) {
					byte half = (byte)Mathf.Ceil(MC.Count / 2.0f);
					ItemStack = new(Item.Get(MC.ID), half);
					MC.Count -= half;
				}
				else if (MC.ID == ID) {
					byte half = (byte)Mathf.Ceil(MC.Count / 2.0f);
					if (Count + half <= StackSize) {
						Count += half;
						MC.Count -= half;
					}
					else {
						MC.Count -= (byte)(StackSize - Count);
						Count = StackSize;
					}
				}
				GetViewport().SetInputAsHandled();
			}
		}
	}

	private void OnEnter() => Hovered = true;

	private void OnExit() => Hovered = false;
}
