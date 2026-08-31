using Godot;
using System;
using TeroftheMagic;

public partial class ButtonsForAny : Button {

    private new void ButtonPressed() {
        Game.Instance.Init();
    }
}
