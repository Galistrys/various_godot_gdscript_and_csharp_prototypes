using Godot;
using System;

namespace KayEth.Entities.Interactables.Rigid.Grabbable.Cables
{
    public class PowerCablePoint : KinematicBody
    {
        public Vector3 PreviousPosition { get; set; }
        public Vector3 CurrentPosition
        {
            get
            {
                return this.GlobalTransform.origin;
            }
            set
            {
                var temp = GlobalTransform;
                temp.origin = value;
                GlobalTransform = temp;
            }
        }

        public bool Locked { get; set; } = false;
    }
}
