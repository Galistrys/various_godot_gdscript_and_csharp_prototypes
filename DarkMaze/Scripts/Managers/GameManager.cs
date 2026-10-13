using Godot;
using Godot.Collections;
using System;

public class GameManager : Node
{
    GUIManager _guiManager = null;
    public GUIManager GUIManager
    {
        get
        {
            if (_guiManager == null)
            {
                _guiManager = GetNodeOrNull<GUIManager>("GUI/GUIManager");
            }
            return _guiManager;
        }
    }

    private Player _player = null;
    public Player Player
    {
        get
        {
            if (_player == null)
            {
                _player = GetNodeOrNull<Player>("World/Player");
            }
            return _player;
        }
    }

    public const string MAP_DIRECTORY = "res://Resources/Maps";
    public GameMap GameMap { get; private set; }

    private System.Collections.Generic.List<string> _diaryEntires = new System.Collections.Generic.List<string>();

    GameSaveManager GameSaveManager { get; set; }

    Spatial MapSpatial { get; set; }

    public GameManager()
    {
        GameSaveManager = new GameSaveManager(Singletons.MainManager.ProfileName);
        AddChild(GameSaveManager);
    }

    public override void _Ready()
    {
        MapSpatial = GetNode<Spatial>("World/Map");
        ChangeMapTo(GameSaveManager.CurrentMapId);
    }

    public void ChangeMapTo(string mapId)
    {
        GetTree().Paused = true;

        if (MapSpatial.GetChildCount() == 1)
        {
            var map = MapSpatial.GetChild<GameMap>(0);
            map.SaveMapDataToGameSaveDictionary();
            map.Free();
        }

        GameMap = ResourceLoader.Load<PackedScene>(MAP_DIRECTORY + "/" + mapId + ".res").Instance() as GameMap;
        MapSpatial.AddChild(GameMap);

        if (GameSaveManager.CurrentMapId != mapId)
            Player.Transform = GameMap.GetNode<Position3D>("SpawnPoints/" + GameSaveManager.CurrentMapId).Transform;

        GameSaveManager.CurrentMapId = mapId;

        GetTree().Paused = false;
    }

    public Godot.Collections.Array<string> GetCollectedNotes()
    {
        return GameSaveManager.GetCollectedNotes();
    }

    public void AddNote(string noteId)
    {
        GameSaveManager.AddNote(noteId);
    }

    public System.Collections.Generic.List<string> GetCollectedEntries()
    {
        return GameSaveManager.GetCollectedEntries();
    }

    public void AddEntry(string entryId)
    {
        GameSaveManager.AddEntry(entryId);
    }

    public void RegisterEvent(string eventId)
    {
        GameSaveManager.RegisterEvent(eventId);
    }

    public void RegisterMainQuest(string mainQuestId)
    {
        GameSaveManager.RegisterMainQuest(mainQuestId);
    }

    public Dictionary<string, object> GetMapData(string mapId)
    {
        return GameSaveManager.GetMapData(mapId);
    }

    public void SetMapData(string mapId, Dictionary<string, object> data)
    {
        GameSaveManager.SetMapData(mapId, data);
    }

    public Dictionary<string, int> GetMainQuestsInProgress()
    {
        return GameSaveManager.GetMainQuestsInProgress();
    }

    public void SaveGame()
    {
        var map = MapSpatial.GetChild<GameMap>(0);
        map.SaveMapDataToGameSaveDictionary();
        GameSaveManager.SaveSaveFile();
    }

    public Godot.Collections.Array<Dictionary> GetItemsFromSave()
    {
        return GameSaveManager.GetItemsFromSave();
    }

    public Vector3 GetPlayerPositionFromSave()
    {
        return GameSaveManager.GetPlayerPositionFromSave();
    }

    public Vector3 GetPlayerRotationFromSave()
    {
        return GameSaveManager.GetPlayerRotationFromSave();
    }
}
