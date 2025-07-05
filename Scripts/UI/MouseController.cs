using Godot;
using System;
using TeroftheMagic.Scripts.Universe;
using static TeroftheMagic.Scripts.Utility.TileUtil;
using static TeroftheMagic.Scripts.Universe.World;
using static TeroftheMagic.Scripts.Utility.Functions;
using TeroftheMagic.Scripts.Utility;

namespace TeroftheMagic.Scripts.UI;

public partial class MouseController : Control {
	private enum ActiveButton { Left, Right }
	private static ActiveButton activeButton;

	private static bool left, right, shift, ctrl, held, blocked;

	private static readonly byte ttu = 12;
	private byte ppCounter = ttu;

	public override void _UnhandledInput(InputEvent @event) {
		base._UnhandledInput(@event);

		if (@event is InputEventMouseButton mouseButton) {
			left = (mouseButton.ButtonMask & MouseButtonMask.Left) == MouseButtonMask.Left;
			right = (mouseButton.ButtonMask & MouseButtonMask.Right) == MouseButtonMask.Right;
			shift = mouseButton.ShiftPressed;
			ctrl = mouseButton.CtrlPressed;

			if (mouseButton.IsPressed() && !held) {
				if (!left && !right) return;
				activeButton = left ? ActiveButton.Left : ActiveButton.Right;
				held = true;
			}
			else if (mouseButton.IsReleased() && !(left || right))
				held = blocked = false;

			GetViewport().SetInputAsHandled();
		}
	}


	public override void _PhysicsProcess(double delta) {
		base._Process(delta);

		if (blocked) return;

		if (held) {
			Vector2 mousePos = Main.GetLocalMousePosition();
			Vector2I mapPos = new((int)mousePos.X / TilePixelSize, (int)Math.Ceiling(-mousePos.Y / TilePixelSize));
			WorldLayer layer = ctrl ? WorldLayer.back : WorldLayer.main;
			if (Count > 0) {
				switch (activeButton) {
					case ActiveButton.Left:
						ItemStack = ItemStack.Use(layer, mapPos);
						blocked = !shift || Count == 0;
						break;
					case ActiveButton.Right:
						Drop();
						break;
				}
			}
			else {
				if (IsOutOfBounds(mapPos))
					return;
				switch (activeButton) {
					case ActiveButton.Left:
						if (IsUnbreakable(layer, mapPos) || layer == WorldLayer.back && !IsAir(WorldLayer.main, mapPos))
							return;
						BreakBlock(layer, mapPos);
						break;
					case ActiveButton.Right:
						if (IsAir(WorldLayer.main, mapPos))
							return;
						Interact(mapPos);
						break;
				}
				if (!shift)
					blocked = true;
			}
		}
	}

	public void Drop() {
		ItemDrop.Spawn(ItemStack, Game.Player.Position).Dropped = true;
		Count = 0;
	}

	public void Update() {
		if (Count == 0)
			ItemStack = new();
		else
			CountLabel.Text = Count.ToString();
	}



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
			if (value > 0) {
				CountLabel.Text = value.ToString();
				itemStack.Count = value;
			}
			else {
				CountLabel.Text = "";
				ItemStack = new();
			}
			Visible = value > 0;
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
