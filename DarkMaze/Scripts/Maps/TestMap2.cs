using Godot;
using Godot.Collections;
using System;

public class TestMap2 : GameMap
{
    public override string MapId { get { return "TEST_MAP_2"; } }

    protected override Dictionary<string, object> CustomData { get; set; } = new Dictionary<string, object>()
    {
        
    };

    public override void ConfigureMap()
    {
        
    }

    public void OnItemEntityIngrPickedUp(string itemId, string eventId)
    {
        Singletons.MainManager.GameManager.RegisterEvent(eventId);
    }
}
