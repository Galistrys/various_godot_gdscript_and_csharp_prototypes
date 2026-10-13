using Godot;
using HorrorGame.Managers;
using System;

namespace HorrorGame.GUI.HUD.Inventory.Views
{
    public class NotesView : Control, IInventoryView
    {
        [Export] NodePath NotesListNodePath { get; set; }

        ItemList _notesList;
        Texture _noteListTexture = ResourceLoader.Load<StreamTexture>("res://Assets/note.png");
        int _noteSelected = -1;

        VBoxContainer _notesCont;
        NoteReader _noteReader;

        public override void _Ready()
        {
            _notesList = GetNode<ItemList>(NotesListNodePath);

            _notesCont = GetNode<VBoxContainer>("NotesList");
            _noteReader = GetNode<NoteReader>("NoteReader");

            _notesList.Clear();
        }

        public void Init(object[] data)
        {
            foreach (var note in GameSaveManager.CurrentGameSave.GetNotes())
            {
                _notesList.AddItem(note, _noteListTexture);
            }
            _noteReader.Clear();

            if (data == null)
            {
                _notesCont.Visible = true;
                _noteReader.Visible = false;
            }
            else
            {
                _notesCont.Visible = false;
                OpenNote(data[0] as string);
            }

            this.Visible = true;
        }

        public void Exit()
        {
            this.Visible = false;

            _noteSelected = -1;
            _notesList.Clear();
        }

        public void Process(float delta)
        {
            if (Input.IsActionJustPressed("inventory"))
            {
                if (_noteReader.Visible)
                    CloseNote();
                MainManager.Instance.GameManager.CloseInventory();
            }
        }

        public void UiCancel()
        {
            if (_noteReader.Visible)
            {
                Logger.Log($"UI Cancel -> Close note");
                CloseNote();
            }
            else
                MainManager.Instance.GameManager.CloseInventory();
        }

        void OpenNote(string noteId)
        {
            Logger.Log("Open note: " + noteId);

            _notesCont.Visible = false;
            _noteReader.LoadNote(noteId);
            _noteReader.Visible = true;
        }

        public void CloseNote()
        {
            Logger.Log($"Close note");

            _noteReader.Visible = false;
            _noteReader.Clear();
            _notesCont.Visible = true;
        }

        void OnOpenNoteButtonPressed()
        {
            if (_noteSelected != -1)
            {
                OpenNote(GameSaveManager.CurrentGameSave.GetNote(_noteSelected));
            }
        }

        void OnNoteListItemSelected(int idx)
        {
            Logger.Log($"Note selected: {idx}");
            _noteSelected = idx;
        }

        void OnNoteListItemActivated(int idx)
        {
            Logger.Log($"Note activated: {idx}");
            _noteSelected = idx;
            OpenNote(GameSaveManager.CurrentGameSave.GetNote(_noteSelected));
        }
    }
}