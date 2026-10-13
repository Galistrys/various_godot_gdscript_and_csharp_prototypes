using Godot;
using HorrorGame.Data;
using HorrorGame.Managers;
using System;

namespace HorrorGame.Entities.Interactables.Pickable
{
    public class NoteEntity : PickableInteractable
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
            
            // MainManager.Instance.GameManager.HUDController.Pointer.ShowMessage(TranslationServer.Translate("FOUND_NOTE_MESSAGE").Replace("<<note_name>>", TranslationServer.Translate(InteractableId)));
            GameSaveManager.CurrentGameSave.AddNote(InteractableId);

            MainManager.Instance.GameManager.OpenInventory(GUI.HUD.Inventory.View.Notes, new object[] { InteractableId });
        }
    }
}