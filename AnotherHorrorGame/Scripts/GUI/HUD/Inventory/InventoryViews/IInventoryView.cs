using Godot;
using System;

namespace HorrorGame.GUI.HUD.Inventory.Views
{
	public interface IInventoryView
	{
		void Init(object[] data);
		void Exit();
		void Process(float delta);
        void UiCancel();
    }
}