using Godot;
using System;

public abstract class MenuGUIState
{
    public static MainMenu MainMenu { get; set; }

    public abstract void AtStart();
    public abstract void OnResume();
    public abstract void OnPause();
    public abstract void AtEnd();

    public abstract void OnEscapePress();
}
