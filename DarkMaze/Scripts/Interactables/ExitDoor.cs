using Godot;
using Godot.Collections;
using System;

public class ExitDoor : StaticBody, IInteractable
{
    [Export]
    private string _interactableName = "EXIT_DOOR";
    public string InteractableName { get { return _interactableName; } }

    [Export]
    private EntityAction _action = EntityAction.ENTER;
    public EntityAction Action { get { return _action; } }

    [Export]
    public bool _isLocked = false;
    public bool IsLocked
    {
        get
        {
            return _isLocked;
        }
        set
        {
            Logger.Log($"Setting IsLocked to \"{value}\"");
            _isLocked = value;
        }
    }
    [Export]
    string itemIdToUnlock = "";

    [Signal]
    public delegate void Unlocked(string eventId);
    [Signal]
    public delegate void DoorLocked(string eventId);

    [Export]
    private string _onUnlockEvent = "";
    [Export]
    private string _onDoorLockedEvent = "";

    [Export]
    string _mapId = "TEST_MAP";

    public bool TryToEnter()
    {
        Logger.Log("Try to Enter");

        if (!IsLocked)
        {
            Singletons.MainManager.GameManager.ChangeMapTo(_mapId);
            return true;
        }
        else
        {
            Logger.Log("Door is locked!");
            EmitSignal(nameof(DoorLocked), _onDoorLockedEvent);
            return false;
        }
    }

    public Dictionary<string, object> ExportToDictionary()
    {
        return new Dictionary<string, object>() {
            {
                nameof(IsLocked),
                GD.Var2Str(IsLocked)
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

    public bool UseItem(string itemId)
    {
        Logger.Log("Using item: " + itemId);

        if (IsLocked && itemId == itemIdToUnlock)
        {
            IsLocked = false;
            EmitSignal(nameof(Unlocked), _onUnlockEvent);
            return true;
        }
        return false;
    }
}
