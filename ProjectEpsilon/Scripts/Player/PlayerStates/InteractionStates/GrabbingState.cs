using Godot;
using KayEth.Entities.Interactables.Rigid.Grabbable;
using KayEth.Tools;
using System;

namespace KayEth.Player.PlayerStates.InteractionStates
{
    public class GrabbingState : InteractionPlayerState
    {
        GrabbableInteractable _grabbableInteractable;

        public GrabbingState(PlayerController player, GrabbableInteractable gi) : base(player)
        {
            _grabbableInteractable = gi;

            Logger.DebugS("Grabbing interactable:", gi.EntityName);

            float z_val = Mathf.Clamp(
                -player.PlayerCamera.GlobalTransform.origin.DistanceTo(_grabbableInteractable.GlobalTransform.origin),
                -PlayerController.MAX_DISTANCE,
                -PlayerController.MIN_DISTANCE
            );

            player.MovableGrabPosition.Translation = new Vector3(0, 0, z_val);
        }

        public override bool CanMovementRecieveInput()
        {
            return true;
        }

        public override void OnEnter()
        {
            _grabbableInteractable.BeginInteraction();
        }

        public override void OnExit()
        {
            _grabbableInteractable.EndInteraction();
            _grabbableInteractable = null;
        }

        public override void PhysicsProcess(float delta)
        {
            var view_change = Player.GetViewChange();

            if (_grabbableInteractable.AllowRotation && Input.IsActionPressed("rotate"))
            {
                Player.ZeroViewChange();
                _grabbableInteractable.PlayerInput(view_change.x * delta, view_change.y * delta, true, Player, delta);
            }
            else
            {
                _grabbableInteractable.PlayerInput(view_change.x * delta, view_change.y * delta, false, Player, delta);
            }

            if (Input.IsActionJustReleased("rotate"))
            {
                _grabbableInteractable.AngularVelocity = Vector3.Zero;
            }

            if (Input.IsActionJustReleased("interaction"))
                Player.ChangeInteractionState(new IdleState(Player));
            
            if (Input.IsActionJustPressed("push"))
            {
                _grabbableInteractable.Push(Player);
                Player.ChangeInteractionState(new IdleState(Player));
            }
        }

        void MoveGrabbablePosition(float change)
        {
            float z_val = Mathf.Clamp(
                    Player.MovableGrabPosition.Translation.z + change,
                    -PlayerController.MAX_DISTANCE,
                    -PlayerController.MIN_DISTANCE
                );

                Player.MovableGrabPosition.Translation = new Vector3(0, 0, z_val);
        }

        public override void Process(float delta)
        {
            if (Player.GlobalTransform.origin.DistanceTo(_grabbableInteractable.GlobalTransform.origin) > PlayerController.MAX_DISTANCE * 2)
                Player.ChangeInteractionState(new IdleState(Player));
            
            if (Input.IsActionPressed("get_closer"))
            {
                MoveGrabbablePosition(-delta * 10);
            }
            if (Input.IsActionPressed("get_further"))
            {
                MoveGrabbablePosition(delta * 10);
            }
        }

        public override void ProcessInput(InputEvent inputEvent)
        {
            if (inputEvent is InputEventMouseButton iemb)
            {
                if (inputEvent.IsActionPressed("get_closer"))
                {
                    MoveGrabbablePosition(-0.1f);
                }
                else if (inputEvent.IsActionPressed("get_further"))
                {
                    MoveGrabbablePosition(0.1f);
                }
            }
        }
    }
}
