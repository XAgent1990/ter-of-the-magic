namespace TeroftheMagic.Scripts.Universe;

using Godot;
using System;

public class Wand : Item {
	public byte Slots { get; set; }
	public float CastTime{ get; set; }
	public float CastDelay{ get; set; }
	public float Cooldown{ get; set; }
	public int HealthMod { get; set; }
	public int ManaMod { get; set; }
}