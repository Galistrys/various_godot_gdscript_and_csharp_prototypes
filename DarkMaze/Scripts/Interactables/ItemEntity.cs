using Godot;
using Godot.Collections;
using System;

[Tool]
public class ItemEntity : RigidBody, IInteractable
{
    public const string PICKED_UP_DICT_KEY = "PickedUp";

    [Export]
    private string _interactableName = "ITEM_ENTITY";
    public string InteractableName { get { return _interactableName; } set { if (Engine.EditorHint) { _interactableName = value; } } }

    [Export]
    private EntityAction _action = EntityAction.PICK;
    public EntityAction Action { get { return _action; } }

    [Signal]
    public delegate void PickedUp(string itemId, string eventId);
    [Export]
    private string _onPickUpEvent = "";

    CollisionShape _col;
    MeshInstance _model;

    string _itemId = "";
    [Export]
    public string ItemId
    {
        get
        {
            return _itemId;
        }
        set
        {
            if (Engine.EditorHint)
            {
                if (!HasNode("Shape") || !HasNode("Model"))
                    return;
                
                _col = GetNode<CollisionShape>("Shape");
                _model = GetNode<MeshInstance>("Model");

                
                if (value == null || value == "")
                {
                    _col.Shape = null;
                    _model.Mesh = null;
                }
                else if (value != _itemId)
                {
                    _itemId = value;
                    File file = new File();

                    ItemData data = null;
                    try
                    {
                        data = Database.GetItemData(_itemId);
                    }
                    catch (Exception e)
                    {
                        GD.PushError("[ItemEntity] " + e.Message + "\n" + e.StackTrace.ToString());
                        _col.Shape = null;
                        _model.Mesh = null;
                        return;
                    }

                    Logger.Log("Got " + data.ToString());

                    string folder = _itemId;
                    if (data.Type == ItemType.Note)
                        folder = Item.NOTE_FOLDER;
                    else if (data.Type == ItemType.Diary)
                        folder = Item.DIARY_FOLDER;

                    string path = Item.ITEM_DATA_PATH + folder + "/" + Item.ITEM_MODEL_NAME + Item.ITEM_MODELS_EXT;

                    if (file.FileExists(path))
                    {
                        _model.Mesh = ResourceLoader.Load<Mesh>(path);
                        _col.Shape = _model.Mesh.CreateConvexShape();
                        InteractableName = data.ItemName;
                    }
                    else
                    {
                        GD.PushError("[ItemEntity] Item " + _itemId + " does not exist. Path: " + path);
                    }
                }
                else
                {
                    
                }
            }
            else
                _itemId = value;
        }
    }

    int _amount = 1;
    [Export]
    public int Amount
    {
        get
        {
            return _amount;
        }
        set
        {
            if (value < 1)
            {
                GD.PushError("[ItemEntity] Amount must be bigger than 0!");
                Amount = 1;
            }
            _amount = value;
        }
    }

    public override void _Ready()
    {
        _col = GetNode<CollisionShape>("Shape");
        _model = GetNode<MeshInstance>("Model");
    }

    public Item PickUp()
    {
        Logger.Log("ItemEntity with " + _itemId + " picked up.");

        EmitSignal(nameof(PickedUp), _itemId, _onPickUpEvent);

        Singletons.MainManager.GameManager.GameMap.RegisterItemEntity(this.Name);

        Free();
        
        return new Item(_itemId, _amount);
    }

    public bool UseItem(string itemId)
    {
        return false;
    }

    public Dictionary<string, object> ExportToDictionary()
    {
        return new Dictionary<string, object>() {
            {
                PICKED_UP_DICT_KEY,
                false
            },
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
        if ((bool)(dataDictionary[PICKED_UP_DICT_KEY]))
        {
            Free();
            return;
        }
            
        
        foreach (var pair in dataDictionary)
        {
            GD.Print(pair);
            if (pair.Key == PICKED_UP_DICT_KEY)
                continue;

            Set(pair.Key, GD.Str2Var(GD.Str(pair.Value)));
        }
    }
}
