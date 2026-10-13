using Godot;
using Godot.Collections;
using System;

public class SubQuest
{
    public const string SUB_QUEST_NAME_KEY = "SubQuestName";
    public const string EVENTS_NEEDED_KEY = "EventsNeeded";

    public string Name { get; private set; } = null;
    public string[] EventsNeeded { get; private set; } = null;

    public SubQuest(Dictionary dict)
    {
        Name = dict[SUB_QUEST_NAME_KEY] as string;

        Godot.Collections.Array events = dict[EVENTS_NEEDED_KEY] as Godot.Collections.Array;
        EventsNeeded = new string[events.Count];

        for (int i = 0; i < events.Count; i++)
        {
            EventsNeeded[i] = events[i] as string;
        }

        Logger.Log($"SubQuest created. Name: {Name}", LogLevel.Info);
    }
}
