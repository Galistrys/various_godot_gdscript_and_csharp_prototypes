using Godot;
using Godot.Collections;
using System;

public abstract class GameMap : Spatial
{
    public const string INTERACTABLES_DATA_KEY = "interactables";
    public const string ENEMIES_DATA_KEY = "enemies";
    public const string CUSTOM_DATA_KEY = "custom";

    public abstract string MapId { get; }

    Dictionary<string, object> MapData { get; set; }
    protected Dictionary<string, Dictionary<string, object>> InteractablesData { get; set; }
    protected Dictionary<string, Dictionary<string, object>> EnemiesData { get; set; }
    protected abstract Dictionary<string, object> CustomData { get; set; }

    protected Spatial _enemiesSpatial;
    protected Spatial _interactablesSpatial;

    public override void _Ready()
    {
        _interactablesSpatial = GetNode<Spatial>("Interactables");
        _enemiesSpatial = GetNode<Spatial>("Enemies");

        MapData = Singletons.MainManager.GameManager.GetMapData(MapId);
        LoadMapDataFromDictionary(MapData);
    }

    ~GameMap()
    {
        GD.Print(this);
    }

    public void SaveMapDataToGameSaveDictionary()
    {
        Logger.Log("Save Map Data To Game Save Dictionary: " + this.MapId);

        UpdateInteractablesDataForExport();
        UpdateEnemiesDataForExport();

        MapData[INTERACTABLES_DATA_KEY] = InteractablesData;
        MapData[ENEMIES_DATA_KEY] = EnemiesData;
        MapData[CUSTOM_DATA_KEY] = CustomData;

        Singletons.MainManager.GameManager.SetMapData(MapId, MapData);
    }

    public void LoadMapDataFromDictionary(Dictionary<string, object> dataDictionary)
    {
        Logger.Log("Load Map Data To Game Save Dictionary: " + this.MapId);

        MapData = dataDictionary;

        InteractablesData = new Dictionary<string, Dictionary<string, object>>(MapData[INTERACTABLES_DATA_KEY] as Dictionary);
        EnemiesData = new Dictionary<string, Dictionary<string, object>>(MapData[ENEMIES_DATA_KEY] as Dictionary);
        var temp = new Dictionary<string, object>(MapData[CUSTOM_DATA_KEY] as Dictionary);

        foreach (var keyPair in temp)
        {
            if (CustomData.ContainsKey(keyPair.Key))
            {
                CustomData[keyPair.Key] = keyPair.Value;
            }
            else
            {
                CustomData.Add(keyPair);
            }
        }

        LoadInteractablesDataFromDictionary();
        LoadEnemiesDataFromDictionary();

        ConfigureMap();
        SaveMapDataToGameSaveDictionary();
    }

    public void LoadInteractablesDataFromDictionary()
    {
        Logger.Log("Load Interactables Data");

        foreach (var pair in InteractablesData)
        {
            Logger.Log("Loading data of: " + pair.Key);
            (_interactablesSpatial.GetNode<Spatial>(pair.Key) as IInteractable).LoadFromDictionary(pair.Value);
        }
    }

    public void LoadEnemiesDataFromDictionary()
    {
        Logger.Log("Load Enemies Data");

        foreach (var pair in EnemiesData)
        {
            Logger.Log("Loading data of: " + pair.Key);
            (_enemiesSpatial.GetNode<Spatial>(pair.Key) as IEnemy).LoadFromDictionary(pair.Value);
        }
    }

    public abstract void ConfigureMap();

    public void UpdateInteractablesDataForExport()
    {
        Logger.Log("Update Interactables Data");

        foreach (Spatial interactable in _interactablesSpatial.GetChildren())
        {
            string name = interactable.Name;
            var data = (interactable as IInteractable).ExportToDictionary();

            if (InteractablesData.ContainsKey(name))
                InteractablesData[name] = data;
            else
                InteractablesData.Add(name, data);
        }
    }

    public void UpdateEnemiesDataForExport()
    {
        Logger.Log("Update Enemies Data");

        foreach (Spatial enemy in _enemiesSpatial.GetChildren())
        {
            string name = enemy.Name;
            var data = (enemy as IEnemy).ExportToDictionary();
            if (EnemiesData.ContainsKey(name))
                EnemiesData[name] = data;
            else
                EnemiesData.Add(name, data);
        }
    }

    public void RegisterItemEntity(string itemEntityName)
    {
        InteractablesData[itemEntityName]["PickedUp"] = true;
    }
}
