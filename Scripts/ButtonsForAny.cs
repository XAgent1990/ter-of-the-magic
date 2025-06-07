using Godot;
using System;
using TeroftheMagic.Scripts;

public partial class ButtonsForAny : Button {

    public override void _Ready() {
        Pressed += ButtonPressed;
    }

    private new void ButtonPressed() {
        Game.Instance.Init();
    }
}
