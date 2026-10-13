using Godot;
using System;

public class TestEntity : Entity
{
    const int MOVE_SPEED = 3;
    public override int moveSpeed { get { return MOVE_SPEED; } }

    public override void _Ready()
    {
        base._Ready();

        int sdas = this.moveSpeed;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event is InputEventKey iek)
        {
            if (iek.Pressed && iek.Scancode == (int)KeyList.Backspace)
            {
                this.CollisionLayer = 0;
                this.CollisionMask = 0;
            }
        }
    }
}
