using Godot;
using Godot.Collections;
using HorrorGame.Managers;
using System;

namespace HorrorGame.GUI.MainMenu.Views
{
    public class LoadGameView : MainMenuView
    {
        public const string _prefix = "  ";
        public const string _dateSep = " - ";
        public const string _timeSep = " - ";
        
        ItemList SavesList { get { return GetNode<ItemList>("Cont/Rows/ScrollContainer/Saves"); } }

        public override void _Ready()
        {
            ReloadSavesList();
        }

        public void ReloadSavesList()
        {
            File file = new File();

            SavesList.Clear();

            Array<string> savesList = GameSaveManager.GetSaves();
            System.Collections.Generic.List<string> tempSaveEntriesList = new System.Collections.Generic.List<string>();

            foreach (string saveName in savesList)
            {
                long hoursToAdd = (Convert.ToInt64((int)OS.GetDatetime()["hour"]) - Convert.ToInt64((int)OS.GetDatetime(true)["hour"])) * 3600L;
                var date = OS.GetDatetimeFromUnixTime(hoursToAdd + Convert.ToInt64(file.GetModifiedTime(GameSaveManager.SAVE_DIR + "/" + saveName + GameSaveManager.SAVE_FILE_EXT)));
                
                string year = GD.Str(date["year"]);
                string month = (int)date["month"] > 9 ? GD.Str(date["month"]) : GD.Str(0, date["month"]);
                string day = (int)date["day"] > 9 ? GD.Str(date["day"]) : GD.Str(0, date["day"]);

                string hour = (int)date["hour"] > 9 ? GD.Str(date["hour"]) : GD.Str(0, date["hour"]);
                string minute = (int)date["minute"] > 9 ? GD.Str(date["minute"]) : GD.Str(0, date["minute"]);
                string second = (int)date["second"] > 9 ? GD.Str(date["second"]) : GD.Str(0, date["second"]);

                tempSaveEntriesList.Add($"{year}.{month}.{day}{_timeSep}{hour}:{minute}:{second}|{saveName}");
            }

            tempSaveEntriesList.Sort();
            tempSaveEntriesList.Reverse();

            foreach (var saveEntry in tempSaveEntriesList)
            {
                SavesList.AddItem(SaveEntryToString(saveEntry), ResourceLoader.Load<Texture>("res://icon.png"));
            }
        }

        string SaveEntryToString(string se)
        {
            var split = se.Split("|");
            return $"{_prefix}{split[1]}{_dateSep}{split[0]}";
        }

        void OnLoadButtonPressed()
        {
            var selected = SavesList.GetSelectedItems();
            if (selected.Length == 1)
            {
                string text = SavesList.GetItemText(selected[0]);
                text = text.Remove(0, _prefix.Length);
                text = text.Remove(text.Find(_dateSep));
                MainMenu.GoToGame(text);
            }
            else
            {
                Logger.Log("Save is not selected.", LogLevel.Warning);
            }
        }

        void OnReturnButtonPressed()
        {
            MainMenu.ChangeView("MainView");
        }
    }
}