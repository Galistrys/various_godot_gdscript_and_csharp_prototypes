using Godot;
using Godot.Collections;
using HorrorGame.Data;
using HorrorGame.GUI.HUD;
using HorrorGame.Managers;
using HorrorGame.Player;
using HorrorGame.Tools;
using System;

namespace HorrorGame.Entities.Interactables.Pickable
{
    public abstract class PickableInteractable : RigidBody, IInteractable
    {
        public abstract string InteractableId { get; set; }

        public PointerType OnRaycastDetectionPointer { get { return PointerType.Pick; } }

        public override void _Ready()
        {
            if (InteractableId == null)
            {
                Logger.Log("PickableInteractable's InteractableId property cannot be null!", LogLevel.Error);
                MainManager.Instance.ExitGame(ExitCode.InteractableWithoutId);
                return;
            }
        }

        public void OnInteraction(PlayerController player)
        {
            this.QueueFree();
            Logger.Log($"PickableInteractable of type {this.GetType().Name} picked up. Id: {InteractableId}");

            OnPickUp();
        }
        
        protected virtual void OnPickUp()
        {
            string name = LevelManager.ConvertNodePath(this.GetPath());
            if (!GameSaveManager.SaveEntityPathExists(GameSaveManager.CurrentGameSave.CurrentMapId, name))
            {
                GameSaveManager.CreateSaveEntityPath(GameSaveManager.CurrentGameSave.CurrentMapId, name);
            }

            GameSaveManager.CurrentGameSave.GetLevelData(GameSaveManager.CurrentGameSave.CurrentMapId)[name] = new Dictionary<string, object>() { { "picked_up", true } };
        }

        public Dictionary<string, object> SaveToDictionary()
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

        public bool LoadFromDictionary(Dictionary<string, object> data)
        {
            try
            {
                if (data.ContainsKey("picked_up"))
                {
                    QueueFree();
                }
                else
                {
                    Translation = (Vector3)GD.Str2Var(data["position"] as string);
                    Rotation = (Vector3)GD.Str2Var(data["rotation"] as string);
                }
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
            return false;
        }
    }
}