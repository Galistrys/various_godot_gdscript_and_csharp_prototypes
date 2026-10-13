using Godot;
using Godot.Collections;
using System;

public class TestMap : GameMap
{
    public override string MapId { get { return "TEST_MAP"; } }

    public const string TestDoorOpenQuestRegisteredKey = "TEST_DOOR_OPEN_QUEST_REGISTERED";

    protected override Dictionary<string, object> CustomData { get; set; } = new Dictionary<string, object>()
    {
        {
            TestDoorOpenQuestRegisteredKey,
            false
        }
    };

    public void OnTestDoorHandlePressed(string eventId)
    {
        if (!(bool)(CustomData[TestDoorOpenQuestRegisteredKey]))
        {
            Singletons.MainManager.GameManager.RegisterMainQuest("TEST_DOOR_OPEN");
            CustomData[TestDoorOpenQuestRegisteredKey] = true;
        }
    }

    public void OnTestDoorUnlocked(string eventId)
    {
        Singletons.MainManager.GameManager.RegisterEvent(eventId);
    }

    public void OnTestItemItemEntityPickedUp(string itemId, string eventId)
    {
        Singletons.MainManager.GameManager.RegisterEvent(eventId);
    }

    public void OnTestIngrItemEntityPickedUp(string itemId, string eventId)
    {
        Singletons.MainManager.GameManager.RegisterEvent(eventId);
    }

    public void OnTestNoteItemEntityPickedUp(string itemId, string eventId)
    {
        Singletons.MainManager.GameManager.RegisterEvent(eventId);
    }

    public override void ConfigureMap()
    {
        // Apply data to the scene from CustomData
    }
}
