using Godot;
using HorrorGame.Entities.Interactables.Grabbable.Limited;
using System;

namespace HorrorGame.Player.PlayerStates
{
    public class LimitedGrabbingPlayerState : PlayerState
    {
        LimitedGrabbableInteractable _limitedGrabbableInteractable;
        
        public LimitedGrabbingPlayerState(LimitedGrabbableInteractable grabbableInteractable)
        {
            _limitedGrabbableInteractable = grabbableInteractable;
            _limitedGrabbableInteractable.GravityScale = 0;
        }

        public override void UpdateLoop(float delta)
        {
            base.UpdateLoop(delta);

            PlayerController.SetMovementDirection(GetDir());
        }

        public override void OnExit()
        {
            base.OnExit();

            _limitedGrabbableInteractable = null;
        }
    }
}