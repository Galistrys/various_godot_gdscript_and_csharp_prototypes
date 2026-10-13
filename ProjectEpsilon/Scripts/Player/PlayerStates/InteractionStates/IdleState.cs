using Godot;
using KayEth.Entities.Interactables;
using KayEth.Tools;
using System;

namespace KayEth.Player.PlayerStates.InteractionStates
{
    public class IdleState : InteractionPlayerState
    {
        IInteractable _currentInteractable;

        public IdleState(PlayerController player) : base(player)
        {
        }

        public override bool CanMovementRecieveInput() => true;

        public override void OnEnter() {}

        public override void OnExit() {}

        public override void PhysicsProcess(float delta)
        {
            DetectInteractable();
            if (_currentInteractable != null)
                if (Input.IsActionJustPressed("interaction"))
                {
                    _currentInteractable.OnInteraction(Player);
                    _currentInteractable = null;
                }
        }

        public override void Process(float delta) {}

        public override void ProcessInput(InputEvent inputEvent) {}

        void DetectInteractable()
        {
            var result = Player.GetRaycastData(4 | 8 | 16);
            if (result.Count == 0)
            {
                if (_currentInteractable != null)
                {
                    _currentInteractable = null;
                    // TODO: Show on HUD
                }
            }
            else if (result["collider"] is IInteractable interactable)
            {
                if (_currentInteractable != interactable)
                {
                    _currentInteractable = interactable;
                    Logger.DebugS("Interactable detected:", interactable.EntityName);

                    // TODO: Show on HUD
                }
            }
        }
    }
}
