using Godot;
using System;

namespace TeroftheMagic.Scripts.Utility;

public class Exceptions {
	public class StackSizeViolation(string message) : Exception(message) {}
	public class NotAnItem(string message) : Exception(message) {}
}
