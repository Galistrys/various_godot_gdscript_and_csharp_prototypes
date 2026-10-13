using Godot;
using System;

public class CrosshairGUIState : GUIState
{
    public override void AtStart()
    {
        Singletons.MainManager.GameManager.GUIManager.HUD.Visible = true;
    }

    public override void OnResume()
    {
        Singletons.MainManager.GameManager.GUIManager.HUD.Visible = true;
    }

    public override void OnPause()
    {
        Singletons.MainManager.GameManager.GUIManager.HUD.Visible = false;
    }

    public override void AtEnd()
    {
        Singletons.MainManager.GameManager.GUIManager.HUD.Visible = false;
    }

    public override void OnEscapePress()
    {
        // Pause game
    }
}
