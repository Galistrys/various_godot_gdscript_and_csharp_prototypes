using Godot;
using System;

namespace SpaceAdventure.Player.PlayerStates.Interaction
{
    public interface IMovable
    {
        void SetNewLinearVelocity(Vector3 linearVelocity);
        void AddCentralImpulse(Vector3 centralImpulse);
    }
}