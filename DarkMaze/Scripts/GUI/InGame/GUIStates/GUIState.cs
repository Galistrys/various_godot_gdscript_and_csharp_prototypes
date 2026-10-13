using Godot;
using System;

public abstract class GUIState
{
    public abstract void AtStart();
    public abstract void OnResume();
    public abstract void OnPause();
    public abstract void AtEnd();

    public virtual void _Process(float delta) {}
    public abstract void OnEscapePress();
    public virtual void OnInteractionEvent(bool pressed) {}
    public virtual void OnPushEvent(bool pressed) {}
}
