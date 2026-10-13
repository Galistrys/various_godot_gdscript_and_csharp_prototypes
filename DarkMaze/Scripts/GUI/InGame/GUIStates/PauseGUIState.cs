using Godot;
using System;

public class PauseGUIState : GUIState
{
    GUIManager _guiManager;
    PauseMenu _pauseMenu;

    bool _flag = false;

    public PauseGUIState()
    {
        _guiManager = Singletons.MainManager.GameManager.GUIManager;
        _pauseMenu = _guiManager.PauseMenu;
        _guiManager.PushState(new BackgroundGUIState());
    }

    public override void AtStart()
    {
        _pauseMenu.Visible = true;
    }

    public override void OnResume()
    {
        _pauseMenu.Visible = true;
    }

    public override void OnPause()
    {
        _pauseMenu.Visible = false;
    }

    public override void AtEnd()
    {
        _pauseMenu.Visible = false;
    }

    public override void OnEscapePress()
    {
        if (_flag)
            _guiManager.PopState();
        else
            _flag = true;
    }
}
