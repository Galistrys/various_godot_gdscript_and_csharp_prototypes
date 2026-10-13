using Godot;
using System;

public partial class Player : CharacterBody3D
{
    public const float SPEED = 3.0f;
    public const float SPRINT_SPEED = 6.0f;
    public const float CROUCH_SPEED = 1.0f;
    public const float JUMP_VELOCITY = 4.5f;

    public const float ACCELERAION = 20.0f;
    public const float AIR_ACCELERATION = 2.0f;

    public const float GRAVITY = 9.81f;

    public const float MOUSE_SENSITIVITY_MODIFIER = 0.005f;
    public const float MAX_ANGLE =  85f;
    public const float MIN_ANGLE = -85f;

    Camera3D _camera;


    public override void _Ready()
    {
        _camera = GetNode<Camera3D>("Camera");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey iek)
        {
            if (iek.Pressed && iek.Keycode == Key.Escape)
            {
                if (Input.MouseMode == Input.MouseModeEnum.Visible)
                    Input.MouseMode = Input.MouseModeEnum.Captured;
                else
                    Input.MouseMode = Input.MouseModeEnum.Visible;
            }
            if (iek.Pressed && iek.Keycode == Key.K)
            {
                var ps = ResourceLoader.Load<PackedScene>("res://tools/fly_camera/fly_camera.scn");
                var fly_camera = ps.Instantiate<FlyCamera>();
                GetParent().AddChild(fly_camera);
                fly_camera.GlobalPosition = GlobalPosition;
                fly_camera.GlobalRotation = GlobalRotation * Vector3.Up;
                QueueFree();
            }
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion iemm)
        {
            RotateY(-iemm.Relative.X * MOUSE_SENSITIVITY_MODIFIER);
            _camera.RotateX(-iemm.Relative.Y * MOUSE_SENSITIVITY_MODIFIER);
            var temp_rot = _camera.RotationDegrees;
            temp_rot.X = Mathf.Clamp(temp_rot.X, MIN_ANGLE, MAX_ANGLE);
            _camera.RotationDegrees = temp_rot;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Walk((float)delta);
    }

    void Walk(float delta)
    {
        float target_y_velocity = 0;

        Vector2 input_dir = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
        Vector3 direction = (Transform.Basis * new Vector3(input_dir.X, 0, input_dir.Y)).Normalized();

        float speed = SPEED;
        float accel = ACCELERAION;

        if (IsOnFloor())
        {
            if (Input.IsActionPressed("move_jump"))
            {
                target_y_velocity = JUMP_VELOCITY;
                accel = AIR_ACCELERATION;
            }
            else if (Input.IsActionPressed("move_sprint") && input_dir.Dot(Vector2.Up) >= 0)
            {
                speed = SPRINT_SPEED;
            }
        }
        else
        {
            target_y_velocity = Velocity.Y - GRAVITY * delta;
            accel = AIR_ACCELERATION;
        }

        Vector3 new_vel = Velocity;

        new_vel.Y = 0;
        new_vel = new_vel.MoveToward(direction * speed, accel * delta);
        new_vel.Y = target_y_velocity;

        Velocity = new_vel;

        MoveAndSlide();
    }
}
