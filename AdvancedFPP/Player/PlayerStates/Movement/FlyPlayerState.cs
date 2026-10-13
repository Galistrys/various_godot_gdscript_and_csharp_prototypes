using Godot;
using System;

namespace SpaceAdventure.Player.PlayerStates.Movement
{
    public class FlyPlayerState : MovementPlayerState
    {
        public Vector3 _targetRotationSpeed = Vector3.Zero;
        public Vector3 _currentRotationSpeed = Vector3.Zero;

        

        public FlyPlayerState(PlayerController playerController) : base(playerController)
        {
        }

        public override void OnEnter()
        {
            _player.EnableFlyColShape();

            _player.GlobalTranslation = _player.HeadSpatial.GlobalTranslation;
            _player.HeadSpatial.Translation = _player.HeadFlyPos.Translation;
            var tempRot = _player.Rotation;
            tempRot.x = _player.HeadSpatial.Rotation.x;
            _player.Rotation = tempRot;
            _player.HeadSpatial.Rotation = Vector3.Zero;
        }

        public override void OnExit()
        {
        }

        public override void PhysicsProcess(float delta)
        {
            CameraRotation(delta);
            Fly(delta);
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
            
            _targetRotationSpeed = _rotateInput;
            // _rotateInput = Vector3.Zero;

            var accel = _targetRotationSpeed.LengthSquared() > 0 ? PlayerController.FLY_ROTATION_ACCELERATION : PlayerController.FLY_ROTATION_DEACCELERATION;
            _currentRotationSpeed = _currentRotationSpeed.LinearInterpolate(_targetRotationSpeed * PlayerController.FLY_ROTATION_SPEED, delta * accel);

            _player.RotateObjectLocal(Vector3.Down, _currentRotationSpeed.x * delta);
            _player.RotateObjectLocal(Vector3.Left, _currentRotationSpeed.y * delta);
            _player.RotateObjectLocal(Vector3.Back, _currentRotationSpeed.z * delta);
            // _player.RotateObjectLocal(_currentRotationSpeed.Normalized(), _currentRotationSpeed.Length() * delta);
        }

        void Fly(float delta)
        {
            Vector3 targetDir = _directionInput;
            if (targetDir.LengthSquared() > 1)
                targetDir = targetDir.Normalized();
            
            var accel = targetDir.LengthSquared() > 0 ? PlayerController.FLY_ACCELERATION : PlayerController.FLY_DEACCELERATION;
            _player.velocity = _player.velocity.LinearInterpolate(targetDir * PlayerController.FLY_SPEED, accel * delta);

            _player.velocity = _player.MoveAndSlide(_player.velocity, _player.Transform.basis.Xform(Vector3.Up));
        }
    }
}