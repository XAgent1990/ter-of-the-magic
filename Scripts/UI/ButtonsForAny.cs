using Godot;
using System;
using TeroftheMagic;
using TeroftheMagic.Utility;

public partial class ButtonsForAny : Button {

    [Export] public int Selection = 0;
    private new void ButtonPressed() {
        switch (Selection)
        {
            case 0:
                WorldArchive.Save("HalloWorld"); 
                break;
            case 1: 
                WorldArchive.Load("HalloWorld"); 
                break;
            default: {
                Game.Instance.Init();
                    break;
                }
        }
    }
}
