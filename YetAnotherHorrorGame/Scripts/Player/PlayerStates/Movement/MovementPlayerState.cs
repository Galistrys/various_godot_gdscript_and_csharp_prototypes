using Godot;
using System;

namespace HorrorGame.Player.PlayerStates.Movement
{
    public abstract class MovementPlayerState : PlayerState
    {
        protected MovementPlayerState(PlayerEntity player) : base(player)
        {
        }
    }
}
