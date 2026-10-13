using Godot;
using HorrorGame.Managers;
using System;

namespace HorrorGame.GUI.MainMenu.Views
{
    public class NewGameView : MainMenuView
    {
        LineEdit SaveNameLineEdit { get { return GetNode<LineEdit>("Cont/Rows/SaveNameLineEdit"); } }
        readonly System.Collections.Generic.List<char> _allowedCharacters = new System.Collections.Generic.List<char>()
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
            
        }

        void OnCreateButtonPressed()
        {
            string checkString = SaveNameLineEdit.Text.ToUpper();
            bool found = false;
            for (int i = 0; i < checkString.Length && !found; i++)
            {
                if (!_allowedCharacters.Contains(checkString[i]))
                    found = true;
            }

            if (found)
            {
                Logger.Log("Save name can only contain letters, numbers and '_' character.", LogLevel.Warning);
                OS.Alert("Save Name can only contain letters, numbers and '_' character.");
                return;
            }

            if (GameSaveManager.CreateSave(SaveNameLineEdit.Text))
            {
                Logger.Log("Created new save file successfully.", LogLevel.Info);
                
                MainMenu.GoToGame(SaveNameLineEdit.Text);
            }
            else
            {
                Logger.Log("Failed to create save file.", LogLevel.Error);
                OS.Alert("Failed to create save file.", "ERROR");
            }
        }

        void OnReturnButtonPressed()
        {
            MainMenu.ChangeView("MainView");
        }
    }
}