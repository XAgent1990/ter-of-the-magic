using Godot;
using System;

public partial class MainButton : Button {
    public PackedScene sceneToLoad = GD.Load<PackedScene>("res://Scenes/Main.tscn");

    public void OnButtonPress() {
        if (sceneToLoad != null) {
            GetTree().Root.AddChild(sceneToLoad.Instantiate<Node2D>());
        }
    }

}
