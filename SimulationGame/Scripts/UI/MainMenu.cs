using Godot;
using System;

public class MainMenu : Control
{
    public override void _Ready()
    {
        GetNode<Button>("CentCont/VBoxCont/PlayButton").Connect("pressed", this, "OnPlayButtonPressed");
    }

    public void OnPlayButtonPressed()
    {
        GetTree().ChangeScene("res://Resources/Scenes/GAME.scn");
    }
}
