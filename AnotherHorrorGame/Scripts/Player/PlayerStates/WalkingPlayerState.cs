using Godot;
using HorrorGame.Entities.Interactables;
using HorrorGame.Entities.Interactables.Grabbable;
using HorrorGame.GUI.HUD;
using System;

namespace HorrorGame.Player.PlayerStates
{
    public class WalkingPlayerState : PlayerState
    {
        IInteractable _currentInteractable;

        public override void UpdateLoop(float delta)
        {
            PlayerController.UpdateCameraRotation(delta);
            PlayerController.SetMovementDirection(GetDir());

            if (Input.IsActionJustPressed("jump"))
            {
                PlayerController.Jump();
            }

            var detected = DetectInteractable();

            if (detected == null)
            {
                if (_currentInteractable != null)
                    PlayerController.GameManager.HUDController.Pointer.ShowPointer(PointerType.Default);
                _currentInteractable = null;
            }
            else
            {
                if (detected != _currentInteractable)
                {
                    _currentInteractable = detected;
                    Logger.Log($"Detected: {_currentInteractable.InteractableId}");
                    PlayerController.GameManager.HUDController.Pointer.ShowPointer(_currentInteractable.OnRaycastDetectionPointer);
                }
            }

            if (_currentInteractable != null)
            {
                if (Input.IsActionJustPressed("interact"))
                {
                    InteractWithInteractable(_currentInteractable);
                }
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();

            PlayerController.ColShape.Disabled = false;
            PlayerController.CrouchColShape.Disabled = true;
        }

        public override void OnResume()
        {
            base.OnResume();
        }

        protected virtual void InteractWithInteractable(IInteractable interactable)
        {
            _currentInteractable.OnInteraction(PlayerController);
            _currentInteractable = null;
            PlayerController.GameManager.HUDController.Pointer.ShowPointer(PointerType.Default);
        }

        IInteractable DetectInteractable()
        {
            var collider = PlayerController.InteractionRay.GetCollider();
            if (collider is IInteractable inter)
            {
                // if (inter != _currentInteractable)
                // {
                //     _currentInteractable = inter;
                //     Logger.Log($"Detected: {_currentInteractable.InteractableId}");
                //     return inter;
                //     // PlayerController.GameManager.HUDController.Pointer.ShowPointer(_currentInteractable.OnRaycastDetectionPointer);
                // }

                return inter;
            }
            // else
            // {
            //     // if (_currentInteractable != null)
            //     //     PlayerController.GameManager.HUDController.Pointer.ShowPointer(PointerType.Default);
            //     _currentInteractable = null;
            // }

            return null;
        }
    }
}