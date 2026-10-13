using Godot;
using System;

public class DiaryGUIState : GUIState
{
    GUIManager _guiManager;
    Journal _journal;
    Diary _diary;

    public DiaryGUIState()
    {
        _guiManager = Singletons.MainManager.GameManager.GUIManager;
        _journal = _guiManager.Journal;
        _diary = _journal.Diary;
    }

    public override void AtStart()
    {
        _diary.AddMissingEntries();
        _diary.Visible = true;
    }

    public override void OnResume()
    {
        _diary.Visible = true;
    }

    public override void OnPause()
    {
        _diary.Visible = false;
    }

    public override void AtEnd()
    {
        _diary.Visible = false;
    }

    public override void OnEscapePress()
    {
        _guiManager.PopState();
    }

    public void OnEntryPressed(string entryId)
    {
        _guiManager.PushState(new ReaderGUIState(entryId));
    }
}
