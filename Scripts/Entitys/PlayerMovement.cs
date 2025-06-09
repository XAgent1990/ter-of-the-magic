using Godot;
using System;
using static TeroftheMagic.Scripts.Game;

public partial class PlayerMovement : CoreEntityMovement {
    public const float Speed = 300.0f;
    public const float JumpVelocity = -400.0f;

    public static Vector2 PlayerPosition;

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
        PlayerPosition = Position;
    }
}
