using Godot;
using Godot.Collections;
using System;

namespace SpaceAdventure.Entites
{
    public interface IEntity
    {
        Dictionary<string, object> SaveToDictionary();
        bool LoadFromDictionary(Dictionary<string, object> data);
    }
}