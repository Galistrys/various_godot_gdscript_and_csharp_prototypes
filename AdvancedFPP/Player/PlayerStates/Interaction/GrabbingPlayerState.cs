using Godot;
using SpaceAdventure.Entites.Interactables.Grabbable;
using System;

namespace SpaceAdventure.Player.PlayerStates.Interaction
{
    public class GrabbingPlayerState : InteractionPlayerState
    {
        bool _flag = false;

        bool _movable;
        bool _rotatable;

        IGrabbable _grabbable;

        bool isRotating = false;

        public GrabbingPlayerState(PlayerController playerController, IGrabbable grabbable) : base(playerController)
        {
            _grabbable = grabbable;
            _movable = grabbable is IMovable;
            _rotatable = grabbable is IRotatable;
        }

        public override void OnEnter()
        {
        }

        public override void OnExit()
        {
        }

        public override void Process(float delta)
        {
            if (Input.IsActionJustPressed("interaction"))
            {
                if (_flag)
                    _player.ChangeInteractionState(new IdlePlayerState(_player));
                else
                    _flag = true;
            }
            if (Input.IsActionJustPressed("push_object"))
            {
                _player.ChangeInteractionState(new IdlePlayerState(_player));
                (_grabbable as IMovable).AddCentralImpulse(GetPushVector());
            }

            isRotating = _rotatable && Input.IsActionPressed("rotate_object");
        }

        public override void PhysicsProcess(float delta)
        {
            if (isRotating)
                (_grabbable as IRotatable).SetNewAngularVelocity(GetAngularVelocity(delta));

            if (_movable)
                (_grabbable as IMovable).SetNewLinearVelocity(GetLinearVelocity(delta));
        }

        public override void ProcessInput(InputEvent e)
        {
        }

        public override bool CanMovementRecieveInput()
        {
            return true;
        }

        public override bool CanRotationRecieveInput()
        {
            return !isRotating;
        }

        Vector3 GetLinearVelocity(float delta)
        {
            var tp = _player.MovableTargetPosition.GlobalTranslation;
            var gp = _grabbable.GetGlobalPosition();

            float dist = tp.DistanceTo(gp);
            Vector3 dir = (tp - gp).Normalized();

            return dir * dist * PlayerController.MOVE_INTERACTABLE_SPEED + _player.velocity;
        }

        Vector3 GetAngularVelocity(float delta)
        {
            float mouseX = _player.ViewChange.x * delta * 10;
            float mouseY = _player.ViewChange.y * delta * 10;

            return _player.Transform.basis.Xform(new Vector3(mouseY, mouseX, 0));
        }

        Vector3 GetPushVector()
        {
            return _player.PlayerCamera.Transform.basis.Xform(Vector3.Forward);
        }
    }
}