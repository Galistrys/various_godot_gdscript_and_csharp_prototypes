using Godot;
using HorrorGame.Entities.Interactables;
using HorrorGame.GUI.HUD;
using HorrorGame.Managers;
using System;

namespace HorrorGame.Player.PlayerStates.Interaction
{
    public class IdlePlayerState : InteractionPlayerState
    {
        IInteractable _currentInteractable;

        public IdlePlayerState(PlayerEntity player) : base(player)
        {
        }

        public override bool CanMovementRecieveInput() => true;

        public override void OnEnter()
        {
            
        }

        public override void OnExit()
        {
            
        }

        public IInteractable DetectInteractable()
        {
            IInteractable result = null;

            var data = _player.GetInteractionData(8);
            if (data.Contains("collider") && data["collider"] is IInteractable interactable)
            {
                result = interactable;
            }

            return result;
        }

        public override void PhysicsProcess(float delta)
        {
            var detected = DetectInteractable();
            if (detected == null)
            {
                if (_currentInteractable != null)
                    MainManager.Instance.GameManager.HUDController.Pointer.ShowPointer(PointerType.Default);
                _currentInteractable = null;
            }
            else
            {
                if (detected != _currentInteractable)
                {
                    _currentInteractable = detected;
                    Logger.Log($"Detected: {_currentInteractable.InteractableId}");
                    MainManager.Instance.GameManager.HUDController.Pointer.ShowPointer(_currentInteractable.OnRaycastDetectionPointer);
                }
            }

            if (_currentInteractable != null)
            {
                if (Input.IsActionJustPressed("interact"))
                {
                    Interact(_currentInteractable);
                }
            }
        }

        protected virtual void Interact(IInteractable interactable)
        {
            _currentInteractable.OnInteraction(_player);
            _currentInteractable = null;
            MainManager.Instance.GameManager.HUDController.Pointer.ShowPointer(PointerType.Default);
        }

        public override void ProcessInput(InputEvent e)
        {
            
        }

        public override void UpdateProcess(float delta)
        {
            
        }
    }
}