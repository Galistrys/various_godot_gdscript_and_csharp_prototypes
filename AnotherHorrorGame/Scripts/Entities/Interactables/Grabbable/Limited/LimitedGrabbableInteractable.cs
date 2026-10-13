using Godot;
using HorrorGame.Player;
using HorrorGame.Player.PlayerStates;
using System;

namespace HorrorGame.Entities.Interactables.Grabbable.Limited
{
    public abstract class LimitedGrabbableInteractable : GrabbableInteractable
    {
        public const float MOUSE_SENSITIVITY = 0.1f;

        public override bool BlockCamera { get { return true; } }

        public abstract void LimitTranslation();
        public abstract void LimitRotation();

        public override void _Process(float delta)
        {
            LimitTranslation();
            LimitRotation();
        }
    }
}