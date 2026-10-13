using Godot;
using System;

public class NewGameMenuGUIState : MenuGUIState
{
    NewGameSection _newGameSection;

    public NewGameMenuGUIState()
    {
        _newGameSection = MainMenu.NewGameSection;
    }

    public override void AtStart()
    {
        _newGameSection.Visible = true;
    }

    public override void OnResume()
    {
        _newGameSection.Visible = true;
    }

    public override void OnPause()
    {
        _newGameSection.Visible = false;
    }

    public override void AtEnd()
    {
        _newGameSection.Visible = false;
    }

    public override void OnEscapePress()
    {
        MainMenu.PopState();
    }
}
