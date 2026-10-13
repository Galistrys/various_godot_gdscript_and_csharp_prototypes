using Godot;
using Godot.Collections;
using System;

public class MovableEntity : RigidBody, IInteractable
{
    [Export]
    private string _interactableName = "MOVABLE_ENTITY";
    public string InteractableName { get { return _interactableName; } }

    [Export]
    private EntityAction _action = EntityAction.GRAB;
    public EntityAction Action { get { return _action; } }

    public virtual bool UseItem(string itemId)
    {
        return false;
    }

    public Dictionary<string, object> ExportToDictionary()
    {
        return new Dictionary<string, object>()
        {
            {
                nameof(this.Translation),
                GD.Var2Str(this.Translation)
            },
            {
                nameof(this.Rotation),
                GD.Var2Str(this.Rotation)
            }
        };
    }

    public void LoadFromDictionary(Dictionary<string, object> dataDictionary)
    {
        foreach (var pair in dataDictionary)
        {
            Set(pair.Key, GD.Str2Var(GD.Str(pair.Value)));
        }
    }
}
