using Godot;
using Godot.Collections;
using System;

namespace KayEth.Entities
{
    public interface IEntity
    {
        string EntityName { get; }

        Dictionary SaveToDictionary();
        void LoadFromDictionary(Dictionary data);
    }
}
