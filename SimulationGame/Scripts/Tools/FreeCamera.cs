using Godot;
using System;

public class FreeCamera : KinematicBody
{
    //Spatial _head;
    Camera _camera;

    float _cameraAngle = 0;
    float _mouseSensitivity = 0.3f;

    Vector3 velocity = Vector3.Zero;
    Vector3 direction = Vector3.Zero;

    const int FLY_SPEED = 40;
    const int FLY_ACCEL = 10;

    const int _RAY_LENGHT = 100;

    public override void _Ready()
    {
        //_head = GetNode<Spatial>("Head");
        //_camera = GetNode<Camera>("Head/Cam");
        _camera = GetNode<Camera>("Cam");

        Input.SetMouseMode(Input.MouseMode.Captured);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion iemm)
        {
            //_head.RotateY(Mathf.Deg2Rad(iemm.Relative.x * _mouseSensitivity));
            this.RotateY(-Mathf.Deg2Rad(iemm.Relative.x * _mouseSensitivity));

            float change = -iemm.Relative.y * _mouseSensitivity;
            if (change + _cameraAngle < 90 && change + _cameraAngle > -90)
            {
                _camera.RotateX(Mathf.Deg2Rad(change));
                _cameraAngle += change;
            }
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        direction = Vector3.Zero;

        var aim = _camera.GetGlobalTransform().basis;
        if (Input.IsKeyPressed((int)KeyList.W))
            direction -= aim.z;
        if (Input.IsKeyPressed((int)KeyList.S))
            direction += aim.z;
        if (Input.IsKeyPressed((int)KeyList.A))
            direction -= aim.x;
        if (Input.IsKeyPressed((int)KeyList.D))
            direction += aim.x;

        direction = direction.Normalized();

        var target = direction * (Input.IsKeyPressed((int)KeyList.Shift) ? FLY_SPEED * 4 : FLY_SPEED);

        velocity = velocity.LinearInterpolate(target, FLY_ACCEL * delta);
        //this.GlobalTranslate(velocity);
        this.MoveAndSlide(velocity, Vector3.Up);

        if (Input.IsActionJustPressed("action_0"))
        {
            // var collisionData = ShootRay(2147483647);
            // CollisionData data = new CollisionData((Godot.Object)collisionData["collider"], (Vector3)collisionData["normal"], (Vector3)collisionData["position"]);
        }
    }

    Godot.Collections.Dictionary ShootRay(int collisionMask)
    {
        Vector2 mousePos = GetTree().Root.GetMousePosition();
        Vector3 from = _camera.ProjectRayOrigin(mousePos);
        Vector3 to = from + _camera.ProjectRayNormal(mousePos) * _RAY_LENGHT;

        var spaceState = GetWorld().DirectSpaceState;
        return spaceState.IntersectRay(from, to, null, collisionMask);
    }
}
