using Godot;
using System;

namespace HorrorGame.Player.PlayerStates
{    
    public abstract class PlayerState
    {
        static private PlayerController _player = null;
        public static PlayerController PlayerController { get { return _player; } }


        public static void SetPlayerController(PlayerController controller)
        {
            _player = controller;
        }

        public void ProcessInput(ref InputEvent inputEvent)
        {
            HandleInput(ref inputEvent);
        }

        public void Process(float delta)
        {
            UpdateLoop(delta);
        }

        public virtual void HandleInput(ref InputEvent inputEvent) {}
        public virtual void UpdateLoop(float delta) {}
        public virtual void PhysicsUpdateLoop(float delta) {}

        public virtual void OnEnter() { Logger.Log("OnEnter: " + this.GetType().Name); }
        public virtual void OnResume() { Logger.Log("OnResume: " + this.GetType().Name); }
        public virtual void OnPause() { Logger.Log("OnPause: " + this.GetType().Name); }
        public virtual void OnExit() { Logger.Log("OnExit: " + this.GetType().Name); }

        public Vector3 GetDir()
        {
            return new Vector3(
				Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left"),
				0,
				Input.GetActionStrength("move_backward") - Input.GetActionStrength("move_forward")
			);
        }
    }
}