using Godot;
using System;

public class ExitGUIState : GUIState
{
    GUIManager _guiManager;
    PauseMenu _pauseMenu;

    public ExitGUIState()
    {
        _guiManager = Singletons.MainManager.GameManager.GUIManager;
        _pauseMenu = _guiManager.PauseMenu;
    }

    public override void AtEnd()
    {
        _pauseMenu.ExitConfirmationDialog.Hide();
    }

    public override void AtStart()
    {
        _pauseMenu.ExitConfirmationDialog.PopupCentered();
    }

    public override void OnEscapePress()
    {
        
    }

    public override void OnPause()
    {
        
    }

    public override void OnResume()
    {
        
    }
}
