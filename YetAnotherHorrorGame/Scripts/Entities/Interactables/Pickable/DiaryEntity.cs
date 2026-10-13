using Godot;
using HorrorGame.Data;
using HorrorGame.Managers;
using System;

namespace HorrorGame.Entities.Interactables.Pickable
{
    public class DiaryEntity : PickableInteractable
    {
        [Export]
        public override string InteractableId { get; set; }

        public override void _Ready()
        {
            base._Ready();
        }

        protected override void OnPickUp()
        {
            base.OnPickUp();
            
            MainManager.Instance.GameManager.HUDController.Pointer.ShowMessage(
                TranslationServer.Translate("FOUND_DIARY_MESSAGE").Replace(
                    "<<diary_name>>", TranslationServer.Translate(
                        Database.Instance.GetDiary(InteractableId).Title
                    )
                )
            );
            GameSaveManager.CurrentGameSave.AddDiary(InteractableId);

            // MainManager.Instance.GameManager.OpenInventory(GUI.HUD.Inventory.View.Diaries, new object[] { InteractableId });
        }
    }
}