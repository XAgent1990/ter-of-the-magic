using Godot;
using System;
using static TeroftheMagic.Scripts.Game;

namespace TeroftheMagic.Scripts;

public partial class UIController : CanvasLayer {

	private byte ppCounter = 0;

	private static bool mouseLeftBlocked, mouseRightBlocked;

	public override void _PhysicsProcess(double delta) {
		base._Process(delta);
		ppCounter++;
		if (ppCounter >= ppPerTick)
			ppCounter -= ppPerTick;
		else
			return;

		if (Input.IsMouseButtonPressed(MouseButton.Left)) {
			if (!mouseLeftBlocked && !mouseRightBlocked) {
				mouseLeftBlocked = !Input.IsPhysicalKeyPressed(Key.Shift);
				BreakBlock(Input.IsPhysicalKeyPressed(Key.Ctrl) ? WorldLayer.back : WorldLayer.main);
				return;
			}
		}
		else
			mouseLeftBlocked = false;
		if (Input.IsMouseButtonPressed(MouseButton.Right)) {
			if (!mouseLeftBlocked && !mouseRightBlocked) {
				mouseRightBlocked = !Input.IsPhysicalKeyPressed(Key.Shift);
				PlaceBlock(Input.IsPhysicalKeyPressed(Key.Ctrl) ? WorldLayer.back : WorldLayer.main);
			}
		}
		else
			mouseRightBlocked = false;
	}
}
