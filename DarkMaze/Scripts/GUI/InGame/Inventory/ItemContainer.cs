using Godot;
using System;

public class ItemContainer : Panel
{
    [Signal]
    public delegate void Highlighted(ItemContainer itemContainer);
    [Signal]
    public delegate void Dehighlighted();

    public string ItemId { get; private set; } = null;
    public int Amount { get; private set; } = 0;

    public TextureRect ItemIcon { get; private set; }
    Label _amountLabel;

    public bool IsHighlighted { get; private set; }

    public override void _Ready()
    {
        ItemIcon = GetNode<TextureRect>("ItemTexture");
        _amountLabel = GetNode<Label>("Amount");
        EraseData();
    }

    public void SetData(string itemId, int amount)
    {
        SetHighlighted(false);
        if (itemId == null || amount == 0)
            EraseData();
        else
        {
            ItemId = itemId;
            Amount = amount;
            ReloadData();
        }
    }

    public void SetData(Item item)
    {
        if (item != null)
            SetData(item.ItemId, item.Amount);
        else
            SetData(null, 0);
    }

    public void EraseData()
    {
        ItemId = null;
        Amount = 0;
        ItemIcon.Texture = null;
        _amountLabel.Text = "";
    }

    public void SetHighlighted(bool highlighted)
    {
        IsHighlighted = highlighted;

        if (highlighted)
        {
            ItemIcon.SelfModulate = Colors.LightGray;
            EmitSignal(nameof(Highlighted), this);
        }
        else
        {
            ItemIcon.SelfModulate = Colors.White;
            EmitSignal(nameof(Dehighlighted));
        }
    }

    private void OnMouseEntered()
    {
        if (ItemId != null)
            SetHighlighted(true);
        else
            SetHighlighted(false);
    }

    private void OnMouseExited()
    {
        SetHighlighted(false);
    }

    public void ReloadData()
    {
        ItemIcon.Texture = ResourceLoader.Load<Texture>(Item.ITEM_DATA_PATH + ItemId + "/" + Item.ITEM_ICON_NAME + Item.ITEM_ICON_EXT);
        _amountLabel.Text = Amount < 2 ? "" : GD.Str(Amount);
    }

    public void FakeOneLess()
    {
        int fakeAmount = Amount - 1;
        _amountLabel.Text = fakeAmount < 2 ? "" : GD.Str(fakeAmount);

        if (fakeAmount < 1)
            ItemIcon.Texture = null;
    }

    public bool IsMouseOver()
    {
        Vector2 mouseLocal = GetLocalMousePosition();

        return
            mouseLocal.x > 0 && mouseLocal.x < this.RectSize.x &&
            mouseLocal.y > 0 && mouseLocal.y < this.RectSize.y;
    }

    public override string ToString()
    {
        return $"ItemContainer [Name: {this.Name} ItemId: {ItemId}, Amount: {Amount} ]";
    }
}
