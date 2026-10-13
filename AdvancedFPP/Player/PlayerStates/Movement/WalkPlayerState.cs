using Godot;
using System;

namespace SpaceAdventure.Player.PlayerStates.Movement
{
    public class WalkPlayerState : MovementPlayerState
    {
        Vector3 _snap = Vector3.Zero;
        Vector3 _gravity = Vector3.Zero;

        bool _canSprint = true;

        // bool _sprinting = false;
        bool _crouching = false;

        public WalkPlayerState(PlayerController playerController) : base(playerController)
        {
        }

        public override void OnEnter()
        {
            _player.Rotation *= Vector3.Up;

            _player.DownRayCast.ForceUpdateTransform();
            _player.DownRayCast.ForceRaycastUpdate();

            _player.CrouchRayCast.ForceUpdateTransform();
            _player.CrouchRayCast.ForceRaycastUpdate();

            if (_player.DownRayCast.GetCollider() != null)
            {
                _player.GlobalTranslation = _player.DownRayCast.GetCollisionPoint();
            }
            else
            {
                _player.GlobalTransform = _player.GlobalTransform.Translated(
                    _player.HeadSpatial.Translation - _player.HeadStandPos.Translation
                );
            }

            if (_player.CrouchRayCast.GetCollider() == null)
            {
                _player.HeadSpatial.Translation = _player.HeadStandPos.Translation;

                _player.EnableStandColShape();

                _crouching = true;
            }
            else
            {
                _player.HeadSpatial.Translation = _player.HeadCrouchPos.Translation;

                _player.EnableCrouchColShape();

                _crouching = false;
            }
        }

        public override void OnExit()
        {
            
        }

        public override void PhysicsProcess(float delta)
        {
            CameraRotation(delta);
            Walk(delta);

            if (_crouching)
                _player.HeadSpatial.Translation = _player.HeadSpatial.Translation.LinearInterpolate(_player.HeadCrouchPos.Translation, PlayerController.HEIGHT_CHANGE_SPEED * delta);
            else
                _player.HeadSpatial.Translation = _player.HeadSpatial.Translation.LinearInterpolate(_player.HeadStandPos.Translation, PlayerController.HEIGHT_CHANGE_SPEED * delta);
        }

        public override void Process(float delta)
        {
            
        }

        // public override void ProcessInput(InputEvent e)
        // {
        // }

        void CameraRotation(float delta)
        {
            if (Input.MouseMode != Input.MouseModeEnum.Captured)
                return;
            
            if (_rotateInput.LengthSquared() > 0)
            {
                _player.RotateY(-_rotateInput.x * delta);
                _player.HeadSpatial.RotateX(-_rotateInput.y * delta);

                var tempRot = _player.HeadSpatial.Rotation;
                tempRot.x = Mathf.Clamp(tempRot.x, PlayerController.MIN_ANGLE, PlayerController.MAX_ANGLE);
                _player.HeadSpatial.Rotation = tempRot;
            }
        }

        void Walk(float delta)
        {
            Vector3 targetDir = _player.GetInputDirection();
            targetDir.y = 0;
            if (targetDir.LengthSquared() > 1)
                targetDir = targetDir.Normalized();
            
            _canSprint = !Input.IsActionPressed("move_backward") && !_crouching;
            
            float speed = PlayerController.WALK_SPEED;
            float accel = PlayerController.JUMP_ACCELERATION;
            
            if (_player.IsOnFloor())
            {
                if (Input.IsActionJustPressed("jump"))
                {
                    _snap = Vector3.Zero;
                    _gravity = Vector3.Up * PlayerController.JUMP_STRENGTH;

                    _crouching = false;
                }
                else
                {
                    _gravity = Vector3.Zero;
                    _snap = -_player.GetFloorNormal();
                }

                if (Input.IsActionPressed("crouch") || _player.CrouchRayCast.GetCollider() != null)
                {
                    speed = PlayerController.CROUCH_SPEED;
                    // _sprinting = false;
                    _crouching = true;

                    _player.EnableCrouchColShape();
                }
                else if (!_crouching && Input.IsActionPressed("sprint") && _canSprint)
                {
                    speed = PlayerController.SPRINT_SPEED;
                    // _sprinting = true;
                    _crouching = false;

                    _player.EnableStandColShape();
                }
                else
                {
                    speed = PlayerController.WALK_SPEED;
                    // _sprinting = false;
                    _crouching = false;

                    _player.EnableStandColShape();
                }
                
                
                if (targetDir.LengthSquared() > 0)
                    accel = PlayerController.ACCELERATION;
                else
                    accel = PlayerController.DEACCELERATION;
            }
            else
            {
                _gravity += _player.GravityAcceleration * Vector3.Down * delta;
                _snap = Vector3.Down;
            }

            _player.velocity = _player.velocity.LinearInterpolate(targetDir * speed, delta * accel);
            _player.MoveAndSlideWithSnap(_player.velocity + _gravity, _snap, Vector3.Up, infiniteInertia: false);
        }
    }
}