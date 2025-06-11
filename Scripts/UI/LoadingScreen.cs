using Godot;
using System;
using Array = Godot.Collections.Array;

namespace TeroftheMagic.Scripts.UI;

public partial class LoadingScreen : Control {
    [Export] public ProgressBar ProgressBar;
    [Export] public string ScenePath;
    [Export] public Array Progress;

    public override void _Ready() {
        ProgressBar.Value = 0;
        Progress = [];
        LoadScene("res://Scenes/Main.tscn");
    }

    public override void _Process(double delta) {
        base._Process(delta);
        if (ScenePath == "")
            return;
        GD.Print($"Loading");
        ResourceLoader.LoadThreadedGetStatus(ScenePath, Progress);
        ProgressBar.Value = (double)Progress[0] * 100;
        if (ProgressBar.Value >= 100.0) {
            GetTree().ChangeSceneToPacked(ResourceLoader.LoadThreadedGet(ScenePath) as PackedScene);
            GD.Print($"Finished load of {ScenePath}");
            ScenePath = "";
        }
    }

    public void LoadScene(string scenePath = "res://Scenes/StartScreen.tscn") {
        GD.Print($"Started load of {scenePath}");
        ResourceLoader.LoadThreadedRequest(ScenePath = scenePath);
    }
}
