using Godot;
using HorrorGame.GUI.HUD.Inventory;
using HorrorGame.Managers;
using System;

namespace HorrorGame.GUI.HUD
{
    public class HUDController : Control
    {
        public Pointer Pointer { get; private set; }
        public InventoryController InventoryController { get; private set; }

        public override void _Ready()
        {
            Pointer = GetNode<Pointer>("Pointer");
            InventoryController = GetNode<InventoryController>("InventoryController");
        }

        public override void _ExitTree()
        {
            Pointer = null;
            InventoryController = null;
        }

        public void ShowInventory(View view, object[] data)
        {
            Logger.Log("Show Inventory: " + view.ToString());
            Pointer.Visible = false;
            InventoryController.ShowInventory(view, data);
            
            // InventoryController.Visible = true;
        }

        public void HideInventory()
        {
            Logger.Log("Hide Inventory");
            Pointer.Visible = true;
            InventoryController.HideInventory();
        }
    }
}