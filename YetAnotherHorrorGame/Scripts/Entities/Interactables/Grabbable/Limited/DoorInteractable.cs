using Godot;
using HorrorGame.Managers;
using HorrorGame.Player;
using System;

namespace HorrorGame.Entities.Interactables.Grabbable.Limited
{
    public class DoorInteractable : LimitedGrabbableInteractable
    {
        public const float CLOSE_MARGIN = 0.01f;

        [Export]
        public float RotationMaxValue { get; private set; }

        [Export]
        public float RotationMinValue { get; private set; }

        private bool _isClosed = false;
        [Export]
        public bool IsClosed
        {
            get
            {
                return _isClosed;
            }
            set
            {
                _isClosed = value;

                Logger.Log($"\"{InteractableId}\" door's IsClosed property setted to \"{value}\"");

                // if (_isClosed)
                //     EmitSignal(nameof(Closed), _onOpenEvent);
                // else
                //     EmitSignal(nameof(Opened), _onCloseEvent);
            }
        }

        public bool _isLocked = false;
        [Export]
        public bool IsLocked
        {
            get
            {
                return _isLocked;
            }
            set
            {
                _isLocked = value;

                Logger.Log($"\"{InteractableId}\" door's IsLocked property setted to \"{value}\"");

                // EmitSignal(nameof(Unlocked), _onUnlockEvent);
            }
        }

        [Export]
        string UnlockItem { get; set; }

        public override void _PhysicsProcess(float delta)
        {
            if (!IsClosed && this.RotationDegrees.y < CLOSE_MARGIN && this.Mode == RigidBody.ModeEnum.Rigid)
            {
                this.Mode = RigidBody.ModeEnum.Static;
                Vector3 temp = this.RotationDegrees;
                temp.y = 0;
                this.RotationDegrees = temp;
                IsClosed = true;
            }
        }

        public override void BeginInteraction()
        {
            Logger.Log("Begin Interaction");

            if (!IsLocked)
            {
                this.Mode = RigidBody.ModeEnum.Rigid;
                IsClosed = false;
                SetPhysicsProcess(false);
                // EmitSignal(nameof(HandlePressed), _onHandlePressEvent);
            }
            else
            {
                IsClosed = true;
                MainManager.Instance.GameManager.HUDController.Pointer.ShowMessage(TranslationServer.Translate("DOOR_LOCKED"));
                Logger.Log("Door is locked!");
                // EmitSignal(nameof(HandlePressed), _onDoorLockedEvent);
            }
        }

        public override void EndInteraction()
        {
            Logger.Log("Ended Interaction");

            SetPhysicsProcess(true);
            if (!IsLocked)
            {
                // EmitSignal(nameof(HandleReleased), _onHandleReleaseEvent);
            }
            else
            {
                // EmitSignal(nameof(HandleReleased), _onDoorLockedEvent);
            }
        }

        public override void LimitRotation()
        {
            Vector3 tempRot = this.RotationDegrees;
            Vector3 tempAng = this.AngularVelocity;

            if (tempRot.y < RotationMinValue)
            {
                tempRot.y = RotationMinValue;
                this.RotationDegrees = tempRot;
                tempAng.y = 0;
                this.AngularVelocity = tempAng;
            }

            if (tempRot.y > RotationMaxValue)
            {
                tempRot.y = RotationMaxValue;
                this.RotationDegrees = tempRot;
                tempAng.y = 0;
                this.AngularVelocity = tempAng;
            }
        }

        public override void LimitTranslation()
        {
            
        }

        public override void MouseInput(float mouseX, float mouseY, bool rotate, PlayerEntity player)
        {
            if (!IsLocked)
            {
                Vector3 playerLocal = (GetChild(0) as Spatial).ToLocal(player.GlobalTransform.origin);
                bool IsPlayerAtFront = playerLocal.z > 0;

                float globalRot = (GetChild(0) as Spatial).GlobalTransform.basis.GetEuler().y;
                float playerRot = player.Rotation.y + Mathf.Pi / 2.0f;

                Vector2 tempGlo = new Vector2(Mathf.Cos(globalRot), Mathf.Sin(globalRot));
                Vector2 tempCal = new Vector2(Mathf.Cos(playerRot), Mathf.Sin(playerRot));

                float angle = tempGlo.AngleTo(tempCal);

                float finalAngleX = -mouseX * MOUSE_SENSITIVITY * Mathf.Cos(angle);
                float finalAngleY = -mouseY * MOUSE_SENSITIVITY * Mathf.Sin(angle);

                ApplyTorqueImpulse(Vector3.Up * finalAngleX);
                ApplyTorqueImpulse(Vector3.Up * finalAngleY);
            }
        }

        public override void Push(PlayerEntity player)
        {
            if (!IsLocked)
            {
                Vector3 playerLocal = (GetChild(0) as Spatial).ToLocal(player.GlobalTransform.origin);
                bool IsPlayerAtFront = playerLocal.z > 0;

                float globalRot = (GetChild(0) as Spatial).GlobalTransform.basis.GetEuler().y;
                float playerRot = player.Rotation.y + Mathf.Pi / 2.0f;

                Vector2 tempGlo = new Vector2(Mathf.Cos(globalRot), Mathf.Sin(globalRot));
                Vector2 tempCal = new Vector2(Mathf.Cos(playerRot), Mathf.Sin(playerRot));

                float angle = tempGlo.AngleTo(tempCal);

                float finalAngleY = this.Weight * MOUSE_SENSITIVITY * Mathf.Sin(angle);

                ApplyTorqueImpulse(Vector3.Up * finalAngleY);
            }
        }

        public override bool UseItem(string itemId)
        {
            if (!IsLocked || UnlockItem == null)
                return false;

            if (UnlockItem == itemId)
            {
                IsLocked = false;
                return true;
            }

            return false;
        }
    }
}