using Godot;
using System;
using TeroftheMagic.Spells;
using TeroftheMagic.UI;
using TeroftheMagic.Universe;
using static TeroftheMagic.Utility.TileUtil;

namespace TeroftheMagic.Interactables;

public partial class WandSlot : Control {
	[Export] private TextureRect Texture;
	private AtlasTexture AtlasTexture;
	public bool Empty { get => ItemStack.Count == 0; }
	public string ID { get => ItemStack.ID; }
	private ItemStack itemStack;
	public ItemStack ItemStack {
		get => itemStack;
		set {
			if (value.Item is not SpellComponent)
				return;
			else if (value.Count > 0 && TryTileSetDataToSprite(value.Item.GetTileSetData(), out CompressedTexture2D texture, out Vector2I pos))
				SetTexture(texture, pos);
			else
				SetTexture(null, Vector2I.Zero);
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
			if (mouseButton.ButtonMask == MouseButtonMask.Left ||
				mouseButton.ButtonMask == MouseButtonMask.Right) {
				if (MC.Empty)
					(MC.ItemStack, ItemStack) = (ItemStack, new());
				else if (MC.ItemStack.Item is SpellComponent)
					(MC.ItemStack, ItemStack) = (ItemStack, MC.ItemStack);
				GetViewport().SetInputAsHandled();
			}
		}
	}

	private void OnEnter() => Hovered = true;

	private void OnExit() => Hovered = false;
}
