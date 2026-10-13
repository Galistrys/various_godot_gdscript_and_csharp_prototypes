using Godot;
using System;

struct SaveEntry
{
    public string Name { get; private set; }
    public string Year { get; private set; }
    public string Month { get; private set; }
    public string Day { get; private set; }
    public string Hour { get; private set; }
    public string Minute { get; private set; }
    public string Second { get; private set; }

    public SaveEntry(string name, string year, string month, string day, string hour, string minute, string second)
    {
        Name = name;
        Year = year;
        Month = month;
        Day = day;
        Hour = hour;
        Minute = minute;
        Second = second;
    }
}

class SaveComparer : System.Collections.Generic.IComparer<SaveEntry>
{
    public int Compare(SaveEntry x, SaveEntry y)
    {
        var tempX = (x.Year, x.Month, x.Day, x.Hour, x.Month, x.Second);
        var tempY = (y.Year, y.Month, y.Day, y.Hour, y.Month, y.Second);

        var tempCompare = tempX.CompareTo(tempY);

        if (tempCompare == 0)
        {
            return x.Name.CompareTo(y.Name);
        }
        else
        {
            return -tempCompare;
        }
    }
}

public class LoadGameSection : CenterContainer
{
    public ItemList SavesList { get; private set; }
    public const string _prefix = "  ";
    public const string _dateSep = " - ";
    public const string _timeSep = " - ";

    public override void _Ready()
    {
        SavesList = GetNode<ItemList>("PanelContainer/VBoxContainer/ScrollContainer/SavesList");
    }

    public void ReloadSavesList()
    {
        File file = new File();

        SavesList.Clear();

        Godot.Collections.Array<string> savesList = GameSaveManager.GetSavesArray();
        System.Collections.Generic.List<SaveEntry> tempList = new System.Collections.Generic.List<SaveEntry>();

        foreach (string saveName in savesList)
        {
            SaveEntry saveEntry;
            
            var date = OS.GetDatetimeFromUnixTime(Convert.ToInt64(file.GetModifiedTime(GameSaveManager.GAME_SAVE_DIRECTORY + "/" + saveName)));

            string year = GD.Str(date["year"]);
            string month = (int)date["month"] > 9 ? GD.Str(date["month"]) : GD.Str(0, date["month"]);
            string day = (int)date["day"] > 9 ? GD.Str(date["day"]) : GD.Str(0, date["day"]);

            string hour = (int)date["hour"] > 9 ? GD.Str(date["hour"]) : GD.Str(0, date["hour"]);
            string minute = (int)date["minute"] > 9 ? GD.Str(date["minute"]) : GD.Str(0, date["minute"]);
            string second = (int)date["second"] > 9 ? GD.Str(date["second"]) : GD.Str(0, date["second"]);

            saveEntry = new SaveEntry(saveName.Replace(GameSaveManager.GAME_SAVE_EXTENSION, ""), year, month, day, hour, minute, second);

            tempList.Add(saveEntry);
        }

        tempList.Sort(new SaveComparer());

        foreach (var tt in tempList)
        {
            string entry = _prefix;
            entry += tt.Name;
            entry += _dateSep;

            entry += tt.Year;
            entry += ".";
            entry += tt.Month;
            entry += ".";
            entry += tt.Day;

            entry += _timeSep;

            entry += tt.Hour;
            entry += ":";
            entry += tt.Minute;
            entry += ":";
            entry += tt.Second;

            SavesList.AddItem(entry);
        }
    }

    public void OnLoadButtonPressed()
    {
        var selected = SavesList.GetSelectedItems();
        if (selected.Length == 1)
        {
            string text = SavesList.GetItemText(selected[0]);
            text = text.Remove(0, _prefix.Length);
            text = text.Remove(text.Find(_dateSep));
            Singletons.MainManager.LoadGame(text);
        }
    }

    public void OnReturnButtonPressed()
    {
        MenuGUIState.MainMenu.PopState();
    }
}
