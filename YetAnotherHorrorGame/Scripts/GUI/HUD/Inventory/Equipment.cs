using Godot;
using HorrorGame.GUI.HUD.Inventory.Views;
using HorrorGame.Managers;
using System;

namespace HorrorGame.GUI.HUD.Inventory
{
    public enum View
    {
        Nothing,
        Items,
        Notes,
        Diaries,
        ToDo
    }

    public class Equipment : CenterContainer
    {
        [Export] NodePath ContentControlNodePath { get; set; }
        public Control Content { get { return GetNode<Control>(ContentControlNodePath); } }

        View _currentView;
        IInventoryView _currentIInventoryView = null;

        public override void _Process(float delta)
        {
            base._Process(delta);
            
            if (_currentView != View.Nothing)
            {
                _currentIInventoryView.Process(delta);
                if (Input.IsActionJustPressed("ui_cancel"))
                {
                    _currentIInventoryView.UiCancel();
                }
            }
        }

        public IInventoryView GetView(View view)
        {
            return Content.GetNode(view.ToString()) as IInventoryView;
        }

        public void OpenView(View view, object[] data)
        {
            if (_currentView != View.Nothing)
            {
                _currentIInventoryView.Exit();
            }

            _currentView = view;
            _currentIInventoryView = GetView(_currentView);

            _currentIInventoryView.Init(data);
            
        }

        public void CloseView()
        {
            if (_currentIInventoryView == null)
                return;
            _currentIInventoryView.Exit();
            _currentView = View.Nothing;
            _currentIInventoryView = null;
        }

        void OnItemsPressed()
        {
            if (_currentView == View.Items)
                return;
            OpenView(View.Items, null);
        }

        void OnNotesPressed()
        {
            if (_currentView == View.Notes)
                return;
            OpenView(View.Notes, null);
        }

        void OnDiariesPressed()
        {
            if (_currentView == View.Diaries)
                return;
            OpenView(View.Diaries, null);
        }

        void OnToDoPressed()
        {
            if (_currentView == View.ToDo)
                return;
        }
    }
}