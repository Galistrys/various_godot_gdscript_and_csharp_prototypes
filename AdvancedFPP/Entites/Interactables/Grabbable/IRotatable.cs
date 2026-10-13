using Godot;
using System;

namespace SpaceAdventure.Player.PlayerStates.Interaction
{
    public interface IRotatable
    {
        void SetNewAngularVelocity(Vector3 angularVelocity);
    }
}