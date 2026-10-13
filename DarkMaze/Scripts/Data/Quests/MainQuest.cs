using Godot;
using Godot.Collections;
using System;

public class MainQuest
{
    public const string QUEST_NAME_KEY = "QuestName";
    public const string START_INDEX_KEY = "StartIndex";
    public const string SUB_QUESTS_KEY = "SubQuests";

    public string Id { get; private set; } = null;
    public string Name { get; private set; } = null;
    public SubQuest[] SubQuests { get; private set; } = null;

    public MainQuest(string id, Dictionary dict)
    {
        Id = id;
        Name = dict[QUEST_NAME_KEY] as string;
        
        Godot.Collections.Array subQuestsRawArray = dict[SUB_QUESTS_KEY] as Godot.Collections.Array;
        SubQuests = new SubQuest[subQuestsRawArray.Count];

        Logger.Log($"MainQuest creation. Id: {Id}, Name: {Name}", LogLevel.Info);

        for (int i = 0; i < subQuestsRawArray.Count; i++)
        {
            SubQuests[i] = new SubQuest(subQuestsRawArray[i] as Dictionary);
        }

        Logger.Log($"MainQuest created. Id: {Id}, Name: {Name}", LogLevel.Info);
    }
}
