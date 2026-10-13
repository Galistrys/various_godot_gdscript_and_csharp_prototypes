using Godot;
using System;

namespace SpaceAdventure.Player.PlayerStates.Interaction
{
    public abstract class InteractionPlayerState
    {
        protected PlayerController _player;

        public InteractionPlayerState(PlayerController playerController)
        {
            _player = playerController;
        }

        public abstract void ProcessInput(InputEvent e);
        public abstract void Process(float delta);
        public abstract void PhysicsProcess(float delta);
        public abstract void OnEnter();
        public abstract void OnExit();

        public abstract bool CanRotationRecieveInput();
        public abstract bool CanMovementRecieveInput();

        
    }
}