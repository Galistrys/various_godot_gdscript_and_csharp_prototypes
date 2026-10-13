using Godot;
using KayEth.Items;
using KayEth.Player;
using System;

namespace KayEth.Entities.Interactables.Rigid.Grabbable
{
    public class FreeGrabbable : GrabbableInteractable
    {
        public override void PlayerInput(float moveX, float moveY, bool rotate, PlayerController player, float delta)
        {
            if (rotate)
            {
                var pos_diff = GlobalTransform.origin - player.GlobalTransform.origin;
                pos_diff.y = 0;
                pos_diff = pos_diff.Normalized();

                AngularVelocity = new Vector3(-pos_diff.z * moveY, moveX, pos_diff.x * moveY) * PlayerController.MOUSE_INCREASE;
            }

            var player_origin = player.MovableGrabPosition.GlobalTransform.origin;
            var dist = player_origin.DistanceTo(GlobalTransform.origin);
            var dir = (player_origin - GlobalTransform.origin);

            LinearVelocity = dir * dist * MOVE_INTERACTABLE_SPEED + player.velocity;
        }

        public override void Push(PlayerController player)
        {
            ApplyCentralImpulse(
                (player.MovableGrabPosition.GlobalTransform.origin - player.PlayerCamera.GlobalTransform.origin)
                    .Normalized() * Weight
            );
        }

        public override bool UseItem(Item item)
        {
            return false;
        }
    }
}
