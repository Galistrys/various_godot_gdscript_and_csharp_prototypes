using Godot;
using System;

namespace HorrorGame.GUI.MainMenu.Views
{
    public abstract class MainMenuView : Control
    {
        public MainMenuController MainMenu { get { return GetNode<MainMenuController>("../.."); } }
    }
}