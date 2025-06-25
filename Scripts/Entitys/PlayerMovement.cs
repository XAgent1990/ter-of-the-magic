using Godot;
using System;
using TeroftheMagic.Scripts;
using static TeroftheMagic.Scripts.Game;

public partial class PlayerMovement : CharacterBody2D {
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;
	public const float VerticalOffset = 1f;


	public static Vector2 PlayerPosition;

	[Export]
	public RayCast2D rayRight;
	[Export]
	public RayCast2D rayRightFoot;

	[Export]
	public RayCast2D rayLeft;
	[Export]
	public RayCast2D rayLeftFoot;

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



		if ((!rayRight.IsColliding() && rayRightFoot.IsColliding() && IsOnFloor() && direction.X > 0) || (!rayLeft.IsColliding() && rayLeftFoot.IsColliding() && IsOnFloor() && direction.X < 0)) {
			float dir = 0;
			if (rayRightFoot.IsColliding()) {
				dir = VerticalOffset;
			}
			else {
				dir = -VerticalOffset;
			}

			Position = new(Position.X + dir, Position.Y - 16);
		}


		Velocity = velocity;
		MoveAndSlide();

		if (Position.Y >= 100) {
			Game.Instance.SpawnPlayer();
			Velocity = Vector2.Zero;
		}
		PlayerPosition = Position;
	}



	//if block boarder is near char in tollerance move block up / down

	// new public static void MoveAndSlide() {
	// 	CharacterBody2D.MoveAndSlide();
	// }
}
