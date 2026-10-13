using Godot;
using System;

public class MainManager : Node
{
    public GameManager GameManager;
    public TerrainManager TerrainManager;

    public override void _Ready()
    {
        SINGLETONS.MainManager = this;

        OS.WindowMaximized = true;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey iek)
        {
            if (iek.Pressed && iek.Scancode == (int)KeyList.Escape)
            {
                //GetTree().Quit();
                //TerrainManager.DestroyWorld();
                new System.Threading.Thread(() => TerrainManager.DestroyWorld()).Start();
            }
        }
    }
}
