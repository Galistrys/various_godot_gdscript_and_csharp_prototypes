using Godot;
using System;

namespace KayEth.Player.PlayerStates.MovementStates
{
    public class NoGravityState : MovementPlayerState
    {
        Vector3 _directionChange = Vector3.Zero;
        float _barrelChange = 0;
        float _barrelRotation = 0;
        Vector3 _direction = Vector3.Zero;

        public NoGravityState(PlayerController player) : base(player)
        {
        }

        public override void OnEnter()
        {
            Player.ColShape.Disabled = true;
            Player.CrouchColShape.Disabled = true;
            Player.FlyingColShape.Disabled = false;

            Player.HeadSpatial.Rotation = Player.HeadSpatial.Rotation * new Vector3(0, 1, 1);
        }

        public override void OnExit()
        {
            
        }

        public override void PhysicsProcess(float delta)
        {
            CameraRotation(delta);
            BarrelRotate(delta);
            Fly(delta);
        }

        public override void Process(float delta)
        {
            Vector2 view_change = Vector2.Zero;
            view_change.x = Input.GetActionStrength("look_right") - Input.GetActionStrength("look_left");
            view_change.y = Input.GetActionStrength("look_down") - Input.GetActionStrength("look_up");
            ViewChange = view_change;

            _directionChange.x = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
            _directionChange.y = Input.GetActionStrength("move_ip") - Input.GetActionStrength("move_down");
            _directionChange.z = Input.GetActionStrength("move_backward") - Input.GetActionStrength("move_forward");

            _barrelChange = Input.GetActionStrength("rotate_left") - Input.GetActionStrength("rotate_right");
        }

        public override void ProcessInput(InputEvent inputEvent)
        {
            if (inputEvent is InputEventMouseMotion iemm)
            {
                Vector2 temp_rel = iemm.Relative;
                temp_rel.x = Mathf.Clamp(
                    temp_rel.x,
                    -PlayerController.MOUSE_CLAMP,
                    PlayerController.MOUSE_CLAMP
                );
                temp_rel.y = Mathf.Clamp(
                    temp_rel.y,
                    -PlayerController.MOUSE_CLAMP,
                    PlayerController.MOUSE_CLAMP
                );
                temp_rel /= PlayerController.MOUSE_CLAMP;

                ViewChange = temp_rel * Player.MouseSensitivity;
            }
        }

        void CameraRotation(float delta)
        {
            if (Input.GetMouseMode() != Input.MouseMode.Captured)
                return;
            
            if (ViewChange.LengthSquared() > 0)
            {
                float change_x = -ViewChange.x * delta;
                float change_y = -ViewChange.y * delta;

                ViewChange = ViewChange.LinearInterpolate(
                    Vector2.Zero,
                    PlayerController.VIEW_CHANGE_SMOOTHING
                );

                Player.RotateObjectLocal(Vector3.Up, change_x);
                Player.RotateObjectLocal(Vector3.Right, change_y);
            }
        }

        void BarrelRotate(float delta)
        {
            _barrelRotation = Mathf.Lerp(
                _barrelRotation,
                _barrelChange * 2,
                PlayerController.FLY_ROTATION_ACCELERATION
            );
            Player.RotateObjectLocal(Vector3.Back, _barrelRotation * delta);
            _barrelChange = 0;
        }

        void Fly(float delta)
        {
            _direction = Vector3.Zero;
            Basis aim = Player.HeadSpatial.GlobalTransform.basis;

            _direction += aim.z * _directionChange.z;
            _direction += aim.y * _directionChange.y;
            _direction += aim.x * _directionChange.x;

            if (_directionChange.LengthSquared() > 1)
                _direction = _direction.Normalized();
            
            _directionChange = Vector3.Zero;

            var target = _direction * PlayerController.FLY_SPEED;

            if (target.LengthSquared() == 0)
            {
                if (Player.InVacuum)
                    target = Player.velocity;
                Player.velocity = Player.velocity.LinearInterpolate(
                    target,
                    PlayerController.FLY_DEACCELERATION * delta
                );
            }
            else
            {
                Player.velocity = Player.velocity.LinearInterpolate(
                    target,
                    PlayerController.FLY_DEACCELERATION * delta
                );
            }

            Player.velocity = Player.MoveAndSlide(Player.velocity);
        }
    }
}
