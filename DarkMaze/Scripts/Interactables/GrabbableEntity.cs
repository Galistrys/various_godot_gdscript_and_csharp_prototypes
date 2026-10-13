using Godot;
using Godot.Collections;
using System;

public abstract class GrabbableEntity : RigidBody, IInteractable
{
    [Export]
    private string _interactableName = "GRABBABLE_ENTITY";
    public string InteractableName { get { return _interactableName; } }

    [Export]
    private EntityAction _action = EntityAction.GRAB;
    public EntityAction Action { get { return _action; } }

    public abstract void MouseMove(Spatial caller, Vector2 mouse, float sensitivity, float delta);
    public abstract bool UseItem(string itemId);

    public virtual void BeginInteraction()
    {

    }

    public virtual void EndedInteraction()
    {

    }

    public abstract Dictionary<string, object> ExportToDictionary();
    public abstract void LoadFromDictionary(Dictionary<string, object> dataDictionary);
}
