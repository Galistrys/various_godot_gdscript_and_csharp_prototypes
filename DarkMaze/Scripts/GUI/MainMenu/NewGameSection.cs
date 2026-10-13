using Godot;
using System;
using System.Collections.Generic;

public class NewGameSection : CenterContainer
{
    public LineEdit ProfileNameLineEdit { get; private set; }
    readonly List<char> _allowedCharacters = new List<char>()
    {
        'Q',
        'W',
        'E',
        'R',
        'T',
        'Y',
        'U',
        'I',
        'O',
        'P',
        'A',
        'S',
        'D',
        'F',
        'G',
        'H',
        'J',
        'K',
        'L',
        'Z',
        'X',
        'C',
        'V',
        'B',
        'N',
        'M',
        '1',
        '2',
        '3',
        '4',
        '5',
        '6',
        '7',
        '8',
        '9',
        '0',
        '_'
    };

    public override void _Ready()
    {
        ProfileNameLineEdit = GetNode<LineEdit>("PanelContainer/VBoxContainer/LineEdit");
    }

    public void OnCreateButtonPressed()
    {
        string checkString = ProfileNameLineEdit.Text.ToUpper();
        bool found = false;
        for (int i = 0; i < checkString.Length && !found; i++)
        {
            if (!_allowedCharacters.Contains(checkString[i]))
                found = true;
        }

        if (found)
        {
            Logger.Log("Profile Name can only contain letters, numbers and '_' character.", LogLevel.Error);
            OS.Alert("Profile Name can only contain letters, numbers and '_' character.");
        }
        else
        {
            Logger.Log("Profile name is correct.", LogLevel.Info);

            if (GameSaveManager.GameSaveExists(ProfileNameLineEdit.Text))
            {
                Logger.Log("Profile \"" + ProfileNameLineEdit.Text + "\" already exists.", LogLevel.Error);
                OS.Alert("That profile already exists.");
            }
            else
            {
                if (GameSaveManager.CreateSaveFile(ProfileNameLineEdit.Text))
                {
                    Singletons.MainManager.LoadGame(ProfileNameLineEdit.Text);
                }
                else
                {
                    Logger.Log("Could not create \"" + ProfileNameLineEdit.Text + "\" save file.", LogLevel.Error);
                    OS.Alert("Could not create save file.");
                }
            }
        }
    }

    public void OnCancelButtonPressed()
    {
        MenuGUIState.MainMenu.PopState();
    }
}
