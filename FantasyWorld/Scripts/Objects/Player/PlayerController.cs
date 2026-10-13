using Godot;
using System;

public class PlayerController : KinematicBody
{
    // Camera
    Camera _playerCamera;
    [Export]
    float _mouseSensitivity = 10;
    Vector2 _mouseRelative = Vector2.Zero;
    [Export]
    float _cameraFov = 70f;
    [Export]
    float _aimCameraFov = 25f;
    [Export]
    float _sprintCameraFovMod = 1.05f;
    [Export]
    float _cameraFovInterpolationSpeed = 8;
    [Export]
    float _aimCameraFovInterpolationSpeed = 8;
    const float MAX_ANGLE = 60f;
    const float MIN_ANGLE = -80f;
    Spatial _cameraBase;
    Spatial _cameraPivot;
    Spatial _cameraOffset;
    RayCast _cameraRay;

    // Movement
    Vector3 _velocity = Vector3.Zero;
    Vector3 _direction = Vector3.Zero;
    bool _canSprint = true;
    bool _sprinting = false;
    float _speed = 0;
    float _midAirAimTimeScale = 0.5f;
    float _midAirAimTimeScaleInterpolationSpeed = 16f;
    bool _secondJump = false;

    // Walking
    [Export]
    float _gravity = 9.81f;
    [Export]
    float _gravityMultiplyer = 2f;
    [Export]
    float _walkSpeed = 6;
    [Export]
    float _sprintSpeed = 9;
    [Export]
    float _aimSpeed = 3;
    [Export]
    float _acceleration = 8;
    [Export]
    float _deacceleration = 10;
    [Export]
    float _jumpHeight = 10;
    [Export]
    float _bodyRotationSpeed = 10;
    [Export]
    float _aimBodyRotationSpeed = 20;
    bool _grounded = true;
    readonly Vector3 FLOOR_NORMAL = Vector3.Up;

    // Slopes
    [Export]
    float _floorMaxAngle = 45;

    // Body
    Spatial _body;

    const float VELOCITY_CLAMP = 0.25f;

