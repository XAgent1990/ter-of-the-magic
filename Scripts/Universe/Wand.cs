namespace TeroftheMagic.Scripts.Universe;

using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using static TeroftheMagic.Scripts.Game;

public class Wand : Item {
	public byte Slots { get; set; }
	public float CastTime { get; set; }
	public float CastDelay { get; set; }
	public float Cooldown { get; set; }
	public int HealthMod { get; set; }
	public int ManaMod { get; set; }
	public float RangeMod { get; set; }
	public Dictionary<DamageType, DamageModifier> DamageMod { get; set; }
	public List<Spell> SpellSlots;

	public struct DamageModifier {
		public int Flat { get; set; }
		public float Percent { get; set; }
	}
}