using Godot;
using Godot.Collections;
using System;

public class Database : Node, ISingleton
{
    public const string ITEM_DATA_JSON_FILE_PATH = "res://Resources/Data/ItemData.json";
    public const string QUESTS_DATA_JSON_FILE_PATH = "res://Resources/Data/QuestsData.json";

    static System.Collections.Generic.Dictionary<string, ItemData> _itemDataDictionary = null;

    private System.Collections.Generic.Dictionary<string, MainQuest> _mainQuests = null;

    public override void _Ready()
    {
        if (!LoadItemDatasFromJSON())
        {
            GetTree().Quit(1);
        }
        Logger.Log("ItemData loaded", LogLevel.Info);

        if (!LoadQuestDatasFromJSON())
        {
            GetTree().Quit(2);
        }
        Logger.Log("QuestsData loaded", LogLevel.Info);
    }

    public static bool LoadItemDatasFromJSON()
    {
        Logger.Log("Loading ItemDatas from JSON file.", LogLevel.Info);

        File file = new File();
        Error result = file.Open(ITEM_DATA_JSON_FILE_PATH, File.ModeFlags.Read);
        if (result != Error.Ok)
        {
            Logger.Log("Error code: " + GD.Str(result), LogLevel.Error);
            return false;
        }

        var jsonResult = JSON.Parse(file.GetAsText());
        if (jsonResult.Error != Error.Ok)
        {
            Logger.Log("Error code: " + GD.Str(jsonResult.Error), LogLevel.Error);
            return false;
        }

        file.Close();

        Dictionary itemDataFromJSON = jsonResult.Result as Dictionary;

        _itemDataDictionary = new System.Collections.Generic.Dictionary<string, ItemData>();
        foreach (var key in itemDataFromJSON.Keys)
        {
            Logger.Log($"Loading ItemData of \"{key.ToString()}\"");
            _itemDataDictionary.Add(key as string, new ItemData(itemDataFromJSON[key] as Dictionary));
        }

        return true;
    }

    public static ItemData GetItemData(string itemId)
    {
        if (Engine.EditorHint && _itemDataDictionary == null)
        {
            if (!LoadItemDatasFromJSON())
            {
                throw new Exception("Failed to load JSON Data.");
            }
        }


        ItemData itemData = null;
        if (_itemDataDictionary.TryGetValue(itemId, out itemData))
        {
            return itemData;
        }
        else
        {
            throw new Exception("Item \"" + itemId + "\" does not exist in Database.");
        }
    }

    public void ResetSingleton()
    {
        
    }

    private bool LoadQuestDatasFromJSON()
    {
        Logger.Log("Loading QuestDatas from JSON file.", LogLevel.Info);

        File file = new File();
        Error result = file.Open(QUESTS_DATA_JSON_FILE_PATH, File.ModeFlags.Read);
        if (result != Error.Ok)
        {
            Logger.Log("Error code: " + GD.Str(result), LogLevel.Error);
            return false;
        }

        var jsonResult = JSON.Parse(file.GetAsText());
        if (jsonResult.Error != Error.Ok)
        {
            Logger.Log("Error code: " + GD.Str(jsonResult.Error), LogLevel.Error);
            return false;
        }

        file.Close();

        _mainQuests = new System.Collections.Generic.Dictionary<string, MainQuest>();
        Dictionary dictResult = jsonResult.Result as Dictionary;
        var keys = dictResult.Keys;
        foreach (var questKey in keys)
        {
            Logger.Log($"Loading QuestData of \"{questKey.ToString()}\"");
            _mainQuests.Add(questKey as string, new MainQuest(questKey as string, dictResult[questKey] as Dictionary));
        }

        return true;
    }

    public MainQuest GetMainQuest(string questId)
    {
        if (_mainQuests.ContainsKey(questId))
        {
            return _mainQuests[questId];
        }
        else
        {
            throw new Exception("Quest \"" + questId + "\" does not exist in Database.");
        }
    }
}

