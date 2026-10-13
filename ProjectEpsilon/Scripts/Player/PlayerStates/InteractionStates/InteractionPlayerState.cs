using Godot;
using System;

namespace KayEth.Player.PlayerStates.InteractionStates
{
    public abstract class InteractionPlayerState : PlayerState
    {
        public InteractionPlayerState(PlayerController player) : base(player)
        {
        }

        public abstract bool CanMovementRecieveInput();
    }
}
