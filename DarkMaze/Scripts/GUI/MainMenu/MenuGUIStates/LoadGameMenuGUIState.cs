using Godot;
using System;

public class LoadGameMenuGUIState : MenuGUIState
{
    LoadGameSection _loadGameSection;

    public LoadGameMenuGUIState()
    {
        _loadGameSection = MainMenu.LoadGameSection;
    }

    public override void AtStart()
    {
        _loadGameSection.ReloadSavesList();
        _loadGameSection.Visible = true;
    }

    public override void OnResume()
    {
        _loadGameSection.Visible = true;
    }

    public override void OnPause()
    {
        _loadGameSection.Visible = false;
    }

    public override void AtEnd()
    {
        _loadGameSection.Visible = false;
    }

    public override void OnEscapePress()
    {
        MainMenu.PopState();
    }
}
