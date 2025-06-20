using Godot;
using System;
using TeroftheMagic.Scripts;
using static TeroftheMagic.Scripts.Game;

public partial class PlayerMovement : CoreEntityMovement {
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;

	public static Vector2 PlayerPosition;

	public void SetPlayerPosition(Vector2 pos) {
		Position = pos;
		Velocity = Vector2.Zero;
	}

	public override void _PhysicsProcess(double delta) {
		Vector2 velocity = Velocity;

		if (!loaded) return;

		// Add the gravity.
		if (!IsOnFloor()) {
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor()) {
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("Left", "Right", "Up", "Down");
		if (direction != Vector2.Zero) {
			velocity.X = direction.X * Speed;
		}
		else {
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
		if (Position.Y >= 100) {
			Position = new Vector2(Position.X, WorldData.size.Y * -16.5f);
			Velocity = Vector2.Zero;
		}
		PlayerPosition = Position;
	}
}
