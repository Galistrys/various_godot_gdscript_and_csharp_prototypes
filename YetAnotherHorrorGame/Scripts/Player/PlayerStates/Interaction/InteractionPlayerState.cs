using Godot;
using System;

namespace HorrorGame.Player.PlayerStates.Interaction
{
    public abstract class InteractionPlayerState : PlayerState
    {
        protected InteractionPlayerState(PlayerEntity player) : base(player)
        {
        }

        public abstract bool CanMovementRecieveInput();
    }
}
