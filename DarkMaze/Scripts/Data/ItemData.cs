using Godot;
using Godot.Collections;
using System;

public class ItemData
{
    public string ItemName { get; private set; }
    public string ItemContent { get; private set; }
    public Dictionary CombineDict { get; private set; }
    public ItemType Type { get; private set; }
    public string OnCreatedEventId { get; private set; } = null;

    public ItemData(string itemName, string itemDesc, Dictionary combineDict, ItemType type)
    {
        ItemName = itemName;
        ItemContent = itemDesc;
        CombineDict = combineDict;
        Type = type;
    }

    public ItemData(Dictionary dict)
    {
        ItemName = (dict["ItemName"] as string);
        ItemContent = (dict["ItemContent"] as string);
        Type = (ItemType)(int.Parse(dict["ItemType"] as string));
        if (Type == ItemType.Item)
        {
            CombineDict = (dict["CombineWith"] as Dictionary);
            if (dict.Contains("OnCreatedEventId"))
            {
                OnCreatedEventId = dict["OnCreatedEventId"] as string;
            }
        }
        else
            CombineDict = null;

        Logger.Log($"{this.ToString()} created.", LogLevel.Info);
    }

    public bool CanBeCombinedWith(string itemId, out string resultId)
    {
        if (CombineDict.Contains(itemId))
        {
            resultId = CombineDict[itemId] as string;
            return true;
        }
        else
        {
            resultId = null;
            return false;
        }
    }

    public override string ToString()
    {
        return $"ItemData [ ItemName: {ItemName}; ItemDesc: {ItemContent} ]";
    }
}
