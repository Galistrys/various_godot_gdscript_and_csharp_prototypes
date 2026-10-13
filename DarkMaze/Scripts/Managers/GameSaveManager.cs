using Godot;
using Godot.Collections;
using System;

public class GameSaveManager : Node
{
    public const string DEFAULT_SAVE_GAME_PATH = "res://Resources/Data/DefaultGameSave.json";
    public const string DEFAULT_MAP_DATA_JSON_FILE = "res://Resources/Data/DefaultMapData.json";
    public const string GAME_SAVE_DIRECTORY = "user://Saves";
    public const string GAME_SAVE_EXTENSION = ".save";

    public const string GAME_SAVE_NOTES_KEY = "notes";
    public const string GAME_SAVE_OCCURRED_EVENTS_KEY = "occurred_events";
    public const string GAME_SAVE_QUESTS_KEY = "quests";
    public const string GAME_SAVE_COMPLETED_KEY = "completed";
    public const string GAME_SAVE_IN_PROGRESS_KEY = "in_progress";
    public const string GAME_SAVE_DIARIES_KEY = "diaries";
    public const string GAME_SAVE_MAPS_DATA_KEY = "maps_data";
    public const string GAME_SAVE_CURRENT_MAP_ID_KEY = "current_map_id";

    public const string GAME_SAVE_ITEMS_KEY = "items";

    public Dictionary<string, object> _defaultMapData;

    string _saveFilePath = null;

    private Dictionary _gameSaveRaw = null;

    Godot.Collections.Array<string> _collectedNotesArray;
    System.Collections.Generic.List<string> _collectedEntiresList;
    Godot.Collections.Array<string> _occurredEventsArray;
    Godot.Collections.Array<string> _mainQuestsCompletedArray;
    Dictionary<string, int> _mainQuestsInProgressDictionary;
    Dictionary<string, Dictionary<string, object>> _mapsData;
    Godot.Collections.Array<Dictionary> _items;
    Vector3 _playerPosition;
    Vector3 _playerRotation;

    public string CurrentMapId { get; set; } = null;

    public GameSaveManager()
    {
        
    }

    public GameSaveManager(string id)
    {
        Logger.Log("Loading save with id: " + id, LogLevel.Info);

        _saveFilePath = GAME_SAVE_DIRECTORY + "/" + id + GAME_SAVE_EXTENSION;

        RegisterMaps();
        LoadSaveFile();
    }

    public static bool CreateFolders()
    {
        Directory dir = new Directory();
        if (!dir.DirExists(GameSaveManager.GAME_SAVE_DIRECTORY))
        {
            if (dir.MakeDirRecursive(GameSaveManager.GAME_SAVE_DIRECTORY) != Error.Ok)
            {
                return false;
            }
        }

        return true;
    }

    public Dictionary<string, int> GetMainQuestsInProgress()
    {
        return _mainQuestsInProgressDictionary;
    }

    void RegisterMaps()
    {
        Logger.Log("Register Maps");

        _mapsData = new Dictionary<string, Dictionary<string, object>>();

        File file = new File();
        file.Open(DEFAULT_MAP_DATA_JSON_FILE, File.ModeFlags.Read);
        _defaultMapData = new Dictionary<string, object>(JSON.Parse(file.GetAsText()).Result as Dictionary);
        file.Close();

        Directory dir = new Directory();
        dir.Open(GameManager.MAP_DIRECTORY);
        dir.ListDirBegin(true, true);

        string current = dir.GetNext();
        while(current != "")
        {
            _mapsData.Add(current.Replace(".res", ""), _defaultMapData.Duplicate(true));
            Logger.Log("MAP \"" + current + "\" registered.", LogLevel.Info);
            current = dir.GetNext();
        }

        dir.ListDirEnd();
    }

    void LoadSaveFile()
    {
        Logger.Log("Loading Save File");

        _gameSaveRaw = new Dictionary();

        File file = new File();
        file.Open(_saveFilePath, File.ModeFlags.Read);
        _gameSaveRaw = JSON.Parse(file.GetAsText()).Result as Dictionary;
        file.Close();

        ConvertFromRawGameSave();
    }

    public void SaveSaveFile()
    {
        ConvertToRawGameSave();

        File file = new File();
        file.Open(_saveFilePath, File.ModeFlags.Write);
        file.StoreString(JSON.Print(_gameSaveRaw));
        file.Close();
    }

