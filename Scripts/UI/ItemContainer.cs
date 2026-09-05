using Godot;
using System;
using TeroftheMagic.Universe;
using static TeroftheMagic.Game;
using static TeroftheMagic.Universe.World;
using static TeroftheMagic.Utility.TileUtil;
using static TeroftheMagic.Utility.Functions;
using TeroftheMagic.Utility;

namespace TeroftheMagic.UI;
public partial class ItemContainer : Control {
    [Export] protected TextureRect Texture;
	[Export] protected Label CountLabel;
    private AtlasTexture AtlasTexture;
	public override void _Ready() {
		base._Ready();

		Texture.Texture = AtlasTexture = new AtlasTexture();
		// AtlasTexture = (AtlasTexture)Texture.Texture;
	}
    protected void SetTexture(Texture2D texture, Vector2I pos) {
		AtlasTexture.Atlas = texture;
		if (texture is not null)
			AtlasTexture.Region = new(pos, TilePixelSizeV);
	}

	public byte StackSize { get => itemStack.StackSize; }
	protected ItemStack itemStack;
	public ItemStack ItemStack {
		get => itemStack;
		set {
			if (value.Count > 0 && TryTileSetDataToSprite(value.Item.GetTileSetData(), out Texture2D texture, out Vector2I pos)) {
				SetTexture(texture, pos);
				if (value.StackSize > 1)
					CountLabel.Text = value.Count.ToString();
				else
					CountLabel.Text = "";
			}
			else {
				SetTexture(null, Vector2I.Zero);
				CountLabel.Text = "";
			}
			itemStack = value;
		}
	}

	public bool Empty { get => ItemStack.Count == 0; }
	public string ID { get => ItemStack.ID; }
	public byte Count {
		get => itemStack.Count;
		set {
			if (value > 1) {
				CountLabel.Text = value.ToString();
				itemStack.Count = value;
			}
			else {
				CountLabel.Text = "";
				if (value == 0)
					ItemStack = new();
			}
		}
	}
}
