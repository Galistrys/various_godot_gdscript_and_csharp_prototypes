using Godot;
using System;

namespace HorrorGame.Managers
{
    public class MainManager : Node
    {
        private GameSaveManager _gameSaveManager;

        public static MainManager Instance { get; private set; }

        public GameManager GameManager
        {
            get
            {
                return GetNodeOrNull<GameManager>("../GameManager");
            }
        }
        
        public MainManager()
        {
            if (Engine.EditorHint)
                return;

            if (Instance != null)
            {
                ExitGame(ExitCode.AutoloadDuplication);
                return;
            }

            Instance = this;
        }

        public override void _Ready()
        {
            if (Engine.EditorHint)
                return;
            
            if (!HorrorGame.Data.Database.Instance.LoadData())
            {
                ExitGame(ExitCode.LoadDataFailed);
                return;
            }

            _gameSaveManager = GetNode<GameSaveManager>("../GameSaveManager");
        }

        public void ExitGame(ExitCode code)
        {
            Logger.Log($"ExitGame. Code: {(int)code}, Id: {code.ToString()}", code == ExitCode.Ok ? LogLevel.Info : LogLevel.Error);
            GetTree().Quit((int)code);
        }

        public override void _Notification(int what)
        {
            if (what == MainLoop.NotificationWmQuitRequest)
            {
                ExitGame(ExitCode.SystemRequest);
                return;
            }
        }

        /// <summary>
        /// Go to Game scene
        /// </summary>
        /// <param name="saveName">Save file name</param>
        public void GoToGame(string saveName)
        {
            if (!_gameSaveManager.LoadGame(saveName))
            {
                Logger.Log("Failed to load save file.", LogLevel.Error);
                return;
            }

            Error err = GetTree().ChangeScene("res://Resources/Scenes/GameManager.scn");
            if (err != Error.Ok)
            {
                Logger.Log("Cannot change to game scene: " + err.ToString(), LogLevel.Error);
                return;
            }
        }
    }
}