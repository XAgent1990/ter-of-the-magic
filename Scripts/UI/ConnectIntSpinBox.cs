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
			case GameActions.mapx:
				Value = Game.WorldWidth;
				break;
			case GameActions.mapy:
				Value = Game.WorldHeight;
				break;
		}

		ValueChanged += OnValueChanged;
	}

	public void OnValueChanged(double newValue) {
		switch (action) {
			case GameActions.seed:
				Print($"Seed = {newValue}");
				Game.Seed = (int)newValue;
				break;
			case GameActions.hMod:
				Print($"HeightMod = {newValue}");
				Game.HeightMod = (float)newValue;
				break;
			case GameActions.cMod:
				Print($"CaveMod = {newValue}");
				Game.CaveMod = (float)newValue;
				break;
			case GameActions.cTh:
				Print($"CaveThreshold = {newValue}");
				Game.CaveThreshold = (byte)newValue;
				break;
			case GameActions.smoothIt:
				Print($"SmoothIterations = {newValue}");
				Game.SmoothIterations = (byte)newValue;
				break;
			case GameActions.minH:
				Print($"MinHeight = {newValue}");
				Game.MinHeight = (byte)newValue;
				break;
			case GameActions.maxH:
				Print($"MaxHeight = {newValue}");
				Game.MaxHeight = (byte)newValue;
				break;
			case GameActions.mapx:
				Print($"Map X = {newValue}");
				Game.WorldWidth = (ushort)(newValue);
				break;
			case GameActions.mapy:
				Print($"Map Y = {newValue}");
				Game.WorldHeight = (ushort)(newValue);
				break;
		}
	}

	private void Print(string output) {
		//		if (false)
		//            GD.Print(output);
	}
}

public enum GameActions {
	seed,
	hMod,
	cMod,
	cTh,
	smoothIt,
	minH,
	maxH,
	mapx,
	mapy
}
