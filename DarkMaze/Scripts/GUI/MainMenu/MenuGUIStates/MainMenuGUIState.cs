using Godot;
using System;

public class MainMenuGUIState : MenuGUIState
{
    public override void AtStart()
    {
        MainMenu.MainSection.Visible = true;
    }

    public override void OnResume()
    {
        MainMenu.MainSection.Visible = true;
    }

    public override void OnPause()
    {
        MainMenu.MainSection.Visible = false;
    }

    public override void AtEnd()
    {
        MainMenu.MainSection.Visible = false;
    }

    public override void OnEscapePress()
    {
        
    }
}
