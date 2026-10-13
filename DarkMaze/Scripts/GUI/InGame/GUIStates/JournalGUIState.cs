using Godot;
using System;

public class JournalGUIState : GUIState
{
    GUIManager _guiManager;
    Journal _journal;

    HBoxContainer _menuButtons;

    public JournalGUIState()
    {
        _guiManager = Singletons.MainManager.GameManager.GUIManager;
        if (!(_guiManager.PeekState() is InventoryGUIState))
            _guiManager.PushState(new BackgroundGUIState());
        _journal = _guiManager.Journal;
        _menuButtons = _journal.GetNode<HBoxContainer>("Menu");
    }

    public override void AtStart()
    {
        _journal.Visible = true;
        _menuButtons.Visible = true;
    }

    public override void OnResume()
    {
        _menuButtons.Visible = true;
    }

    public override void OnPause()
    {
        _menuButtons.Visible = false;
    }

    public override void AtEnd()
    {
        _menuButtons.Visible = false;
        _journal.Visible = false;
    }

    public override void OnEscapePress()
    {
        _guiManager.PopState();
    }

    public void OnNotesButtonPressed()
    {
        _guiManager.PushState(new NotesGUIState());
    }

    public void OnDiaryButtonPressed()
    {
        _guiManager.PushState(new DiaryGUIState());
    }

    public void OnToDoButtonPressed()
    {
        _guiManager.PushState(new ToDoGUIState());
    }

    public void OnExitButtonPressed()
    {
        OnEscapePress();
    }
}
