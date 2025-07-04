using Godot;
using System;
using TeroftheMagic.Scripts;
using TeroftheMagic.Scripts.Universe;
using TeroftheMagic.Scripts.Utility;

public partial class TileMapLayerController : TileMapLayer {

	public WorldChunk Chunk { get; set; }

	private enum ActiveButton { Left, Right }
	private static ActiveButton activeButton;
	private static byte ppCounter = 0;

	private static bool left, right, shift, ctrl, held, blocked;

	public override void _Input(InputEvent @event) {
		base._Input(@event);

		if (@event is InputEventMouseButton mouseButton) {
			left = (mouseButton.ButtonMask & MouseButtonMask.Left) == MouseButtonMask.Left;
			right = (mouseButton.ButtonMask & MouseButtonMask.Right) == MouseButtonMask.Right;
			shift = mouseButton.ShiftPressed;
			ctrl = mouseButton.CtrlPressed;

			if (mouseButton.IsPressed() && !held) {
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
			Vector2 MP = GetLocalMousePosition() / TileUtil.TilePixelSize;
			MP.Y = -MP.Y + 1;
			Vector2I cOff = (Vector2I)MP.Floor();
			Vector2I mapPos = Chunk.origin + cOff;
			if (cOff.X >= 0 && cOff.X < WorldData.chunkSize &&
				cOff.Y >= 0 && cOff.Y < WorldData.chunkSize) {
				// GD.Print($"MouseHeld {cOff} {mapPos} {Chunk.layer} {(ctrl ? WorldLayer.back : WorldLayer.main)}");
				if (Chunk.layer == (ctrl ? WorldLayer.back : WorldLayer.main))
					GD.Print("Mouse Button Event");
				else
					return;
			}
			else return;
			switch (activeButton) {
				case ActiveButton.Left:
					Game.BreakBlock(Chunk.layer);
					break;
				case ActiveButton.Right:
					Game.PlaceBlock(Chunk.layer);
					break;
			}
			if (!shift)
				blocked = true;
		}

		// ppCounter++;
		// if (ppCounter >= ppPerTick)
		// 	ppCounter -= ppPerTick;
		// else
		// 	return;

		// if (Input.IsMouseButtonPressed(MouseButton.Left)) {
		// 	if (!mouseLeftBlocked && !mouseRightBlocked) {
		// 		mouseLeftBlocked = !Input.IsPhysicalKeyPressed(Key.Shift);
		// 		BreakBlock(Input.IsPhysicalKeyPressed(Key.Ctrl) ? WorldLayer.back : WorldLayer.main);
		// 		return;
		// 	}
		// }
		// else
		// 	mouseLeftBlocked = false;
		// if (Input.IsMouseButtonPressed(MouseButton.Right)) {
		// 	if (!mouseLeftBlocked && !mouseRightBlocked) {
		// 		mouseRightBlocked = !Input.IsPhysicalKeyPressed(Key.Shift);
		// 		PlaceBlock(Input.IsPhysicalKeyPressed(Key.Ctrl) ? WorldLayer.back : WorldLayer.main);
		// 	}
		// }
		// else
		// 	mouseRightBlocked = false;
	}

}
