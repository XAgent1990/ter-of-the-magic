using Godot;
using System;
using TeroftheMagic.Scripts;

public partial class ConnectIntSpinBox : SpinBox {

    [Export] public GameActions action;

    public override void _Ready() {
        switch (action) {
            case GameActions.seed:
                Value = Game.Seed;
                break;
            case GameActions.hMod:
                Value = Game.HeightMod;
                break;
            case GameActions.cMod:
                Value = Game.CaveMod;
                break;
            case GameActions.cTh:
                Value = Game.CaveThreshold;
                break;
            case GameActions.smoothIt:
                Value = Game.SmoothIterations;
                break;
            case GameActions.minH:
                Value = Game.MinHeight;
                break;
            case GameActions.maxH:
                Value = Game.MaxHeight;
                break;
        }
        ValueChanged += _ValueChanged;
    }

    public override void _ValueChanged(double newValue) {
        switch (action) {
            case GameActions.seed:
                GD.Print($"Seed = {newValue}");
                Game.Seed = (int)newValue;
                break;
            case GameActions.hMod:
                GD.Print($"HeightMod = {newValue}");
                Game.HeightMod = (float)newValue;
                break;
            case GameActions.cMod:
                GD.Print($"CaveMod = {newValue}");
                Game.CaveMod = (float)newValue;
                break;
            case GameActions.cTh:
                GD.Print($"CaveThreshold = {newValue}");
                Game.CaveThreshold = (byte)newValue;
                break;
            case GameActions.smoothIt:
                GD.Print($"SmoothIterations = {newValue}");
                Game.SmoothIterations = (byte)newValue;
                break;
            case GameActions.minH:
                GD.Print($"MinHeight = {newValue}");
                Game.MinHeight = (byte)newValue;
                break;
            case GameActions.maxH:
                GD.Print($"MaxHeight = {newValue}");
                Game.MaxHeight = (byte)newValue;
                break;
        }
    }

}

public enum GameActions {
    seed,
    hMod,
    cMod,
    cTh,
    smoothIt,
    minH,
    maxH
}
