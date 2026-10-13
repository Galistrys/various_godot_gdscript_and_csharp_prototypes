using Godot;
using Godot.Collections;
using HorrorGame.Entities.Interactables;
using HorrorGame.GUI.HUD;
using HorrorGame.Managers;
using HorrorGame.Player;
using HorrorGame.Player.PlayerStates;
using System;

namespace HorrorGame.Entities.Interactables.Grabbable
{
    public abstract class GrabbableInteractable : RigidBody, IInteractable
    {
        [Export]
        public string InteractableId { get; private set; }

        [Export]
        public bool AllowRotation { get; private set; }

        public abstract bool BlockCamera { get; }

        public PointerType OnRaycastDetectionPointer { get { return PointerType.Grab; } }

        public override void _Ready()
        {
            if (InteractableId == null)
            {
                Logger.Log("GrabbableInteractable's InteractableId property cannot be null!", LogLevel.Error);
                MainManager.Instance.ExitGame(ExitCode.InteractableWithoutId);
                return;
            }
        }

        public void OnInteraction(PlayerController player)
        {
            player.PushState(new GrabbingPlayerState(this));
        }

        public abstract void MouseInput(float mouseX, float mouseY, bool rotate, PlayerController player);
        public abstract void Push(PlayerController player);

        public virtual void BeginInteraction()
        {
            this.GravityScale = 0;
        }

        public virtual void EndInteraction()
        {
            this.GravityScale = 1;
        }

        public virtual bool LoadFromDictionary(Dictionary<string, object> data)
        {
            try
            {
                Translation = (Vector3)GD.Str2Var(data["position"] as string);
                Rotation = (Vector3)GD.Str2Var(data["rotation"] as string);
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, LogLevel.Error);
                return false;
            }

            return true;
        }

        public virtual Dictionary<string, object> SaveToDictionary()
        {
            return new Dictionary<string, object>()
            {
                {
                    "position",
                    GD.Var2Str(this.Translation)
                },
                {
                    "rotation",
                    GD.Var2Str(this.Rotation)
                }
            };
        }

        public abstract bool UseItem(string itemId);
    }
}
