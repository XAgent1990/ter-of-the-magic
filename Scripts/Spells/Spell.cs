using Godot;
using System;
using System.Collections.Generic;

public class Spell : Item {
	public Spell(string target, Node node) {
		RegisterSpell(target);
		SpawnParent = node;
	}
	PackedScene SpellBody;
	// List<Node2D> SpellBodys = new();

	Node SpawnParent;
	public override void OnUse(Vector2 pos, Vector2 dir) {
		Node2D spell = SpellBody.Instantiate<Node2D>();
		// SpellBodys.Add(spell);
		spell.Position = pos + (dir * -1);

		SpawnParent.AddChild(spell);
	}
	public void RegisterSpell(string target) {
		SpellBody = GD.Load<PackedScene>(target);
	}

	public void Teleportation(Node2D target, Vector2 tpos) {

		target.Position = tpos;
	}

	public void Cast() {

	}

	public void Activate() {

	}

	public void ParticleSpawn() {

	}

}

public enum DamageTypes {
	Magical,
	Envirement,
	Physical
}

public enum ProjectileMovement {
}

public enum ProjectileTypeForm {

}

public enum SpellType {
	Projectile, //pew pew | can miss
	Target, //ziel
	Zone, //Zone
	Rush, //move a -> b
	Wall,
	Teleportation,
	Shout, //selfcast
	Throw
}