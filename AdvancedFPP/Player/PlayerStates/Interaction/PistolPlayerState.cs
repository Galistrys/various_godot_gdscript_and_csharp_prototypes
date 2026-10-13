using Godot;
using SpaceAdventure.Player.Tools;
using System;

namespace SpaceAdventure.Player.PlayerStates.Interaction
{
    public class PistolPlayerState : InteractionPlayerState
    {
        bool _showHidePlaying = true;

        public PistolPlayerState(PlayerController playerController) : base(playerController)
        {
        }

        public override void OnEnter()
        {
            ShowWeapon();
        }

        public override void OnExit()
        {
            _player.PistolTool.Visible = false;
        }

        public override void PhysicsProcess(float delta)
        {
            if (Input.IsActionJustPressed("interaction"))
            {
                _player.PistolTool.Shoot();
            }
        }

        public override void Process(float delta)
        {
        }

        public override void ProcessInput(InputEvent e)
        {
            if (e is InputEventKey iek)
            {
                if (iek.Pressed && iek.Scancode == (int)KeyList.G)
                {
                    if (!_showHidePlaying)
                    {
                        HideWeapon();
                    }
                }
            }
        }

        async void ShowWeapon()
        {
            _player.PistolTool.CallDeferred(nameof(Pistol.ShowPistol));
            await _player.ToSignal(_player.PistolTool, nameof(Pistol.AnimationFinished));
            _showHidePlaying = false;
        }

        async void HideWeapon()
        {
            _player.PistolTool.CallDeferred(nameof(Pistol.HidePistol));
            await _player.ToSignal(_player.PistolTool, nameof(Pistol.AnimationFinished));

            _player.CallDeferred(nameof(PlayerController.ChangeToIdleInteractionState));
        }

        public override bool CanMovementRecieveInput()
        {
            return true;
        }

        public override bool CanRotationRecieveInput()
        {
            return true;
        }
    }
}