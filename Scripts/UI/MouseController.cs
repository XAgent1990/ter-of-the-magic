using Godot;
using System;
using TeroftheMagic.Scripts.Universe;
using static TeroftheMagic.Scripts.Utility.TileUtil;

namespace TeroftheMagic.Scripts.UI;

public partial class MouseController : Control {
	private MouseController() { }
	public static MouseController Instance { get; private set; }
	private static Viewport Viewport;
	[Export] private TextureRect Texture;
	private AtlasTexture AtlasTexture;
	[Export] private Label CountLabel;
	public bool Empty { get => ItemStack.Count == 0; }
	public string ID { get => ItemStack.ID; }
	public byte Count {
		get => itemStack.Count;
		set {
			itemStack.Count = value;
			if (value > 0)
				CountLabel.Text = value.ToString();
			else
				CountLabel.Text = "";
			Instance.Visible = value > 0;
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
			if (itemStack.Count > 0)
				CountLabel.Text = itemStack.Count.ToString();
			else
				CountLabel.Text = "";
			Instance.Visible = value.Count != 0;
			itemStack = value;
		}
	}

	public static Vector2 MPosition {
		get => Viewport.GetMousePosition();
	}

	public override void _Ready() {
		base._Ready();

		Instance = this;
		Viewport = GetViewport();
		Texture.Texture = new AtlasTexture();
		AtlasTexture = (AtlasTexture)Texture.Texture;
	}

	public override void _Process(double delta) {
		base._Process(delta);

		if (Visible)
			Position = MPosition;
	}

	private void SetTexture(CompressedTexture2D texture, Vector2I pos) {
		AtlasTexture.Atlas = texture;
		if (texture is not null)
			AtlasTexture.Region = new(pos, TilePixelSizeV);
	}
}
