using Godot;
using System;

public class Reader : PanelContainer
{
    Label _titleLabel;
    RichTextLabel _contentRichTextLabel;

    public override void _Ready()
    {
        _titleLabel = GetNode<Label>("HBoxContainer/VBoxContainer/TitleLabel");
        _contentRichTextLabel = GetNode<RichTextLabel>("HBoxContainer/VBoxContainer/Content");
    }

    public void ShowContent(string itemId)
    {
        Logger.Log("Showing content of \"" + itemId + "\"");

        ItemData itemData = Database.GetItemData(itemId);

        _titleLabel.Text = itemData.ItemName;
        _contentRichTextLabel.BbcodeText = TranslationServer.Translate(itemData.ItemContent).Replace("\\n", "\n");
    }

    public void Clean()
    {
        Logger.Log("Reader cleaning");

        _titleLabel.Text = "";
        _contentRichTextLabel.BbcodeText = "";
    }
}
