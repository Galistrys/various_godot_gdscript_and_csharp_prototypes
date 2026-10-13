using Godot;
using HorrorGame.Managers;
using System;

namespace HorrorGame.GUI.MainMenu.Views
{
    public class MainView : MainMenuView
    {
        void OnNewGameButtonPressed()
        {
            MainMenu.ChangeView("NewGameView");
        }

        void OnLoadGameButtonPressed()
        {
            MainMenu.ChangeView("LoadGameView");
        }

        void OnSettingsButtonPressed()
        {

        }

        void OnAboutButtonPressed()
        {

        }

        void OnExitButtonPressed()
        {
            MainManager.Instance.ExitGame(ExitCode.Ok);
        }
    }
}