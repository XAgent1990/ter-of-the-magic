using Godot;
using System;
using static TeroftheMagic.Scripts.Game;
using TeroftheMagic.Scripts;
using TeroftheMagic.Scripts.Utility;
using TeroftheMagic.Scripts.Universe;

public partial class TestUi : Control {

	private float UpdateTimer = 0.0f;
	private float UpdateTime = 0.75f;

	Label PlayerPosX;
	Label PlayerPosY;
	Label MousePosX;
	Label MousePosY;
	Label CanPosX;
	Label CanPosY;
	Label FPSCounter;

	public override void _Ready() {
		base._Ready();


		PlayerPosX = GetNode<Label>("BG/VBoxContainer/PPX");
		PlayerPosY = GetNode<Label>("BG/VBoxContainer/PPY");
		MousePosX = GetNode<Label>("BG/VBoxContainer/MPX");
		MousePosY = GetNode<Label>("BG/VBoxContainer/MPY");
		CanPosX = GetNode<Label>("BG/VBoxContainer/CPX");
		CanPosY = GetNode<Label>("BG/VBoxContainer/CPY");
		FPSCounter = GetNode<Label>("BG/VBoxContainer/FPS");

	}

	public override void _Process(double delta) {
		base._Process(delta);

		if (!loaded) return;

		UpdateTimer += (float)delta;

		if (UpdateTimer >= UpdateTime) {
			Vector2 canvasPos = GetLocalMousePosition();
			CanPosX.Text = $"Canvas Pos x: [ {canvasPos.X} ]";
			CanPosY.Text = $"Canvas Pos y: [ {canvasPos.Y} ]";

			Vector2 mousePos = World.Main.GetLocalMousePosition();
			Vector2I mapPos = new((int)(mousePos.X / 16), (int)(mousePos.Y / 16));
			MousePosX.Text = $"Mouse Pos x: [ {mapPos.X} ]";
			MousePosY.Text = $"Mouse Pos y: [ {mapPos.Y *= -1} ]";

			Vector2 playerPos = PlayerMovement.PlayerPosition / 16;
			PlayerPosX.Text = $"Player Pos x: [ {playerPos.X} ]";
			PlayerPosY.Text = $"Player Pos y: [ {playerPos.Y *= -1} ]";
			UpdateTimer = 0.0f;
		}
		FPSCounter.Text = $"FPS: [ {Engine.GetFramesPerSecond()} ]";

	}


}
