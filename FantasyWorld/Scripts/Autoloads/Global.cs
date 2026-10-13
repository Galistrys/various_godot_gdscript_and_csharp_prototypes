using Godot;
using System;

public sealed class Global : Node
{
    public static Logger Logger { get; private set; }
    public static GameManager GameManager { get; private set; }

    public override void _Ready()
    {
        GameManager = GetNode<GameManager>("/root/GameManager");
    }
}