    public override void _Ready()
    {
        _cameraBase = GetNode<Spatial>("CameraBase");
        _cameraPivot = _cameraBase.GetNode<Spatial>("CameraPivot");
        _cameraOffset = _cameraPivot.GetNode<Spatial>("CameraOffset");
        _cameraRay = _cameraPivot.GetNode<RayCast>("RayCamera");
        _playerCamera = _cameraOffset.GetNode<Camera>("Camera");

        _cameraRay.AddException(this);

        _playerCamera.Fov = _cameraFov;
        _body = GetNode<Spatial>("Body");

        Input.SetMouseMode(Input.MouseMode.Captured);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion iemm)
            _mouseRelative = iemm.Relative;
    }

    public override void _PhysicsProcess(float delta)
    {
        CameraRotation(delta);
        CameraMove();
        Walk(delta);
    }

    void CameraRotation(float delta)
    {
        if (Input.GetMouseMode() != Input.MouseMode.Captured)
            return;
        
        if (_mouseRelative.LengthSquared() > 0)
        {
            float mouseX = -_mouseRelative.x * _mouseSensitivity * delta;
            float mouseY = -_mouseRelative.y * _mouseSensitivity * delta;

            _mouseRelative = Vector2.Zero;

            _cameraBase.RotateY(Mathf.Deg2Rad(mouseX));

            _cameraPivot.RotateX(Mathf.Deg2Rad(mouseY));

            Vector3 tempRot = _cameraPivot.RotationDegrees;
            tempRot.x = Mathf.Clamp(tempRot.x, MIN_ANGLE, MAX_ANGLE);
            _cameraPivot.RotationDegrees = tempRot;
        }
    }

    void CameraMove()
    {
        if (_cameraRay.IsColliding())
        {
            _cameraOffset.Translation = new Vector3(0, 0, _cameraRay.GlobalTransform.origin.DistanceTo(_cameraRay.GetCollisionPoint()));
        }
        else
        {
            _cameraOffset.Translation = -_cameraRay.CastTo;
        }
    }

    void Walk(float delta)
    {
        Vector3 snap = Vector3.Zero;
        _grounded = IsOnFloor();

        var aim = _playerCamera.GlobalTransform.basis;

        if (_grounded)
        {
            _secondJump = false;
            _speed = 0;
            _direction = Vector3.Zero;

            _canSprint = true;
            if (Input.IsActionPressed("ui_up"))
            {
                _direction -= aim.z;
            }
            if (Input.IsActionPressed("ui_down"))
            {
                _direction += aim.z;
                _canSprint = false;
            }
            if (Input.IsActionPressed("ui_left"))
            {
                _direction -= aim.x;
            }
            if (Input.IsActionPressed("ui_right"))
            {
                _direction += aim.x;
            }
            _direction.y = 0;
            _direction = _direction.Normalized();

            snap = Vector3.Down;
            if (Input.IsActionJustPressed("ui_select"))
            {
                snap = Vector3.Zero;
                _velocity.y = _jumpHeight;
            }

            if (Input.IsActionPressed("sprint") && _canSprint)
            {
                _speed = _sprintSpeed;
                _sprinting = true;
            }
            else
            {
                _speed = _walkSpeed;
                _sprinting = false;
            }
        }
        else
        {
            if (Input.IsActionJustPressed("ui_select") && !_secondJump)
            {
                _secondJump = true;
                _velocity.y = _jumpHeight;
            }
        }

        _velocity.y += -_gravity * _gravityMultiplyer * delta;

        if (Input.IsActionPressed("aim"))
        {
            Vector3 forward = -aim.z;
            Vector2 tempRot = new Vector2(forward.z, forward.x);
            _body.Rotation = new Vector3(0, Mathf.LerpAngle(_body.Rotation.y, tempRot.Angle() + Mathf.Pi, delta * _aimBodyRotationSpeed), 0);

            _playerCamera.Fov = Mathf.Lerp(_playerCamera.Fov, _aimCameraFov, delta * _aimCameraFovInterpolationSpeed);

            _speed = _aimSpeed;
        }
        else
        {
            if (_direction.Length() > 0.001f)
            {
                Vector2 tempRot = new Vector2(_direction.z, _direction.x);
                _body.Rotation = new Vector3(0, Mathf.LerpAngle(_body.Rotation.y, tempRot.Angle() + Mathf.Pi, delta * _bodyRotationSpeed), 0);
            }

            if (_sprinting)
            {
                _playerCamera.Fov = Mathf.Lerp(_playerCamera.Fov, _cameraFov * _sprintCameraFovMod, delta * _cameraFovInterpolationSpeed);
            }
            else
            {
                _playerCamera.Fov = Mathf.Lerp(_playerCamera.Fov, _cameraFov, delta * _cameraFovInterpolationSpeed);
            }
        }

        Vector3 tempVal = _velocity;
        tempVal.y = 0;
        Vector3 target = _direction * _speed;
        float tempAccel = 0;
        if (_direction.Dot(tempVal) > 0)
            tempAccel = _acceleration;
        else
            tempAccel = _deacceleration;
        
        tempVal = tempVal.LinearInterpolate(target, tempAccel * delta);
        _velocity.x = tempVal.x;
        _velocity.z = tempVal.z;

        if (_direction.Dot(_velocity) == 0)
        {
            if (_velocity.x < VELOCITY_CLAMP && _velocity.x > -VELOCITY_CLAMP)
                _velocity.x = 0;
            if (_velocity.z < VELOCITY_CLAMP && _velocity.z > -VELOCITY_CLAMP)
                _velocity.z = 0;
        }

        _velocity.y = MoveAndSlideWithSnap(_velocity, snap, FLOOR_NORMAL, true, 4, Mathf.Deg2Rad(_floorMaxAngle)).y;
    }
}
