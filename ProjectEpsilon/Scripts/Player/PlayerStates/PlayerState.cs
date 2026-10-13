using Godot;
using System;

namespace KayEth.Player.PlayerStates
{
    public abstract class PlayerState
    {
        protected PlayerController Player { get; private set; }

        public PlayerState(PlayerController player)
        {
            Player = player;
        }

        public void Destroy()
        {
            Player = null;
        }

        public abstract void ProcessInput(InputEvent inputEvent);
        public abstract void Process(float delta);
        public abstract void PhysicsProcess(float delta);
        public abstract void OnEnter();
        public abstract void OnExit();
    }
}
