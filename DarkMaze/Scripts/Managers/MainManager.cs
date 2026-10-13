using Godot;
using System;

public class MainManager : Node, ISingleton
{
    private GameManager _gameManager = null;
    public GameManager GameManager
    {
        get
        {
            if (_gameManager == null)
            {
                _gameManager = GetTree().Root.GetNodeOrNull<GameManager>("GAME");
            }
            return _gameManager;
        }
    }

    public string ProfileName { get; private set; } = "null";

    public override void _Ready()
    {
        if (!GameSaveManager.CreateFolders())
        {
            OS.Alert("Cannot create save folder.");
            GetTree().Quit(3);
        }
    }

    public void ResetSingleton()
    {
        _gameManager = null;
        ProfileName = null;
    }

    public void LoadGame(string profileName)
    {
        GD.Print("Loading game: '", profileName, "'");
        
        ProfileName = profileName;
        GetTree().ChangeScene("res://Resources/Scenes/GAME.scn");
        GetTree().Paused = true;
    }
}
