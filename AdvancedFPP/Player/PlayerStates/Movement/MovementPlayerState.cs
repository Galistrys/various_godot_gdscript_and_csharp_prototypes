using Godot;
using System;

namespace SpaceAdventure.Player.PlayerStates.Movement
{
    public abstract class MovementPlayerState
    {
        protected PlayerController _player;

        protected Vector3 _rotateInput = Vector3.Zero;
        protected Vector3 _directionInput = Vector3.Zero;

        public MovementPlayerState(PlayerController playerController)
        {
            _player = playerController;
        }

        // public abstract void ProcessInput(InputEvent e);
        public abstract void OnEnter();
        public abstract void OnExit();

        public void UpdateRotationInputs()
        {
            _rotateInput = new Vector3(
                _player.ViewChange.x,
                _player.ViewChange.y,
                (
                    Input.GetActionStrength("rotate_left") - Input.GetActionStrength("rotate_right")
                )
            );
        }

        public void UpdateMovementInputs()
        {
            _directionInput = _player.GetInputDirection();
        }

        public void ZeroRotationInputs()
        {
            _rotateInput = Vector3.Zero;
        }

        public void ZeroMovementInputs()
        {
            _directionInput = Vector3.Zero;
        }

        public abstract void Process(float delta);
        public abstract void PhysicsProcess(float delta);
    }
}