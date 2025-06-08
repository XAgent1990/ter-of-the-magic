using Godot;
using System;
using static TeroftheMagic.Scripts.Game;
using TeroftheMagic.Scripts;
using TeroftheMagic.Scripts.Utility;

public partial class TestUi : Control {

    Label PlayerPosX;
    Label PlayerPosY;
    Label MousePosX;
    Label MousePosY;
    Label CanPosX;
    Label CanPosY;

    public override void _Ready() {
        base._Ready();


        PlayerPosX = GetNode<Label>("BG/VBoxContainer/PPX");
        PlayerPosY = GetNode<Label>("BG/VBoxContainer/PPY");
        MousePosX = GetNode<Label>("BG/VBoxContainer/MPX");
        MousePosY = GetNode<Label>("BG/VBoxContainer/MPY");
        CanPosX = GetNode<Label>("BG/VBoxContainer/CPX");
        CanPosY = GetNode<Label>("BG/VBoxContainer/CPY");

    }

    public override void _Process(double delta) {
        base._Process(delta);

        Vector2 vector2 = GetLocalMousePosition();
        CanPosX.Text = $"Canvas Pos x: [ {vector2.X} ]";
        CanPosY.Text = $"Canvas Pos y: [ {vector2.Y} ]";

        Vector2I mapPos = world.main.LocalToMap(world.main.GetLocalMousePosition());
        MousePosX.Text = $"Mouse Pos x: [ {mapPos.X} ]";
        MousePosY.Text = $"Mouse Pos y: [ {mapPos.Y *= -1} ]";


        Vector2I playerPos = world.main.LocalToMap(PlayerMovement.PlayerPosition);
        PlayerPosX.Text = $"Player Pos x: [ {playerPos.X} ]";
        PlayerPosY.Text = $"Player Pos y: [ {playerPos.Y *= -1} ]";

    }


}
