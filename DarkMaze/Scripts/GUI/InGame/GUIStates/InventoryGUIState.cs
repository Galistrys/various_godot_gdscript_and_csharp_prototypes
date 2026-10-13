using Godot;
using System;

public class InventoryGUIState : GUIState
{
    GUIManager _guiManager;
    Inventory _inventory;
    TextureRect _heldItemTextureRect;

    Vector2 _heldItemOffset = Vector2.Zero;

    bool _techFlag = false;

    public InventoryGUIState()
    {
        _guiManager = Singletons.MainManager.GameManager.GUIManager;
        _inventory = _guiManager.Inventory;
        _heldItemTextureRect = _guiManager.GetNode<TextureRect>("HeldItemIcon");
        _guiManager.PushState(new BackgroundGUIState());
    }

    public override void AtStart()
    {
        _inventory.ItemContainerDehighlighted();
        _inventory.UpdateInventory();
        _inventory.Visible = true;
    }

    public override void OnResume()
    {
        _inventory.Visible = true;
    }

    public override void OnPause()
    {
        _inventory.Visible = false;
    }

    public override void AtEnd()
    {
        _inventory.Visible = false;
        _inventory.ItemContainerDehighlighted();
        _heldItemTextureRect.Visible = false;
    }

    public override void OnEscapePress()
    {
        _guiManager.PopState();
        _inventory.ItemContainerDehighlighted();
    }

    ItemContainer _beginItemContainer = null;

    public override void OnInteractionEvent(bool pressed) // Pick/Combine item
    {
        Logger.Log($"OnInteractionEvent. Pressed: {pressed}");
        ItemContainer currentContainer = _inventory.GetCurrentItemContainer();

        if (currentContainer != null && currentContainer.ItemId == null)
            currentContainer = null;

        if (pressed && currentContainer != null)
        {
            Logger.Log("CurrentContainer: " + currentContainer.ToString());
            _beginItemContainer = currentContainer;
            _heldItemTextureRect.Texture = _beginItemContainer.ItemIcon.Texture;
            RefreshOffset();
            RefreshHeldItemPos();
            _heldItemTextureRect.Visible = true;
            _beginItemContainer.FakeOneLess();
        }
        else if (!pressed && _beginItemContainer != null)
        {
            if (currentContainer == null)
            {
                Logger.Log("Resetting Begin Container: " + _beginItemContainer.ToString());
                _heldItemTextureRect.Visible = false;
                _beginItemContainer.ReloadData();
                _beginItemContainer = null;
            }
            else
            {
                string beginId = _beginItemContainer.ItemId;
                string currentId = currentContainer.ItemId;

                Logger.Log($"Trying to combine items: \"{beginId}\" and \"{currentId}\"");

                ItemData data = Database.GetItemData(beginId);
                string resultId = null;

                if (_beginItemContainer != currentContainer && data.CanBeCombinedWith(currentId, out resultId))
                {
                    Logger.Log($"Combining items: \"{beginId}\" and \"{currentId}\"");
                    _beginItemContainer = null;

                    _heldItemTextureRect.Visible = false;

                    _inventory.RemoveOne(beginId);
                    _inventory.RemoveOne(currentId);

                    _inventory.AddItem(new Item(resultId, 1));

                    ItemData resultData = Database.GetItemData(resultId);
                    if (resultData.OnCreatedEventId != null)
                    {
                        Singletons.MainManager.GameManager.RegisterEvent(resultData.OnCreatedEventId);
                    }
                }
                else
                {
                    Logger.Log($"Items cannot be combined: \"{beginId}\" and \"{currentId}\"");
                    _heldItemTextureRect.Visible = false;
                    _beginItemContainer.ReloadData();
                    _beginItemContainer = null;
                }
            }
        }
    }

    public override void _Process(float delta)
    {
        RefreshHeldItemPos();
        if (Input.IsActionJustPressed("inventory"))
        {
            if (_techFlag)
            {
                OnEscapePress();
            }
            else
            {
                _techFlag = true;
            }
        }
    }

    public void RefreshOffset()
    {
        _heldItemOffset = _beginItemContainer.ItemIcon.GetLocalMousePosition();
    }

    public void RefreshHeldItemPos()
    {
        _heldItemTextureRect.RectPosition = _guiManager.GetLocalMousePosition() - _heldItemOffset;
    }

    public override void OnPushEvent(bool pressed) // Use item
    {
        if (_beginItemContainer == null)
        {
            ItemContainer currentContainer = Singletons.MainManager.GameManager.GUIManager.Inventory.GetCurrentItemContainer();
            if (!pressed && currentContainer != null && currentContainer.ItemId != null)
            {
                Logger.Log($"Using item from: {currentContainer.ToString()}");
                Singletons.MainManager.GameManager.GUIManager.PopState();
                Singletons.MainManager.GameManager.Player.GrabItem(currentContainer.ItemId);
            }
        }
    }
}
