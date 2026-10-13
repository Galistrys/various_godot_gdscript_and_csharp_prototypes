using Godot;
using Godot.Collections;
using HorrorGame.Data;
using HorrorGame.Data.Items;
using HorrorGame.Managers;
using System;

namespace HorrorGame.Entities.Interactables.Pickable
{
    [Tool]
    public class ItemEntity : PickableInteractable
    {
        string _interactableId;

        [Export]
        public override string InteractableId
        {
            get
            {
                return _interactableId;
            }
            set
            {
                if (Engine.EditorHint)
                {
                    if (!HasNode("Col") || !HasNode("Model"))
                        return;

                    if (value == null && value == "")
                    {
                        Col.Shape = null;
                        Model.Mesh = null;
                    }
                    else if (value != _interactableId)
                    {
                        _interactableId = value;
                        File file = new File();

                        if (file.FileExists(ItemData.GetMeshPath(_interactableId)) && file.FileExists(ItemData.GetShapePath(_interactableId)))
                        {
                            Logger.Log($"\"{_interactableId}\"'s mesh and shape found.", LogLevel.Info);
                            LoadMeshAndCollision(_interactableId);
                        }
                        else
                        {
                            Logger.Log($"ItemEnitity: \"{_interactableId}\"'s mesh and/or shape not found. Item does not exists or maybe is not instanceable.", LogLevel.Error);
                            return;
                        }
                    }
                }
                else
                {
                    _interactableId = value;
                }
            }
        }

        CollisionShape Col { get { return GetNodeOrNull<CollisionShape>("Col"); } }
        MeshInstance Model { get { return GetNodeOrNull<MeshInstance>("Model"); } }

        public override void _Ready()
        {
            base._Ready();

            if (!Engine.EditorHint)
                LoadMeshAndCollision(_interactableId);
        }

        protected override void OnPickUp()
        {
            base.OnPickUp();

            MainManager.Instance.GameManager.HUDController.Pointer.ShowMessage(TranslationServer.Translate("PICKED_UP_MESSAGE").Replace("<<item_name>>", TranslationServer.Translate(Database.Instance.GetItemData(_interactableId).Name)));
            GameSaveManager.CurrentGameSave.AddItem(_interactableId);
            
            Logger.Log("Picked Up");
        }

        void LoadMeshAndCollision(string itemId)
        {
            Model.Mesh = ResourceLoader.Load<Mesh>(ItemData.GetMeshPath(itemId));
            Col.Shape = ResourceLoader.Load<Shape>(ItemData.GetShapePath(itemId));;
        }
    }
}