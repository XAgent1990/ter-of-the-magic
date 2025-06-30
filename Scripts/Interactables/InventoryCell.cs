using Godot;
using System;
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
			itemStack.Count = value;
			if (itemStack.Count > 0)
				CountLabel.Text = value.ToString();
			else
				CountLabel.Text = "";
		}
	}
	public byte StackSize { get => itemStack.StackSize; }
	private ItemStack itemStack;
	public ItemStack ItemStack {
		get => itemStack;
		set {
			if (TryTileSetDataToSprite(value.Item.GetTileSetData(), out CompressedTexture2D texture, out Vector2I pos))
				SetTexture(texture, pos);
			else
				SetTexture(null, Vector2I.Zero);
			itemStack = value;
			if (itemStack.Count > 0)
				CountLabel.Text = itemStack.Count.ToString();
			else
				CountLabel.Text = "";
		}
	}

	public override void _Ready() {
		base._Ready();

		Texture.Texture = new AtlasTexture();
		AtlasTexture = (AtlasTexture)Texture.Texture;
	}

	private void SetTexture(CompressedTexture2D texture, Vector2I pos) {
		AtlasTexture.Atlas = texture;
		GD.Print($"Texture set to {texture.ResourcePath} at {pos}");
		if (texture is not null)
			AtlasTexture.Region = new(pos, TilePixelSizeV);
	}
}
