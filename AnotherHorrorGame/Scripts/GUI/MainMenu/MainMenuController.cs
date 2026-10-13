using Godot;
using HorrorGame.GUI.MainMenu.Views;
using HorrorGame.Managers;
using System;

namespace HorrorGame.GUI.MainMenu
{
    public class MainMenuController : Node
    {
        public const string VIEWS_DIR = "res://Resources/GUI/MainMenuViews";
        public const string VIEWS_EXT = ".res";

        Control Handle { get { return GetNode<Control>("Handle"); } }
        MainMenuView CurrentView
        {
            get
            {
                if (Handle.GetChildCount() == 0)
                    return null;
                return Handle.GetChildOrNull<MainMenuView>(0);
            }
        }

        AnimationPlayer AnimPlayer { get { return GetNode<AnimationPlayer>("AnimationPlayer"); } }

        public override void _Ready()
        {
            OS.WindowMaximized = true;
            ChangeView("MainView");
        }

        public void ChangeView(string viewName)
        {
            Logger.Log($"Changing view to {viewName}", LogLevel.Info);

            string path = VIEWS_DIR + "/" + viewName + VIEWS_EXT;

            File file = new File();
            if (!file.FileExists(path))
            {
                Logger.Log($"View not found at: {path}", LogLevel.Error);
                return;
            }

            MainMenuView current = CurrentView;
            if (current != null)
            {
                current.QueueFree();
            }
            current = null;

            PackedScene viewScene = ResourceLoader.Load<PackedScene>(VIEWS_DIR + "/" + viewName + VIEWS_EXT);
            Handle.AddChild(viewScene.Instance());
        }

        public async void GoToGame(string saveName)
        {
            AnimPlayer.Play("FadeOut");
            await ToSignal(AnimPlayer, "animation_finished");

            MainManager.Instance.CallDeferred(nameof(MainManager.GoToGame), saveName);
        }
    }
}