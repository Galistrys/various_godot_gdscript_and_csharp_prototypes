using Godot;
using Godot.Collections;
using System;

public class Inventory : PanelContainer
{
    public const int INVENTORY_SIZE = 15;
    Item[] _items = new Item[INVENTORY_SIZE];

    GridContainer _itemsContainer;
    Label _itemNameLabel;
    RichTextLabel _itemDescRichTextLabel;

    public override void _Ready()
    {
        _itemsContainer = GetNode<GridContainer>("Columns/MiddleColumn/ItemsContainer");
        _itemNameLabel = GetNode<Label>("Columns/MiddleColumn/ItemData/ItemName");
        _itemDescRichTextLabel = GetNode<RichTextLabel>("Columns/MiddleColumn/ItemData/ItemDesc");

        ItemContainerDehighlighted();

        foreach (ItemContainer ic in _itemsContainer.GetChildren())
        {
            ic.Connect(nameof(ItemContainer.Highlighted), this, nameof(ItemContainerHighlighted));
            ic.Connect(nameof(ItemContainer.Dehighlighted), this, nameof(ItemContainerDehighlighted));
        }

        foreach (var dict in Singletons.MainManager.GameManager.GetItemsFromSave())
        {
            AddItem(new Item(
                dict[Item.ITEM_ID_KEY] as string,
                (int)GD.Str2Var(GD.Str(dict[Item.ITEM_AMOUNT_KEY]))
            ));
        }

        UpdateInventory();
    }

    ~Inventory()
    {
        GD.Print(this.ToString() + " destroyed");
    }

    public void UpdateInventory()
    {
        Logger.Log("Update GUI Inventory.");

        Item[] oldItems = _items;
        _items = new Item[INVENTORY_SIZE];

        int newCounter = 0;
        for (int i = 0; i < INVENTORY_SIZE; i++)
        {
            if (oldItems[i] != null && oldItems[i].Amount < 1)
                oldItems[i] = null;

            if (oldItems[i] != null)
            {
                _items[newCounter++] = oldItems[i];
            }
        }

        for (int i = 0; i < INVENTORY_SIZE; i++)
        {
            _itemsContainer.GetChild<ItemContainer>(i).SetData(_items[i]);
        }
    }

    public void ItemContainerHighlighted(ItemContainer itemContainer)
    {
        Logger.Log("Item Container highlighted: " + itemContainer.ToString());
        ItemData data = Database.GetItemData(itemContainer.ItemId);

        _itemNameLabel.Text = data.ItemName;
        _itemDescRichTextLabel.BbcodeText = TranslationServer.Translate(data.ItemContent);
    }

    public void ItemContainerDehighlighted()
    {
        _itemNameLabel.Text = "";
        _itemDescRichTextLabel.BbcodeText = "";
    }

    public void AddItem(Item newItem)
    {
        Logger.Log("Adding item: " + newItem.ToString());
        bool found = false;
        for (int i = 0; i < INVENTORY_SIZE && !found; i++)
        {
            if (_items[i] == null)
            {
                _items[i] = newItem;
                found = true;
            }
            else if (_items[i].ItemId == newItem.ItemId)
            {
                _items[i].Amount += newItem.Amount;
                found = true;
            }
        }

        if (found)
        {
            UpdateInventory();
        }
        else
        {
            Logger.Log("INVENTORY FULL! Need to change level design.", LogLevel.Error);
        }
    }

    public void RemoveOne(string itemId)
    {
        Logger.Log($"Removing one \"{itemId}\" from inventory.");
        bool found = false;
        for (int i = 0; i < INVENTORY_SIZE && !found; i++)
        {
            if (_items[i] != null)
            {
                if (_items[i].ItemId == itemId)
                {
                    _items[i].Amount = 0;
                    found = true;
                }
            }
        }

        if (found)
        {
            UpdateInventory();
        }
        else
        {
            Logger.Log("In player's inventory there is no " + itemId + ".", LogLevel.Error);
        }
    }

    public ItemContainer GetCurrentItemContainer()
    {
        foreach (ItemContainer ic in _itemsContainer.GetChildren())
        {
            if (ic.IsMouseOver())
                return ic;
        }

        return null;
    }

    public Godot.Collections.Array<Dictionary> GetItemsArray()
    {
        Godot.Collections.Array<Dictionary> resultArray = new Godot.Collections.Array<Dictionary>();

        foreach (Item item in _items)
        {
            if (item != null)
            {
                resultArray.Add(new Dictionary() {
                    {
                        Item.ITEM_ID_KEY,
                        item.ItemId
                    },
                    {
                        Item.ITEM_AMOUNT_KEY,
                        item.Amount
                    }
                });
            }
        }

        return resultArray;
    }
}
