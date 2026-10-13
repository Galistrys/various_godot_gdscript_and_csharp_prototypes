using Godot;
using HorrorGame.Data;
using HorrorGame.Data.Items;
using HorrorGame.Managers;
using System;

namespace HorrorGame.GUI.HUD.Inventory
{
	public class InventoryItem : Panel
	{
		static string _pathToIcon = "MarginContainer/VBoxContainer/ItemIcon";
		static string _pathToLabel = "MarginContainer/VBoxContainer/ItemName";

		TextureRect ItemIcon { get { return GetNode<TextureRect>(_pathToIcon); } }
		Label ItemName { get { return GetNode<Label>(_pathToLabel); } }

		public void Configure(string itemId)
		{
			Logger.Log("Inventory item configure: " + itemId, LogLevel.Info);
			var itemData = Database.Instance.GetItemData(itemId);

			ItemIcon.Texture = ItemData.GetItemTexture(itemId);
			ItemName.Text = itemData.Name;
		}

		public void Highlight()
		{
			SelfModulate = new Color(0.75f, 0.75f, 0.75f);
		}

		public void Reset()
		{
			SelfModulate = Colors.White;
		}
	}
}