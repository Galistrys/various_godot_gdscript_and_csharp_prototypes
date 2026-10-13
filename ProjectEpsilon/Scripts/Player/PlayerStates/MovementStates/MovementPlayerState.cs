using Godot;
using System;

namespace KayEth.Player.PlayerStates.MovementStates
{
    public abstract class MovementPlayerState : PlayerState
    {
        public Vector2 ViewChange { get; protected set; }

        public MovementPlayerState(PlayerController player) : base(player)
        {
        }

        public void ZeroViewChange()
        {
            ViewChange = Vector2.Zero;
        }
    }
}
