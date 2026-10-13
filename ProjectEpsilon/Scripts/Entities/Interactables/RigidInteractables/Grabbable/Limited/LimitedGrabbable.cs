using Godot;
using KayEth.Items;
using KayEth.Player;
using System;

namespace KayEth.Entities.Interactables.Rigid.Grabbable.Limited
{
    public abstract class LimitedGrabbable : GrabbableInteractable
    {
        public abstract void LimitTranslation();
        public abstract void LimitRotation();

        public override void _Process(float delta)
        {
            LimitTranslation();
            LimitRotation();
        }
    }
}
