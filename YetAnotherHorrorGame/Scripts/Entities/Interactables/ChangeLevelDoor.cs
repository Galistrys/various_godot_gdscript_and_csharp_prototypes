using Godot;
using Godot.Collections;
using HorrorGame.GUI.HUD;
using HorrorGame.Managers;
using HorrorGame.Player;
using System;

namespace HorrorGame.Entities.Interactables
{
    public class ChangeLevelDoor : StaticBody, IInteractable
    {
        [Export]
        public string InteractableId { get; set; }

        [Export]
        public string TargetLevel { get; set; }

        public PointerType OnRaycastDetectionPointer { get { return PointerType.Door; } }

        [Export]
        public bool Locked { get; private set; }

        [Export]
        string UnlockItem { get; set; }

        public override void _Ready()
        {
            if (InteractableId == null)
            {
                Logger.Log("ChangeLevelDoor's InteractableId property cannot be null!", LogLevel.Error);
                MainManager.Instance.ExitGame(ExitCode.InteractableWithoutId);
                return;
            }

            if (TargetLevel == null)
            {
                Logger.Log("ChangeLevelDoor's TargetLevel property cannot be null!", LogLevel.Error);
                MainManager.Instance.ExitGame(ExitCode.InteractableWithoutId);
                return;
            }
        }

        public void OnInteraction(PlayerEntity player)
        {
            if (Locked)
            {
                Logger.Log($"Door \"{InteractableId}\" is locked. Stop.");
                MainManager.Instance.GameManager.HUDController.Pointer.ShowMessage(TranslationServer.Translate("DOOR_LOCKED"));
            }
            else
            {
                Logger.Log($"Door \"{InteractableId}\" is not locked. Proceed.");
                MainManager.Instance.GameManager.ChangeLevel(TargetLevel);
            }
        }

        public Dictionary<string, object> SaveToDictionary()
        {
            return new Dictionary<string, object>()
            {
                {
                    "is_locked",
                    Locked
                }
            };
        }

        public bool LoadFromDictionary(Dictionary<string, object> data)
        {
            try
            {
                Locked = (bool)data["is_locked"];
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, LogLevel.Error);
                return false;
            }

            return true;
        }

        public bool UseItem(string itemId)
        {
            if (!Locked || UnlockItem == null)
                return false;

            if (UnlockItem == itemId)
            {
                Logger.Log($"Door \"{InteractableId}\" unlocked!");
                Locked = false;
                return true;
            }

            return false;
        }
    }
}