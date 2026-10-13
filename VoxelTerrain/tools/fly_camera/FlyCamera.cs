using Godot;
using System;

public partial class FlyCamera : CharacterBody3D
{
    const float MIN_ANGLE = -90.0f * (Mathf.Pi / 180.0f);
    const float MAX_ANGLE =  90.0f * (Mathf.Pi / 180.0f);

    Camera3D _camera;
    Vector2 _viewChange = Vector2.Zero;
    Vector2 _cameraSensitivity = new Vector2(0.1f, 0.1f);

    float _speed = 20.0f;
    float _sprint_speed = 40f;

    public override void _Ready()
    {
        _camera = GetNode<Camera3D>("Camera3D");

        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion iemm)
        {
            _viewChange = iemm.Relative;
        }

        if (@event is InputEventMouseButton iemb)
        {
            if (iemb.ButtonIndex == MouseButton.WheelUp && iemb.Pressed)
            {
                _speed += 1;
                _sprint_speed = _speed * 2;
                GD.Print("Camera Speed: ", _speed, " - ", _sprint_speed);
            }
            else if (iemb.ButtonIndex == MouseButton.WheelDown && iemb.Pressed)
            {
                _speed -= 1;
                _sprint_speed = _speed * 2;
                GD.Print("Camera Speed: ", _speed, " - ", _sprint_speed);
            }
        }

        if (@event is InputEventKey iek)
        {
            if (iek.Pressed)
            {
                if (iek.Keycode == Key.Escape)
                {
                    if (Input.MouseMode == Input.MouseModeEnum.Captured)
                        Input.MouseMode = Input.MouseModeEnum.Visible;
                    else
                        Input.MouseMode = Input.MouseModeEnum.Captured;
                }
                if (iek.Keycode == Key.I)
                {
                    GetViewport().DebugDraw = Viewport.DebugDrawEnum.Disabled;
                }
                if (iek.Keycode == Key.O)
                {
                    GetViewport().DebugDraw = Viewport.DebugDrawEnum.Wireframe;
                }
                if (iek.Keycode == Key.P)
                {
                    GetViewport().DebugDraw = Viewport.DebugDrawEnum.Overdraw;
                }

                if (iek.Keycode == Key.K)
                {
                    var ps = ResourceLoader.Load<PackedScene>("res://player/player.scn");
                    var player = ps.Instantiate<Player>();
                    GetParent().AddChild(player);
                    player.GlobalPosition = GlobalPosition;
                    player.GlobalRotation = GlobalRotation * Vector3.Up;
                    QueueFree();
                }
            }
        }
    }

    public override void _Process(double delta)
    {
        if (_viewChange.LengthSquared() > 0)
        {
            float delta_f = (float)delta;
            RotateY(-_viewChange.X * _cameraSensitivity.X * delta_f);
            var temp_rotation = _camera.Rotation;
            temp_rotation.X = Mathf.Clamp(
                _camera.Rotation.X - _viewChange.Y * _cameraSensitivity.Y * delta_f,
                MIN_ANGLE, MAX_ANGLE
            );
            _camera.Rotation = temp_rotation;
        }

        _viewChange = Vector2.Zero;
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 input_dir = Input.GetVector(
            "debug_move_left", "debug_move_right", "debug_move_forward", "debug_move_backward"
        );

        Vector3 direction = (
            _camera.GlobalTransform.Basis * new Vector3(
                input_dir.X, 0, input_dir.Y
            )
        ).Normalized();

        float up_down = Input.GetActionStrength("debug_move_up") - Input.GetActionStrength("debug_move_down");
        if (up_down != 0)
            direction.Y = up_down;
        
        if (direction.LengthSquared() > 0)
            Velocity = direction * (Input.IsActionPressed("debug_move_sprint") ? _sprint_speed : _speed);
        else
            Velocity = Velocity.MoveToward(Vector3.Zero, _speed);

        MoveAndSlide();
    }
}
