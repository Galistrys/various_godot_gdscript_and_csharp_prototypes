using Godot;
using HorrorGame.Entities.Interactables.Grabbable;
using System;

namespace HorrorGame.Player.PlayerStates.Interaction
{
    public class GrabbingPlayerState : InteractionPlayerState
    {
        GrabbableInteractable _grabbableInteractable;
        public Vector3 InteractionPoint { get; private set; }

        public GrabbingPlayerState(PlayerEntity player, GrabbableInteractable grabbableInteractable) : base(player)
        {
            _grabbableInteractable = grabbableInteractable;
            
            float zVal = Mathf.Clamp(-(_player.PlayerCamera.GlobalTransform.origin.DistanceTo(_grabbableInteractable.GlobalTransform.origin)), -PlayerEntity.MAX_DISTANCE, -PlayerEntity.MIN_DISTANCE);
            _player.MovablePosition.Translation = new Vector3(0, 0, zVal);
        }

        public override bool CanMovementRecieveInput()
        {
            return !_grabbableInteractable.BlockCamera;
        }

        public override void OnEnter()
        {
            _grabbableInteractable.BeginInteraction();
        }

        public override void OnExit()
        {
            _grabbableInteractable.EndInteraction();
            _grabbableInteractable = null;
        }

        public override void PhysicsProcess(float delta)
        {
            if (_grabbableInteractable.AllowRotation && Input.IsActionPressed("rotate"))
            {
                _grabbableInteractable.MouseInput(_player.viewChange.x * delta, _player.viewChange.y * delta, true, _player);
                _player.viewChange = Vector2.Zero;
            }
            else
            {
                _grabbableInteractable.MouseInput(_player.viewChange.x * delta, _player.viewChange.y * delta, false, _player);
            }

            if (Input.IsActionJustReleased("interact"))
            {
                _player.ChangeInteractionState(new IdlePlayerState(_player));
            }

            if (Input.IsActionJustPressed("aim"))
            {
                _grabbableInteractable.Push(_player);
                _player.ChangeInteractionState(new IdlePlayerState(_player));
            }
        }

        public override void ProcessInput(InputEvent e)
        {
            
        }

        public override void UpdateProcess(float delta)
        {
            if (Input.IsActionPressed("bring_further"))
                BringFurther(delta);
            if (Input.IsActionPressed("bring_closer"))
                BringCloser(delta);

            if (_player.GlobalTransform.origin.DistanceTo(_grabbableInteractable.GlobalTransform.origin) > PlayerEntity.MAX_DISTANCE)
                _player.ChangeInteractionState(new IdlePlayerState(_player));
        }

        void BringCloser(float delta)
        {
            Vector3 temp = new Vector3(0, 0, Mathf.Clamp(_player.MovablePosition.Translation.z + delta, -PlayerEntity.MAX_DISTANCE, -PlayerEntity.MIN_DISTANCE));
            _player.MovablePosition.Translation = temp;
        }

        void BringFurther(float delta)
        {
            Vector3 temp = new Vector3(0, 0, Mathf.Clamp(_player.MovablePosition.Translation.z - delta, -PlayerEntity.MAX_DISTANCE, -PlayerEntity.MIN_DISTANCE));
            _player.MovablePosition.Translation = temp;
        }
    }
}