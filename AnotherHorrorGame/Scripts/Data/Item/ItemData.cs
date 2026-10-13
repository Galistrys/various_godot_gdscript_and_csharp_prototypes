using Godot;
using Godot.Collections;

namespace HorrorGame.Data.Items
{
    public class ItemData
    {
        public const string ITEM_DATA_DIR = "res://Resources/Data/Items";
        public const string ITEM_MODEL_EXT = "mesh";
        public const string ITEM_SHAPE_EXT = "shape";
        public const string ITEM_TEXTURE_EXT = "tex";

        public const string ITEM_NAME_KEY = "name";
        public const string ITEM_DESCRIPTION_KEY = "description";
        public const string ITEM_COMBINE_KEY = "combine";
        public const string ITEM_IS_INSTANCEABLE_KEY = "is_instanceable";

        public string Name { get; private set; }
        public string Description { get; private set; }
        Dictionary<string, string> Combines { get; set; }
        public bool IsInstanceable { get; private set; }

        public ItemData(Dictionary<string, object> data)
        {
            Name = (string)data[ITEM_NAME_KEY];
            Description = (string)data[ITEM_DESCRIPTION_KEY];
            IsInstanceable = (bool)data[ITEM_IS_INSTANCEABLE_KEY];

            Combines = null;
            if (data.ContainsKey(ITEM_COMBINE_KEY))
            {
                Combines = new Dictionary<string, string>(data[ITEM_COMBINE_KEY] as Dictionary);
            }

            Logger.Log("Created: " + this.ToString());
        }

        public override string ToString()
        {
            return $"ItemData(Name: {Name}, Description: {Description}, IsInstanceable: {IsInstanceable})";
        }

        public static string GetMeshPath(string itemId)
        {
            return ITEM_DATA_DIR + "/" + itemId + "/" + itemId + "." + ITEM_MODEL_EXT;
        }

        public static string GetShapePath(string itemId)
        {
            return ITEM_DATA_DIR + "/" + itemId + "/" + itemId + "." + ITEM_SHAPE_EXT;
        }

        public static ImageTexture GetItemTexture(string itemId)
        {
            return ResourceLoader.Load<ImageTexture>(ITEM_DATA_DIR + "/" + itemId + "/" + itemId + "." + ITEM_TEXTURE_EXT);
        }

        public string CombineWith(string itemId)
        {
            if (Combines == null)
                return null;
            
            if (Combines.ContainsKey(itemId))
                return Combines[itemId];
            
            return null;
        }
    }
}