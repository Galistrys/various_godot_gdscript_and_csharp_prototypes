using Godot;
using Godot.Collections;
using System;

public class Item
{
    public const string ITEM_DATA_PATH = "res://Resources/Items/";
    public const string ITEM_ICON_EXT = ".png";
    public const string ITEM_ICON_NAME = "item_icon";
    public const string ITEM_MODELS_EXT = ".mesh";
    public const string ITEM_MODEL_NAME = "model";
    public const string NOTE_FOLDER = "NOTE_ITEM";
    public const string DIARY_FOLDER = "DIARY_ITEM";

    public const string ITEM_ID_KEY = "item_id";
    public const string ITEM_AMOUNT_KEY = "item_amount";

    public string ItemId { get; private set; }
    public int Amount { get; set; }

    public Item(string itemId, int amount)
    {
        ItemId = itemId;
        Amount = amount;

        Logger.Log($"Item created. ItemId: {ItemId}, Amount: {Amount}");
    }

    public override string ToString()
    {
        if (ItemId == null)
            return GD.Str("Item: [ ItemId: null, Amount: ", Amount, " ]");
        else
            return GD.Str("Item: [ ItemId: ", ItemId, ", Amount: ", Amount, " ]");
    }
}
