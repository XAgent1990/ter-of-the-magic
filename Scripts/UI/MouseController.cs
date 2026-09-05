using Godot;
using System;
using TeroftheMagic.Universe;
using static TeroftheMagic.Game;
using static TeroftheMagic.Universe.World;
using static TeroftheMagic.Utility.TileUtil;
using static TeroftheMagic.Utility.Functions;
using TeroftheMagic.Utility;

namespace TeroftheMagic.UI;

public partial class MouseController : ItemContainer {
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
		base._PhysicsProcess(delta);

		if (blocked) return;

		if (held) {
			Vector2 canvPos = Viewport.GetMousePosition();
			Vector2 canvSize = Viewport.GetVisibleRect().Size;
			if(canvPos.X < 0 || canvPos.Y < 0 || canvPos.X > canvSize.X || canvPos.Y > canvSize.Y)
				return;
			Vector2 mousePos = Main.GetLocalMousePosition();
			Vector2I mapPos = new(((int)Math.Floor(mousePos.X / TilePixelSize)).Mod(WorldWidth), (int)Math.Ceiling(-mousePos.Y / TilePixelSize));
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
	public new ItemStack ItemStack {
		get => itemStack;
		set {
			base.ItemStack = value;
			Instance.Visible = value.Count != 0;
		}
	}

	public static Vector2 MPosition {
		get => Viewport.GetMousePosition();
	}

	public override void _Ready() {
		base._Ready();

		Instance = this;
		Viewport = GetViewport();
	}

	public override void _Process(double delta) {
		base._Process(delta);

		if (Visible)
			Position = MPosition;
	}
}
