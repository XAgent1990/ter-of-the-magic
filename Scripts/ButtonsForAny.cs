using Godot;
using System;
using TeroftheMagic.Scripts;

public partial class ButtonsForAny : Button {

    public override void _Ready() {
        Pressed += ButtonPressed;
    }

    private void ButtonPressed() {
        Game.Instance.GenerateNewWorld();
        Game.Instance.LoadWorld();
    }
}
