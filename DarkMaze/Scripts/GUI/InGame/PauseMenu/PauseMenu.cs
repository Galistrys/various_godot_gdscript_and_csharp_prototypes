using Godot;
using System;

public class PauseMenu : PanelContainer
{
    GUIManager _guiManager;

    [Export]
    public NodePath _exitDialogPath;
    public ConfirmationDialog ExitConfirmationDialog { get; private set; }

    public override void _Ready()
    {
        _guiManager = Singletons.MainManager.GameManager.GUIManager;
        ExitConfirmationDialog = GetNode<ConfirmationDialog>(_exitDialogPath);

        ExitConfirmationDialog.GetCancel().Text = "EXIT_DIALOG_CANCEL";
        ExitConfirmationDialog.GetOk().Text = "EXIT_DIALOG_CONFIRM";

        ExitConfirmationDialog.GetCloseButton().Visible = false;
    }

    public void OnExitConfirmationDialogConfirmed()
	{
        GetTree().Paused = false;
		GetTree().ChangeScene("res://Resources/Scenes/MainMenu.scn");
	}

	public void OnExitConfirmationDialogPopupHide()
	{
		_guiManager.PopState();
	}

    public void OnResumeButtonPressed()
    {
        _guiManager.PopState();
    }

    public void OnSaveButtonPressed()
    {
        Singletons.MainManager.GameManager.SaveGame();
    }

    public void OnExitButtonPressed()
    {
        _guiManager.PushState(new ExitGUIState());
    }
}
