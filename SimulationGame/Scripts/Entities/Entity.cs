using Godot;
using System;

public abstract class Entity : KinematicBody
{
    /// <summary>
    /// Entity movement speed
    /// </summary>
    /// <value>Speed as Integer</value>
    public abstract int moveSpeed { get; } 

    Spatial _headSpatial;
    Camera _playerCamera;

    Vector3 _lastOnGroundMotion = Vector3.Zero;

    // TEMP
    // TODO: move it to MainManager
    public float mouseSensitivity = 0.2f;
    public float gravityModificator = 0.2f;
    // END of TEMP

    /// <summary>
    /// DO NOT USE. ENGINE STUFF.
    /// </summary>
    public override void _Ready()
    {
        // TODO: if not IsNetworkMaster() then remove camera
        Input.SetMouseMode(Input.MouseMode.Captured);
        

        _headSpatial = GetNode<Spatial>("Head");
        _playerCamera = GetNode<Camera>("Head/Camera");

        SetPhysicsProcess(false);
    }

    /// <summary>
    /// DO NOT USE. ENGINE STUFF.
    /// </summary>
    public override void _PhysicsProcess(float delta)
    {
        Vector3 motion = Vector3.Zero;
        Basis playerBasis = this.Transform.basis;

        if (Input.IsActionPressed("move_forward"))
            motion -= playerBasis[2];
        if (Input.IsActionPressed("move_backward"))
            motion += playerBasis[2];
        if (Input.IsActionPressed("move_left"))
            motion -= playerBasis[0];
        if (Input.IsActionPressed("move_right"))
            motion += playerBasis[0];
        
        motion.y = 0;
        motion = motion.Normalized();
        _lastOnGroundMotion.y -= 10 * delta;

        motion *= Input.IsKeyPressed((int)KeyList.Shift) ? moveSpeed * 2 : moveSpeed;
        motion.y = _lastOnGroundMotion.y;
        
        if (IsOnFloor())
        {
            if (Input.IsActionPressed("jump"))
            {
                motion.y = 5;
            }
        }
        else
        {
            Vector3 inter = _lastOnGroundMotion.LinearInterpolate(motion, 0.02f);
            motion.x = inter.x;
            motion.z = inter.z;
        }

        _lastOnGroundMotion = MoveAndSlide(motion, Vector3.Up);

        // if (Input.IsActionPressed("jump") && IsOnFloor())
        // {
        //     motion.y = 1;
        //     _yVel = -5;
        //     MoveAndSlide(motion.Normalized() * moveSpeed, Vector3.Up);
        // }
        
        // if (IsOnFloor())
        // {
        //     motion.y = 0;
        //     _yVel = 0;
        //     if (motion.Length() != 0)
        //         MoveAndSlide(motion.Normalized() * moveSpeed, Vector3.Up);
        //         //MoveAndSlideWithSnap(motion.Normalized() * moveSpeed, Vector3.Down, Vector3.Up);
        //     //_lastOnGroundMotion = motion;
        // }
        // else
        // {
        //     _yVel = _yVel + 9.81f * delta;
        //     motion.y = -_yVel * gravityModificator;
        //     MoveAndSlide(motion * moveSpeed, Vector3.Up);
        // }
    }

    /// <summary>
    /// DO NOT USE. ENGINE STUFF.
    /// </summary>
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion iemm)
        {
            Vector2 relative = iemm.GetRelative();

            RotateY(-relative.x / 100 * mouseSensitivity);

            float currentRotation = _playerCamera.GetRotationDegrees().x;
            if ((currentRotation > -80 && currentRotation < 80) || (currentRotation <= -80 && -relative.y > 0) || (currentRotation >= -80 && -relative.y < 0))
            {
                _playerCamera.RotateX(-relative.y / 100 * mouseSensitivity);
            }
        }
        else if (@event is InputEventKey iek)
        {
            if (iek.Scancode == (int)KeyList.Escape)
            {
                GetTree().Quit();
            }
            if (iek.Scancode == (int)KeyList.Pause)
            {
                SetPhysicsProcess(true);
            }
        }
    }
}