    void ConvertFromRawGameSave()
    {
        Logger.Log("Converting Raw Game Save");

        _collectedNotesArray = new Godot.Collections.Array<string>(_gameSaveRaw[GAME_SAVE_NOTES_KEY] as Godot.Collections.Array);
        ConvertDiaryEntries();
        _occurredEventsArray = new Array<string>(_gameSaveRaw[GAME_SAVE_OCCURRED_EVENTS_KEY] as Godot.Collections.Array);
        _mainQuestsCompletedArray = new Array<string>((_gameSaveRaw[GAME_SAVE_QUESTS_KEY] as Dictionary)[GAME_SAVE_COMPLETED_KEY] as Godot.Collections.Array);
        _mainQuestsInProgressDictionary = new Dictionary<string, int>((_gameSaveRaw[GAME_SAVE_QUESTS_KEY] as Dictionary)[GAME_SAVE_IN_PROGRESS_KEY] as Dictionary);
        ConvertMapData();

        CurrentMapId = _gameSaveRaw[GAME_SAVE_CURRENT_MAP_ID_KEY] as string;

        _items = new Array<Dictionary>(_gameSaveRaw[GAME_SAVE_ITEMS_KEY] as Godot.Collections.Array);

        _playerPosition = (Vector3)GD.Str2Var(_gameSaveRaw[Player.PLAYER_POSITION_KEY] as string);
        _playerRotation = (Vector3)GD.Str2Var(_gameSaveRaw[Player.PLAYER_ROTATION_KEY] as string);
    }

    void ConvertToRawGameSave()
    {
        _gameSaveRaw[Player.PLAYER_POSITION_KEY] = GD.Var2Str(Singletons.MainManager.GameManager.Player.Translation);
        _gameSaveRaw[Player.PLAYER_ROTATION_KEY] = GD.Var2Str(Singletons.MainManager.GameManager.Player.Rotation);

        _gameSaveRaw[GAME_SAVE_NOTES_KEY] = new Godot.Collections.Array(_collectedNotesArray);
        _gameSaveRaw[GAME_SAVE_DIARIES_KEY] = new Godot.Collections.Array(_collectedEntiresList);
        _gameSaveRaw[GAME_SAVE_OCCURRED_EVENTS_KEY] = new Godot.Collections.Array(_occurredEventsArray);
        _gameSaveRaw[GAME_SAVE_QUESTS_KEY] = new Dictionary() {
            {
                GAME_SAVE_COMPLETED_KEY,
                new Godot.Collections.Array(_mainQuestsCompletedArray)
            },
            {
                GAME_SAVE_IN_PROGRESS_KEY,
                new Dictionary<string, int>(_mainQuestsInProgressDictionary)
            }
        };
        _gameSaveRaw[GAME_SAVE_MAPS_DATA_KEY] = new Dictionary<string, Dictionary<string, object>>(_mapsData);
        _gameSaveRaw[GAME_SAVE_CURRENT_MAP_ID_KEY] = CurrentMapId;

        _items = Singletons.MainManager.GameManager.GUIManager.Inventory.GetItemsArray();
        _gameSaveRaw[GAME_SAVE_ITEMS_KEY] = _items;
    }

    void ConvertDiaryEntries()
    {
        Logger.Log("Convert Diary Entries");

        var entries = (_gameSaveRaw[GAME_SAVE_DIARIES_KEY] as Godot.Collections.Array);
        _collectedEntiresList = new System.Collections.Generic.List<string>();

        foreach (string entry in entries)
        {
            _collectedEntiresList.Add(entry);
        }

        _collectedEntiresList.Sort();
    }

    void ConvertMapData()
    {
        Logger.Log("Convert Map Data");

        var mapDataRaw = (_gameSaveRaw[GAME_SAVE_MAPS_DATA_KEY] as Dictionary);
        foreach (string key in mapDataRaw.Keys)
        {
            _mapsData[key] = new Dictionary<string, object>(mapDataRaw[key] as Dictionary);
        }
    }

    public Dictionary<string, object> GetMapData(string mapId)
    {
        if (_mapsData.ContainsKey(mapId))
        {
            return _mapsData[mapId];
        }
        else
        {
            return null;
        }
    }

    public void SetMapData(string mapId, Dictionary<string, object> data)
    {
        Logger.Log("Set Map Data: " + mapId);
        _mapsData[mapId] = data;
    }

    public Godot.Collections.Array<string> GetCollectedNotes()
    {
        return _collectedNotesArray;
    }

    public void AddNote(string noteId)
    {
        Logger.Log("Add Collected Note: " + noteId);
        _collectedNotesArray.Add(noteId);
    }

    public System.Collections.Generic.List<string> GetCollectedEntries()
    {
        return _collectedEntiresList;
    }

