using Godot;
using System;

public class Notes : PanelContainer
{
    public VBoxContainer NotesList { get; private set; }

    public override void _Ready()
    {
        NotesList = GetNode<VBoxContainer>("ScrollCont/Columns/NotesList");
    }

    public void AddNote(string noteId)
    {
        Logger.Log("Adding note: " + noteId);

        Button button = new Button();
        button.Name = noteId;
        button.Text = Database.GetItemData(noteId).ItemName;
        
        NotesList.AddChild(button);
        button.Connect("pressed", this, nameof(NotePressed), new Godot.Collections.Array { noteId });
    }

    public void AddMissingNotes()
    {
        Logger.Log("Adding Missing Notes");

        var foundNotes = Singletons.MainManager.GameManager.GetCollectedNotes();

        foreach (string noteId in foundNotes)
        {
            bool found = false;
            foreach (Button btn in NotesList.GetChildren())
            {
                if (btn.Name == noteId)
                {
                    found = true;
                }
            }

            if (!found)
                AddNote(noteId);
        }
    }

    public void NotePressed(string noteId)
    {
        if (Singletons.MainManager.GameManager.GUIManager.PeekState() is NotesGUIState notesGUIState)
        {
            notesGUIState.OnNotePressed(noteId);
        }
    }
}
