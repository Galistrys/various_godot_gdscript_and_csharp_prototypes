using Godot;
using System;

public class NotesGUIState : GUIState
{
    GUIManager _guiManager;
    Journal _journal;
    Notes _notes;

    public NotesGUIState()
    {
        _guiManager = Singletons.MainManager.GameManager.GUIManager;
        _journal = _guiManager.Journal;
        _notes = _journal.Notes;
    }

    public override void AtStart()
    {
        _notes.AddMissingNotes();
        _notes.Visible = true;
    }

    public override void OnResume()
    {
        _notes.Visible = true;
    }

    public override void OnPause()
    {
        _notes.Visible = false;
    }

    public override void AtEnd()
    {
        _notes.Visible = false;
    }

    public override void OnEscapePress()
    {
        _guiManager.PopState();
    }

    public void OnNotePressed(string noteId)
    {
        _guiManager.PushState(new ReaderGUIState(noteId));
    }
}
