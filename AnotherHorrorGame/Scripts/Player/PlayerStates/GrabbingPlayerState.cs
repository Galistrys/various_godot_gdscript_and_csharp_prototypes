using Godot;
using HorrorGame.Entities.Interactables.Grabbable;
using System;

namespace HorrorGame.Player.PlayerStates
{
    public class GrabbingPlayerState : PlayerState
    {
        GrabbableInteractable _grabbableInteractable;
        public Vector3 InteractionPoint { get; private set; }

        float _delta = 0;
        
        public GrabbingPlayerState(GrabbableInteractable grabbableInteractable)
        {
            _grabbableInteractable = grabbableInteractable;

            float zVal = Mathf.Clamp(-(PlayerController.PlayerCamera.GlobalTransform.origin.DistanceTo(_grabbableInteractable.GlobalTransform.origin)), -PlayerController.RAY_LENGTH, -PlayerController.MIN_DISTANCE);
            PlayerController.MovableGrabPosition.Translation = new Vector3(0, 0, zVal);
        }

        public override void PhysicsUpdateLoop(float delta)
        {
            if (PlayerController.GlobalTransform.origin.DistanceTo(_grabbableInteractable.GlobalTransform.origin) > PlayerController.MAX_DISTANCE)
                PlayerController.PopState();
        }

        public override void UpdateLoop(float delta)
        {
            _delta = delta;

            PlayerController.SetMovementDirection(GetDir());
            if (Input.IsActionJustPressed("jump"))
            {
                PlayerController.Jump();
            }

            if (Input.IsActionPressed("bring_further"))
                BringFurther(delta);
            if (Input.IsActionPressed("bring_closer"))
                BringCloser(delta);

            if (_grabbableInteractable.AllowRotation && Input.IsActionPressed("rotate"))
            {
                _grabbableInteractable.MouseInput(PlayerController.viewChange.x * delta, PlayerController.viewChange.y * delta, true, PlayerController);
                PlayerController.viewChange = Vector2.Zero;
            }
            else
            {
                _grabbableInteractable.MouseInput(PlayerController.viewChange.x * delta, PlayerController.viewChange.y * delta, false, PlayerController);
                if (!_grabbableInteractable.BlockCamera)
                    PlayerController.UpdateCameraRotation(delta);
                else
                    PlayerController.viewChange = Vector2.Zero;
            }

             if (Input.IsActionJustReleased("interact"))
            {
                PlayerController.PopState();
            }

            if (Input.IsActionJustPressed("aim"))
            {
                _grabbableInteractable.Push(PlayerController);
                PlayerController.PopState();
            }
        }

        public override void HandleInput(ref InputEvent inputEvent)
        {
            if (inputEvent is InputEventMouseButton iemb)
            {
                if (iemb.ButtonIndex == (int)ButtonList.WheelDown)
                {
                    BringCloser(_delta);
                }
                else if (iemb.ButtonIndex == (int)ButtonList.WheelUp)
                {
                    BringFurther(_delta);
                }
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _grabbableInteractable.BeginInteraction();

            PlayerController.ColShape.Disabled = false;
            PlayerController.CrouchColShape.Disabled = true;
        }

        public override void OnExit()
        {
            base.OnExit();

            _grabbableInteractable.EndInteraction();

            _grabbableInteractable = null;
        }

        void BringCloser(float delta)
        {
            Vector3 temp = new Vector3(0, 0, Mathf.Clamp(PlayerController.MovableGrabPosition.Translation.z + delta, -PlayerController.RAY_LENGTH, -PlayerController.MIN_DISTANCE));
            PlayerController.MovableGrabPosition.Translation = temp;
        }

        void BringFurther(float delta)
        {
            Vector3 temp = new Vector3(0, 0, Mathf.Clamp(PlayerController.MovableGrabPosition.Translation.z - delta, -PlayerController.RAY_LENGTH, -PlayerController.MIN_DISTANCE));
            PlayerController.MovableGrabPosition.Translation = temp;
        }
    }
}