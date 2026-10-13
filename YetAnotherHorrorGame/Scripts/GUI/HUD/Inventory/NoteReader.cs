using Godot;
using HorrorGame.Data;
using HorrorGame.GUI.HUD.Inventory.Views;
using System;

namespace HorrorGame.GUI.HUD.Inventory
{
    public class NoteReader : VBoxContainer
    {
        RichTextLabel _content;
        Label _title;

        public override void _Ready()
        {
            base._Ready();

            _title = GetNode<Label>("NoteTitle");
            _content = GetNode<RichTextLabel>("MarginContainer/NoteContent");
        }

        public void Clear()
        {
            Logger.Log("Clear note");
            
            _title.Text = "";
            _content.Clear();
        }

        public void LoadNote(string noteId)
        {
            Logger.Log("Loading note: " + noteId);

            _title.Text = noteId;
            _content.BbcodeText = TranslationServer.Translate(Database.Instance.GetNote(noteId)).Replace("<<new_line>>", "\n");
        }

        void OnReturnButtonPressed()
        {
            if (Visible)
            {
                GetParent<NotesView>().CloseNote();
            }
        }
    }
}