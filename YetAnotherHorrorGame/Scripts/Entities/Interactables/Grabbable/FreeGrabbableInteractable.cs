using Godot;
using HorrorGame.Player;
using System;

namespace HorrorGame.Entities.Interactables.Grabbable
{
    public class FreeGrabbableInteractable : GrabbableInteractable
    {
        public override bool BlockCamera { get { return false; } }

        public override void MouseInput(float mouseX, float mouseY, bool rotate, PlayerEntity player)
        {
            if (rotate)
            {
                Vector3 posDif = this.GlobalTransform.origin - player.GlobalTransform.origin;
                posDif.y = 0;
                posDif = posDif.Normalized();

                this.AngularVelocity = new Vector3(-posDif.z * mouseY, mouseX, posDif.x * mouseY);
            }

            float dist = player.MovablePosition.GlobalTransform.origin.DistanceTo(this.GlobalTransform.origin);
            Vector3 dir = (player.MovablePosition.GlobalTransform.origin - this.GlobalTransform.origin).Normalized();

            this.LinearVelocity = dir * dist * PlayerEntity.MOVE_INTERACTABLE_SPEED + player.velocity;
        }

        public override void Push(PlayerEntity player)
        {
            this.ApplyCentralImpulse((player.MovablePosition.GlobalTransform.origin - player.PlayerCamera.GlobalTransform.origin).Normalized() * this.Weight);
        }

        public override bool UseItem(string itemId)
        {
            return false;
        }
    }
}