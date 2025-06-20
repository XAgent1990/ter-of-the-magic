using Godot;
using System;

public partial class SpellBook : Node2D {
	Spell _spell;
	Spell _teleport;
	public override void _Ready() {
		_spell = new("res://Prefabs/enemy.tscn", GetTree().Root);
		// _teleport = new($"..", this.GetParent());
		_teleport = new("res://Prefabs/enemy.tscn", GetTree().Root);

	}
	public override void _Process(double delta) {
		//base._Process(delta);
		if (Input.IsActionJustPressed("MouseClickMiddle")) {
			// _spell.OnUse(GlobalPosition, GlobalPosition - GetGlobalMousePosition());
			// pos + (dir * -1
			_teleport.Teleportation(GetParent<Node2D>(), GlobalPosition + (GlobalPosition - GetGlobalMousePosition()) * -1);
		}
	}
}
