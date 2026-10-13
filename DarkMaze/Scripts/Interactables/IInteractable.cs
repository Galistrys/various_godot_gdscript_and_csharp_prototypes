using Godot;
using Godot.Collections;
using System;

public interface IInteractable
{
    string InteractableName { get; }
    EntityAction Action { get; }

    bool UseItem(string itemId);

    Dictionary<string, object> ExportToDictionary();
    void LoadFromDictionary(Dictionary<string, object> dataDictionary);
}
