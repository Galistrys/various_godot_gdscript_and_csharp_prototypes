using Godot;
using System;

namespace HorrorGame.Player.PlayerStates.Movement
{
    public class WalkPlayerState : MovementPlayerState
    {
        Vector3 _directionChange = Vector3.Zero;

        bool _grounded = true;
        Vector3 _direction = Vector3.Zero;
        bool _canSprint = true;

        bool _sprinting = false;
        bool _crouching = false;

        public WalkPlayerState(PlayerEntity player) : base(player)
        {
        }

        public override void OnEnter()
        {
            _player.ColShape.Disabled = false;
            _player.CrouchColShape.Disabled = true;
            _player.FlyColShape.Disabled = true;

            _player.HeadSpatial.Rotation = Vector3.Zero;

            _player.Transform = _player.GlobalTransform.LookingAt(_player.GlobalTransform.origin - _player.GlobalTransform.basis.z, -_player.gravityVector.Normalized());
        }

        public override void OnExit()
        {
            
        }

        public override void PhysicsProcess(float delta)
        {
            CameraRotation(delta);
            Walk(delta);
        }

        public override void ProcessInput(InputEvent e)
        {
            if (e is InputEventMouseMotion iemm)
            {
                var temp_rel = iemm.Relative;
                temp_rel.x = Mathf.Clamp(temp_rel.x, -PlayerEntity.MOUSE_CLAMP, PlayerEntity.MOUSE_CLAMP);
                temp_rel.y = Mathf.Clamp(temp_rel.y, -PlayerEntity.MOUSE_CLAMP, PlayerEntity.MOUSE_CLAMP);
                temp_rel /= PlayerEntity.MOUSE_CLAMP;

                _player.viewChange = temp_rel * _player.MouseSensitivity;
            }
        }

        public override void UpdateProcess(float delta)
        {
            _player.viewChange.x = Input.GetActionStrength("look_right") - Input.GetActionStrength("look_left");
            _player.viewChange.y = Input.GetActionStrength("look_down") - Input.GetActionStrength("look_up");

            _directionChange.x = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
            _directionChange.z = Input.GetActionStrength("move_backward") - Input.GetActionStrength("move_forward");
        }

        void CameraRotation(float delta)
        {
            if (Input.GetMouseMode() != Input.MouseMode.Captured)
                return;
            
            if (_player.viewChange.LengthSquared() > 0)
            {
                float change_x = -_player.viewChange.x * 100 * delta;
                float change_y = -_player.viewChange.y * 100 * delta;

                _player.viewChange = _player.viewChange.LinearInterpolate(Vector2.Zero, PlayerEntity.VIEW_CHANGE_SMOOTHING);

                _player.RotateY(Mathf.Deg2Rad(change_x));
                _player.HeadSpatial.RotateX(Mathf.Deg2Rad(change_y));

                var temp_rot = _player.HeadSpatial.RotationDegrees;
                temp_rot.x = Mathf.Clamp(temp_rot.x, PlayerEntity.MIN_ANGLE, PlayerEntity.MAX_ANGLE);
                _player.HeadSpatial.RotationDegrees = temp_rot;
            }
        }

        void Walk(float delta)
        {
            _direction = Vector3.Zero;
            var aim = _player.GlobalTransform.basis;

            var snap = Vector3.Zero;
            _grounded = _player.IsOnFloor();

            _canSprint = !(_directionChange.z > 0);

            _direction += aim.z * _directionChange.z;
            _direction += aim.x * _directionChange.x;
            _direction.y = 0;

            if (_directionChange.LengthSquared() > 1)
                _direction = _direction.Normalized();

            _directionChange = Vector3.Zero;

            if (_grounded)
            {
                snap = _player.gravityVector.Normalized();
                if (Input.IsActionJustPressed("jump"))
                {
                    snap = Vector3.Zero;
                    _player.velocity.y = PlayerEntity.JUMP_STRENGTH;
                }
            }

            _player.velocity.y += -_player.gravityVector.Length() * 2 * delta;

            float speed = 0;
            if (Input.IsActionPressed("crouch") && _grounded)
            {
                speed = PlayerEntity.CROUCH_SPEED;
                _player.PlayerCamera.Fov = Mathf.Lerp(_player.PlayerCamera.Fov, PlayerEntity.CAMERA_FOV, delta * 8);
                _sprinting = false;
                _crouching = true;

                _player.ColShape.Disabled = true;
                _player.CrouchColShape.Disabled = false;
            }
            else if (!_crouching && Input.IsActionPressed("sprint") && _canSprint)
            {
                speed = PlayerEntity.SPRINT_SPEED;
                _player.PlayerCamera.Fov = Mathf.Lerp(_player.PlayerCamera.Fov, PlayerEntity.CAMERA_FOV * 1.05f, delta * 8);
                _sprinting = true;
                _crouching = false;

                _player.ColShape.Disabled = false;
                _player.CrouchColShape.Disabled = true;
            }
            else
            {
                speed = PlayerEntity.WALK_SPEED;
                _player.PlayerCamera.Fov = Mathf.Lerp(_player.PlayerCamera.Fov, PlayerEntity.CAMERA_FOV, delta * 8);
                _sprinting = false;
                _crouching = false;

                _player.ColShape.Disabled = false;
                _player.CrouchColShape.Disabled = true;
            }

            if (_crouching)
            {
                var temp_transform = _player.HeadSpatial.Transform;
                temp_transform.origin.y = Mathf.Lerp(temp_transform.origin.y, _player.InitialHeadY + PlayerEntity.CROUCH_HEIGHT_DIFF, delta * PlayerEntity.HEIGHT_CHANGE_SPEED);
                _player.HeadSpatial.Transform = temp_transform;
            }
            else if (_player.HeadSpatial.Transform.origin.y != 0)
            {
                var temp_transform = _player.HeadSpatial.Transform;
                temp_transform.origin.y = Mathf.Lerp(temp_transform.origin.y, _player.InitialHeadY, delta * PlayerEntity.HEIGHT_CHANGE_SPEED);
                _player.HeadSpatial.Transform = temp_transform;
            }

            var temp_vel = _player.velocity;
            temp_vel.y = 0;
            var target = _direction * speed;

            var temp_accel = 0f;
            if (!_grounded)
                temp_accel = PlayerEntity.MID_AIR_ACCEL;
            else if (_direction.Dot(temp_vel) > 0)
                temp_accel = PlayerEntity.ACCELERATION;
            else
                temp_accel = PlayerEntity.DEACCELERATION;

            temp_vel = temp_vel.LinearInterpolate(target, temp_accel * delta);
            _player.velocity.x = temp_vel.x;
            _player.velocity.z = temp_vel.z;

            if (_direction.Dot(_player.velocity) == 0)
            {
                if (_player.velocity.x < PlayerEntity.VELOCITY_CAP && _player.velocity.x > -PlayerEntity.VELOCITY_CAP)
                    _player.velocity.x = 0;
                if (_player.velocity.z < PlayerEntity.VELOCITY_CAP && _player.velocity.z > -PlayerEntity.VELOCITY_CAP)
                    _player.velocity.z = 0;
            }

            _player.velocity.y = _player.MoveAndSlideWithSnap(_player.velocity, snap, -_player.gravityVector.Normalized(), true, 4, Mathf.Deg2Rad(PlayerEntity.FLOOR_MAX_ANGLE), false).y;
        }
    }
}
