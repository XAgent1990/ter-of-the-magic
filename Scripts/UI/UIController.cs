using Godot;
using System;
using static TeroftheMagic.Scripts.Game;

namespace TeroftheMagic.Scripts;

public partial class UIController : CanvasLayer {

    private static bool mouseLeftBlocked, mouseRightBlocked;

    public override void _Process(double delta) {
        base._Process(delta);

        if (Input.IsMouseButtonPressed(MouseButton.Left)) {
            if (!mouseLeftBlocked) {
                mouseLeftBlocked = !Input.IsPhysicalKeyPressed(Key.Shift);
                BreakBlock(Input.IsPhysicalKeyPressed(Key.Ctrl));
            }
        }
        else
            mouseLeftBlocked = false;
        if (Input.IsMouseButtonPressed(MouseButton.Right)) {
            if (!mouseRightBlocked) {
                mouseRightBlocked = !Input.IsPhysicalKeyPressed(Key.Shift);
                PlaceBlock(Input.IsPhysicalKeyPressed(Key.Ctrl));
            }
        }
        else
            mouseRightBlocked = false;
    }
}
