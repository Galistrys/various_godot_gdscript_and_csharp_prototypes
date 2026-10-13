using Godot;
using System;

public sealed class GameManager : Node
{
    public GameManager()
    {
        FmodEngine.Init();
    }
}
