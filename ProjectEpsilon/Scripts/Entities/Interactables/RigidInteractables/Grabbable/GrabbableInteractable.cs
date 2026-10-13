using Godot;
using KayEth.Player;
using KayEth.Player.PlayerStates.InteractionStates;
using System;

namespace KayEth.Entities.Interactables.Rigid.Grabbable
{
    public abstract class GrabbableInteractable : RigidInteractable
    {
        public const float MOVE_INTERACTABLE_SPEED = 10;

        [Export] public bool AllowRotation { get; private set; }
        [Export] public virtual bool BlockCamera { get; private set; }

        public override sealed void OnInteraction(PlayerController player)
        {
            player.ChangeInteractionState(new GrabbingState(player, this));
        }

        public abstract void PlayerInput(float moveX, float moveY, bool rotate, PlayerController player, float delta);
        public abstract void Push(PlayerController player);

        public virtual void BeginInteraction()
        {
            GravityScale = 0;
        }

        public virtual void EndInteraction()
        {
            GravityScale = 1;
        }
    }
}
