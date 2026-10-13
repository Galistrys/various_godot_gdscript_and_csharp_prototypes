using Godot;
using Godot.Collections;
using System;

public class BasicEntity : StaticBody, IInteractable
{
    [Export]
    private string _interactableName = "BASIC_ENTITY";
    public string InteractableName { get { return _interactableName; } }

    [Export]
    private EntityAction _action = EntityAction.EXAMINE;
    public EntityAction Action { get { return _action; } }

    [Export]
    private string _examineText = "EXAMINE_TEXT";
    public string ExamineText { get { return _examineText; } }

    public Dictionary<string, object> ExportToDictionary()
    {
        return new Dictionary<string, object>();
    }

    public void LoadFromDictionary(Dictionary<string, object> dataDictionary)
    {
        
    }

    public bool UseItem(string itemId)
    {
        return false;
    }
}