    public void AddEntry(string entryId)
    {
        Logger.Log("Add Collected Entry: " + entryId);
        _collectedEntiresList.Add(entryId);
        _collectedEntiresList.Sort();
    }

    public void RegisterMainQuest(string mainQuestId)
    {
        Logger.Log("RegisterMainQuest: " + mainQuestId);

        if (!_mainQuestsCompletedArray.Contains(mainQuestId) && !_mainQuestsInProgressDictionary.ContainsKey(mainQuestId))
        {
            Logger.Log("\"" + mainQuestId + "\" quest added");
            _mainQuestsInProgressDictionary.Add(mainQuestId, 0);
        }

        CheckForCompletion();
    }

    public void CheckForCompletion()
    {
        Logger.Log("Check for subquests completion.");

        foreach (string mainQuestId in _mainQuestsInProgressDictionary.Keys)
        {
            MainQuest mainQuest = Singletons.Database.GetMainQuest(mainQuestId);

            while (IsMainQuestsCurrentSubquestCompleted(mainQuest, _mainQuestsInProgressDictionary[mainQuestId]))
            {
                Logger.Log("MainQuest \"" + mainQuestId + "\" proceeded.");
                _mainQuestsInProgressDictionary[mainQuestId] += 1;
            }
        }

        while (!CheckForCompletedQuests());
    }

    public void RegisterEvent(string eventId)
    {
        Logger.Log("RegisterEvent: " + eventId);
        _occurredEventsArray.Add(eventId);

        CheckForCompletion();
    }

    bool IsMainQuestsCurrentSubquestCompleted(MainQuest mainQuest, int questProgress)
    {
        Logger.Log("Is \"" + mainQuest.Id + "\" subquest completed?");

        if (mainQuest.SubQuests.Length == questProgress)
            return false;
        
        bool stepMissing = false;

        foreach (string eventId in mainQuest.SubQuests[questProgress].EventsNeeded)
        {
            if (!IsEventRegistered(eventId))
                stepMissing = true;
        }
        
        return !stepMissing;
    }

    bool CheckForCompletedQuests()
    {
        Logger.Log("Check For Completed Quests");

        bool changed = false;
        foreach (var questProgresPair in _mainQuestsInProgressDictionary)
        {
            string questId = questProgresPair.Key;
            int questProgress = questProgresPair.Value;

            Logger.Log("Checking \"" + questId + "\".");

            if (questProgress == Singletons.Database.GetMainQuest(questId).SubQuests.Length)
            {
                Logger.Log("MainQuest \"" + questId + "\" completed.");
                changed = true;
                _mainQuestsInProgressDictionary.Remove(questId);
                _mainQuestsCompletedArray.Add(questId);
            }
        }

        return !changed;
    }

    public bool IsEventRegistered(string eventId)
    {
        return _occurredEventsArray.Contains(eventId);
    }

    public static bool CreateSaveFile(string profileName)
    {
        Logger.Log("Creating save file: " + profileName, LogLevel.Info);

        File file = new File();

        if (file.Open(DEFAULT_SAVE_GAME_PATH, File.ModeFlags.Read) != Error.Ok)
            return false;
        string defCont = file.GetAsText();
        file.Close();

        file = new File();

#if DEBUG
        if (file.Open(GAME_SAVE_DIRECTORY + "/" + profileName + GAME_SAVE_EXTENSION, File.ModeFlags.Write) != Error.Ok)
            return false;
#else
        if (file.OpenEncryptedWithPass(GAME_SAVE_DIRECTORY + "/" + profileName + GAME_SAVE_EXTENSION, File.ModeFlags.Write, "You4re4F*ck1ngH4cker") != Error.Ok)
            return false;
#endif
        file.StoreString(defCont);
        file.Close();

        return true;
    }

    public static bool GameSaveExists(string profileName)
    {
        File file = new File();
        return file.FileExists(GAME_SAVE_DIRECTORY + "/" + profileName + GAME_SAVE_EXTENSION);
    }

    public static Godot.Collections.Array<string> GetSavesArray()
    {
        Directory dir = new Directory();
        dir.Open(GAME_SAVE_DIRECTORY);
        dir.ListDirBegin(true, true);

        Godot.Collections.Array<string> names = new Array<string>();

        string current = dir.GetNext();
        while(current != "")
        {
            names.Add(current);
            current = dir.GetNext();
        }

        dir.ListDirEnd();
        return names;
    }

    public Godot.Collections.Array<Dictionary> GetItemsFromSave()
    {
        return _items;
    }

    public Vector3 GetPlayerPositionFromSave()
    {
        return _playerPosition;
    }

    public Vector3 GetPlayerRotationFromSave()
    {
        return _playerRotation;
    }
}
