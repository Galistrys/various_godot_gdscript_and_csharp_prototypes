using Godot;
using System;

public class Diary : PanelContainer
{
    public VBoxContainer EntriesList { get; private set; }

    public override void _Ready()
    {
        EntriesList = GetNode<VBoxContainer>("ScrollCont/Columns/EntriesList");
    }

    public Button CreateEntry(string entryId)
    {
        Logger.Log("Create Entry: " + entryId);

        Button button = new Button();
        button.Name = entryId;
        button.Text = Database.GetItemData(entryId).ItemName;

        button.Connect("pressed", this, nameof(EntryPressed), new Godot.Collections.Array() { entryId });
        return button;
    }

    public void AddMissingEntries()
    {
        Logger.Log("Adding Missing Entries");

        var foundEntries = Singletons.MainManager.GameManager.GetCollectedEntries();

        foreach (string entryId in foundEntries)
        {
            if (!EntriesList.HasNode(entryId))
            {
                Logger.Log("Adding " + entryId);
                bool added = false;
                foreach (Button btn in EntriesList.GetChildren())
                {
                    if (string.Compare(btn.Name, entryId) > 0)
                    {
                        EntriesList.AddChildBelowNode(btn, CreateEntry(entryId));
                        added = true;
                        break;
                    }
                }

                if (!added)
                {
                    EntriesList.AddChild(CreateEntry(entryId));
                }
            }
        }
    }

    public void EntryPressed(string entryId)
    {
        Logger.Log("Entry pressed: " + entryId);
        if (Singletons.MainManager.GameManager.GUIManager.PeekState() is DiaryGUIState diaryGUIState)
        {
            diaryGUIState.OnEntryPressed(entryId);
        }
    }
}
