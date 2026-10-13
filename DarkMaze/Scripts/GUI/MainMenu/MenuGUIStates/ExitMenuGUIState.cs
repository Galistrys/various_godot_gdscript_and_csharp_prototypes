using Godot;
using System;

public class ExitMenuGUIState : MenuGUIState
{
    public override void AtStart()
    {
        MainMenu.ExitConfirmationDialog.GetCancel().Text = "EXIT_DIALOG_CANCEL";
        MainMenu.ExitConfirmationDialog.GetOk().Text = "EXIT_DIALOG_CONFIRM";

        MainMenu.ExitConfirmationDialog.GetCloseButton().Visible = false;

        MainMenu.ExitConfirmationDialog.PopupCentered();
    }

    public override void OnResume()
    {
        
    }

    public override void OnPause()
    {
        
    }

    public override void AtEnd()
    {
        MainMenu.ExitConfirmationDialog.Hide();
    }

    public override void OnEscapePress()
    {
        
    }
}
