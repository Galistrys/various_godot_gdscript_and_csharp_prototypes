using Godot;
using HorrorGame.Data;
using HorrorGame.Data.Items;
using HorrorGame.Managers;
using System;

namespace HorrorGame.GUI.HUD.Inventory.Views
{
    public class ItemsView : VBoxContainer, IInventoryView
    {
		[Export] NodePath _pathToGridContainer { get; set; }
		GridContainer _gridContainer;

		[Export] NodePath ItemNameNodePath { get; set; }
		[Export] NodePath ItemDescNodePath { get; set; }

		Label _itemNameLabel;
		RichTextLabel _itemDescRichTextLabel;

		PackedScene _inventoryItemScene;

		int _currentItem = -1;
		int _selectedItem = -1;

        public override void _Ready()
        {
			_gridContainer = GetNode<GridContainer>(_pathToGridContainer);
			_inventoryItemScene = ResourceLoader.Load<PackedScene>("res://Resources/GUI/HUD/Inventory/InventoryItem.res");

			_itemNameLabel = GetNode<Label>(ItemNameNodePath);
			_itemDescRichTextLabel = GetNode<RichTextLabel>(ItemDescNodePath);
        }

        public void Init(object[] data)
        {
			int count = 0;
			foreach (var itemId in GameSaveManager.CurrentGameSave.GetItems())
			{
				InventoryItem item = _inventoryItemScene.Instance() as InventoryItem;
				item.Configure(itemId);
				_gridContainer.AddChild(item);

				item.Connect("mouse_entered", this, nameof(MouseEnteredItem), new Godot.Collections.Array() { count });
				item.Connect("mouse_exited", this, nameof(MouseExitedItem));
				count++;
			}

            this.Visible = true;
        }

		void MouseEnteredItem(int idx)
		{
			ItemData data = Database.Instance.GetItemData(GameSaveManager.CurrentGameSave.GetItem(idx));
			_itemNameLabel.Text = data.Name;
			_itemDescRichTextLabel.BbcodeText = TranslationServer.Translate(data.Description).Replace("<<newline>>", "\n");
			Logger.Log($"MouseEntered: {idx}");
			_currentItem = idx;
		}

		void MouseExitedItem()
		{
			_itemNameLabel.Text = "";
			_itemDescRichTextLabel.Clear();
			Logger.Log("MouseExited");
			_currentItem = -1;
		}

		InventoryItem GetInventoryItem(int idx)
		{
			if (_gridContainer.GetChildCount() == 0)
				return null;
			return _gridContainer.GetChild<InventoryItem>(idx);
		}

		public void Exit()
        {
            this.Visible = false;

			foreach (Node node in _gridContainer.GetChildren())
			{
				node.QueueFree();
			}

			_itemNameLabel.Text = "";
			_itemDescRichTextLabel.Clear();

			_currentItem = -1;
			_selectedItem = -1;
        }

        public void Process(float delta)
        {
			if (Input.IsActionJustPressed("inventory"))
			{
				if (_selectedItem != -1)
				{
					Logger.Log($"Close inventory -> Resetting");
					GetInventoryItem(_selectedItem).Reset();
					_selectedItem = -1;
				}
				MainManager.Instance.GameManager.CloseInventory();
			}

			if (_selectedItem == -1)
			{
				if (Input.IsActionJustPressed("interact") && _currentItem != -1)
				{
					_selectedItem = _currentItem;
					GetInventoryItem(_selectedItem).Highlight();
					Logger.Log($"Selected item: {_selectedItem}");
				}
				else if (Input.IsActionJustPressed("aim") && _currentItem != -1)
				{
					MainManager.Instance.GameManager.PlayerController.PushState(new HorrorGame.Player.PlayerStates.HoldingItemPlayerState(GameSaveManager.CurrentGameSave.GetItem(_currentItem)));
					MainManager.Instance.GameManager.CloseInventory();
				}
			}
			else
			{
				if (Input.IsActionJustPressed("interact"))
				{
					if (_currentItem == -1)
					{
						Logger.Log($"Selected nothing -> Resetting");
						GetInventoryItem(_selectedItem).Reset();
						_selectedItem = -1;
					}
					else if (_currentItem == _selectedItem)
					{
						Logger.Log($"Selected same item: {_selectedItem} -> Resetting");
						GetInventoryItem(_selectedItem).Reset();
						_selectedItem = -1;
					}
					else
					{
						Logger.Log($"Selected different item: {_selectedItem}");
						ItemData data = Database.Instance.GetItemData(GameSaveManager.CurrentGameSave.GetItem(_selectedItem));
						string result = data.CombineWith(GameSaveManager.CurrentGameSave.GetItem(_currentItem));

						if (result == null)
						{
							Logger.Log($"Cannot combine -> Resetting");
							GetInventoryItem(_selectedItem).Reset();
							_selectedItem = -1;
						}
						else
						{
							Logger.Log($"Combining -> Resetting");
							if (_selectedItem > _currentItem)
							{
								GameSaveManager.CurrentGameSave.RemoveItem(_selectedItem);
								GameSaveManager.CurrentGameSave.RemoveItem(_currentItem);
							}
							else
							{
								GameSaveManager.CurrentGameSave.RemoveItem(_currentItem);
								GameSaveManager.CurrentGameSave.RemoveItem(_selectedItem);
							}

							GameSaveManager.CurrentGameSave.AddItem(result);
							_selectedItem = -1;

							Exit();
							Init(null);
						}
					}
				}
			}
        }

        public void UiCancel()
        {
            if (_selectedItem == -1)
			{
				MainManager.Instance.GameManager.CloseInventory();
			}
			else
			{
				Logger.Log($"UI Cancel -> Resetting");
				GetInventoryItem(_selectedItem).Reset();
				_selectedItem = -1;
			}
        }
    }
}