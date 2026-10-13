using Godot;
using HorrorGame.Player;
using System;
namespace HorrorGame.Entities.Interactables.Grabbable
{
    public class FreeGrabbableInteractable : GrabbableInteractable
    {
        public override bool BlockCamera { get { return false; } }

        public override void MouseInput(float mouseX, float mouseY, bool rotate, PlayerController player)
        {
            if (rotate)
            {
                Vector3 posDif = this.GlobalTransform.origin - player.GlobalTransform.origin;
                posDif.y = 0;
                posDif = posDif.Normalized();

                this.AngularVelocity = new Vector3(-posDif.z * mouseY, mouseX, posDif.x * mouseY);
            }

            float dist = player.MovableGrabPosition.GlobalTransform.origin.DistanceTo(this.GlobalTransform.origin);
            Vector3 dir = (player.MovableGrabPosition.GlobalTransform.origin - this.GlobalTransform.origin).Normalized();

            this.LinearVelocity = dir * dist * PlayerController.MOVE_INTERACTABLE_SPEED + player.velocity + Vector3.Up * player.verticalVelocity;
        }

        public override void Push(PlayerController player)
        {
            this.ApplyCentralImpulse((player.MovableGrabPosition.GlobalTransform.origin - player.PlayerCamera.GlobalTransform.origin).Normalized() * this.Weight);
        }

        public override bool UseItem(string itemId)
        {
            return false;
        }
    }
}