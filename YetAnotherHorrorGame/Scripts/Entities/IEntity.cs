using Godot;
using Godot.Collections;

namespace HorrorGame.Entities
{
    public interface IEntity
    {
        Dictionary<string, object> SaveToDictionary();
        bool LoadFromDictionary(Dictionary<string, object> data);
    }
}