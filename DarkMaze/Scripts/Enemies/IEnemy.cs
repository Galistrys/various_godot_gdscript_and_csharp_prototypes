using Godot;
using Godot.Collections;
using System;

public interface IEnemy
{
    Dictionary<string, object> ExportToDictionary();
    void LoadFromDictionary(Dictionary<string, object> dataDictionary);
}
