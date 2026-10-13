using Godot;
using SpaceShooter.Data;
using System;

namespace SpaceShooter.Managers
{
    public class MainManager : Node
    {
        public static MainManager Instance { get; private set; }

        public GameConfiguration GameConfiguration { get; set; }

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

            PauseMode = PauseModeEnum.Process;
            OS.WindowMaximized = true;
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
        
        public void GoToBattleScene()
        {
            GetTree().ChangeScene("res://Resources/Scenes/Game.scn");
        }
    }
}