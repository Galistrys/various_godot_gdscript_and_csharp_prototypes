using Godot;
using System.Collections.Generic;
using HorrorGame.Data.Items;

namespace HorrorGame.Data.Databases
{
    public class ItemsDatabase : GameDataDatabase
    {
        public const string ITEMS_DATA_FILE_PATH = "res://Resources/Data/Items.json";
        Dictionary<string, ItemData> _itemDatas;

        public ItemsDatabase()
        {
            _itemDatas = new Dictionary<string, ItemData>();
        }

        public override bool LoadData()
        {
            return LoadData(ITEMS_DATA_FILE_PATH);
        }

        protected override bool ConvertData(Godot.Collections.Dictionary dictionary)
        {
            foreach (var itemId in dictionary.Keys)
            {
                _itemDatas.Add(itemId as string, new ItemData(new Godot.Collections.Dictionary<string, object>(dictionary[itemId] as Godot.Collections.Dictionary)));
            }

            return true;
        }

        public bool GetItemData(string itemId, out ItemData data)
        {
            return _itemDatas.TryGetValue(itemId, out data);
        }

        public bool ItemDataExists(string itemId)
        {
            return _itemDatas.ContainsKey(itemId);
        }
    }
}