using Godot;
using SpaceAdventure.Entites.Interactables;
using System;

namespace SpaceAdventure.Player.PlayerStates.Interaction
{
    public class IdlePlayerState : InteractionPlayerState
    {
        IInteractable _currentlyDetectedInteractable = null;


        public IdlePlayerState(PlayerController playerController) : base(playerController)
        {
        }

        public override void OnEnter()
        {
            _player.InteractionRayCast.Enabled = true;
        }

        public override void OnExit()
        {
            _player.InteractionRayCast.Enabled = false;
        }

        public override void ProcessInput(InputEvent e)
        {
            
        }

        public override void PhysicsProcess(float delta)
        {
            DetectInteractable();
            if (_currentlyDetectedInteractable != null)
            {
                if (Input.IsActionJustPressed("interaction"))
                    _player.ChangeInteractionState(_currentlyDetectedInteractable.OnInteraction(_player));
            }
        }

        public override void Process(float delta)
        {

        }

        public override bool CanMovementRecieveInput()
        {
            return true;
        }

        public override bool CanRotationRecieveInput()
        {
            return true;
        }

        void DetectInteractable()
        {
            var collider = _player.InteractionRayCast.GetCollider();
            if (collider is IInteractable interactable)
            {
                if (_currentlyDetectedInteractable != interactable)
                {
                    GD.Print($"Detected: {interactable.InteractableName}");
                }
                
                _currentlyDetectedInteractable = interactable;
            }
            else
            {
                _currentlyDetectedInteractable = null;
            }
        }
    }
}