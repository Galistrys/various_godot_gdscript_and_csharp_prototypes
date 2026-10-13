using Godot;
using System;

namespace HorrorGame.Player.PlayerStates
{
    public abstract class PlayerState
    {
        protected PlayerEntity _player;

        public PlayerState(PlayerEntity player)
        {
            _player = player;
        }

        public void Destroy()
        {
            _player = null;
        }

        public abstract void ProcessInput(InputEvent e);
        public abstract void UpdateProcess(float delta);
        public abstract void PhysicsProcess(float delta);
        public abstract void OnEnter();
        public abstract void OnExit();
    }
}
