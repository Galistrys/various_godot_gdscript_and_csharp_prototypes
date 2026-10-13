using Godot;
using HorrorGame.Managers;
using System;

namespace HorrorGame.GUI.PauseMenu
{
    public class PauseMenuController : Control
    {
        public CenterContainer ButtonsCont { get { return GetNode<CenterContainer>("Cont"); } }
        public ConfirmationDialog ConfirmationDialog { get { return GetNode<ConfirmationDialog>("Confirm"); } }

        public override void _Ready()
        {
            ConfirmationDialog.GetOk().Text = "YES";
            ConfirmationDialog.GetCancel().Text = "NO";
        }

        public override void _Process(float delta)
        {
            if (this.Visible)
            {
                if (Input.IsActionJustPressed("pause"))
                    OnResumeButtonPressed();
            }
        }

        public void OnResumeButtonPressed()
        {
            if (ConfirmationDialog.Visible)
                return;

            MainManager.Instance.GameManager.ClosePauseMenu();
        }

        public void OnSaveButtonPressed()
        {
            if (ConfirmationDialog.Visible)
                return;
            
            MainManager.Instance.GameManager.LevelManager.SaveLevelData();
            GameSaveManager.CurrentGameSave.SaveData();
        }

        public void OnSettingsButtonPressed()
        {
            if (ConfirmationDialog.Visible)
                return;
            

        }

        public void OnExitButtonPressed()
        {
            if (ConfirmationDialog.Visible)
                return;
            
            ConfirmationDialog.PopupCentered();
        }

        public void OnConfirmConfirmed()
        {
            MainManager.Instance.GameManager.GoToMainMenu();
        }

        public void ShowPauseMenu()
        {
            this.Visible = true;
        }

        public void ClosePauseMenu()
        {
            if (ConfirmationDialog.Visible)
                return;
            this.Visible = false;
        }
    }
}