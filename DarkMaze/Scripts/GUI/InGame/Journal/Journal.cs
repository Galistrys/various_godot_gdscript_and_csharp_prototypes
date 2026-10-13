using Godot;
using System;

public class Journal : PanelContainer
{
    public HBoxContainer Menu { get; private set; }
    public Notes Notes { get; private set; }
    public Diary Diary { get; private set; }
    public ToDo ToDo { get; private set; }
    public Reader Reader { get; private set; }

    public override void _Ready()
    {
        Notes = GetNode<Notes>("Notes");
        Diary = GetNode<Diary>("Diary");
        ToDo = GetNode<ToDo>("ToDo");
        Reader = GetNode<Reader>("Reader");
    }

    public void OnNotesButtonPressed()
    {
        if (Singletons.MainManager.GameManager.GUIManager.PeekState() is JournalGUIState journalGUIState)
        {
            journalGUIState.OnNotesButtonPressed();
        }
    }

    public void OnDiaryButtonPressed()
    {
        if (Singletons.MainManager.GameManager.GUIManager.PeekState() is JournalGUIState journalGUIState)
        {
            journalGUIState.OnDiaryButtonPressed();
        }
    }

    public void OnToDoButtonPressed()
    {
        if (Singletons.MainManager.GameManager.GUIManager.PeekState() is JournalGUIState journalGUIState)
        {
            journalGUIState.OnToDoButtonPressed();
        }
    }

    public void OnExitButtonPressed()
    {
        if (Singletons.MainManager.GameManager.GUIManager.PeekState() is JournalGUIState journalGUIState)
        {
            journalGUIState.OnExitButtonPressed();
        }
    }
}
