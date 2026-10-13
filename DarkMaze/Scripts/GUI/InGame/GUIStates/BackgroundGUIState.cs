using Godot;
using System;

public class BackgroundGUIState : GUIState
{
    public override void AtStart()
    {
        // Singletons.MainManager.GameManager.Player.SetPhysicsProcess(false);
        Logger.Log("Pausing SceneTree!", LogLevel.Info);
        Singletons.MainManager.GetTree().Paused = true;

        Singletons.MainManager.GameManager.GUIManager.UIControl.Visible = true;
        (Singletons.MainManager.GameManager.GUIManager.GetParent() as GUI).BackgroundFadeIn();
        Input.SetMouseMode(Input.MouseMode.Visible);
    }

    public override void OnResume()
    {
        Singletons.MainManager.GameManager.GUIManager.PopState();
    }

    public override void OnPause()
    {
        
    }

    public override void AtEnd()
    {
        Singletons.MainManager.GameManager.GUIManager.UIControl.Visible = false;
        (Singletons.MainManager.GameManager.GUIManager.GetParent() as GUI).BackgroundFadeOut();
        Input.SetMouseMode(Input.MouseMode.Captured);
        
        // Singletons.MainManager.GameManager.Player.SetPhysicsProcess(true);
        Logger.Log("Unpausing SceneTree!", LogLevel.Info);
        Singletons.MainManager.GetTree().Paused = false;
    }

    public override void OnEscapePress()
    {
        
    }
}
