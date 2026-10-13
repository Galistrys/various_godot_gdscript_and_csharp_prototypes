using Godot;
using Godot.Collections;
using System;

public abstract class StateEntity : StaticBody, IInteractable
{
    [Export]
    private string _interactableName = "STATE_ENTITY";
    public string InteractableName { get { return _interactableName; } }

    [Export]
    private EntityAction _action = EntityAction.PRESS;
    public EntityAction Action { get { return _action; } }
    
    public abstract void BeginChangeState();
    public abstract void EndChangeState();

    public virtual bool UseItem(string itemId)
    {
        return false;
    }

    public abstract Dictionary<string, object> ExportToDictionary();
    public abstract void LoadFromDictionary(Dictionary<string, object> dataDictionary);
}
