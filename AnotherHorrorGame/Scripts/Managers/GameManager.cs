using Godot;
using HorrorGame.GUI;
using HorrorGame.GUI.HUD;
using HorrorGame.GUI.HUD.Inventory;
using HorrorGame.GUI.PauseMenu;
using HorrorGame.Player;
using HorrorGame.Tools;
using System;

namespace HorrorGame.Managers
{
    public class GameManager : Node
    {
        public LevelManager LevelManager { get; private set; }
        public LoadingOverlay LoadingOverlay { get; private set; }
        public HUDController HUDController { get; private set; }
        public PauseMenuController PauseMenuController { get; private set; }

        public PlayerController PlayerController { get { return GetNode<PlayerController>("World/Player"); } }

        public override void _Ready()
        {
            LevelManager = GetNode<LevelManager>("World/LevelManager");
            LoadingOverlay = GetNode<LoadingOverlay>("LoadingOverlay");
            HUDController = GetNode<HUDController>("HUDController");
            PauseMenuController = GetNode<PauseMenuController>("PauseMenuController");

            ChangeLevel(GameSaveManager.CurrentGameSave.CurrentMapId);
        }

        public override void _ExitTree()
        {
            LevelManager = null;
            LoadingOverlay = null;
            HUDController = null;
            PauseMenuController = null;
        }

        public async void ChangeLevel(string newLevelId)
        {
            Logger.Log($"Changing level to: {newLevelId}.", LogLevel.Info);
            Logger.Log("Pause game.");
            GetTree().Paused = true;

            if (!LoadingOverlay.Active)
            {
                Logger.Log("Show overlay.");
                LoadingOverlay.ShowLoading();
            }

            await ToSignal(LoadingOverlay, nameof(LoadingOverlay.BackgroundShown));

            Logger.Log($"Changing scene.");
            LevelManager.CallDeferred(nameof(LevelManager.ChangeLevel), newLevelId);
        }

        public void ChangeLevelFinished(string newLevelId)
        {
            Logger.Log("Change level process finished.");
            
            LoadingOverlay.OnLoadingComplete();
            GameSaveManager.CurrentGameSave.CurrentMapId = newLevelId;
        }

        void OnLoadingOverlayBackgroundHidden()
        {
            Logger.Log("Resume game.");
            GetTree().Paused = false;
        }

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventKey iek)
            {
                if (iek.Pressed && iek.Scancode == (int)KeyList.K)
                {
                    LevelManager.SaveLevelData();
                }
            }
        }

        public async void OpenPauseMenu()
        {
            Logger.Log("Open pause menu.", LogLevel.Info);
            GetTree().Paused = true;
            await ToSignal(GetTree(), "idle_frame");
            Input.SetMouseMode(Input.MouseMode.Visible);
            PauseMenuController.ShowPauseMenu();
        }

        public async void ClosePauseMenu()
        {
            Logger.Log("Close pause menu.", LogLevel.Info);
            PauseMenuController.ClosePauseMenu();
            Input.SetMouseMode(Input.MouseMode.Captured);
            await ToSignal(GetTree(), "idle_frame");
            GetTree().Paused = false;
        }

        public async void OpenInventory(View view, object[] data)
        {
            if (HUDController.InventoryController.Visible || HUDController.InventoryController.Busy)
                return;

            Logger.Log("Open inventory.", LogLevel.Info);

            GetTree().Paused = true;
            await ToSignal(GetTree(), "idle_frame");
            // Input.SetMouseMode(Input.MouseMode.Visible);
            HUDController.ShowInventory(view, data);
        }

        public async void CloseInventory()
        {
            if (!HUDController.InventoryController.Visible || HUDController.InventoryController.Busy)
                return;
            
            Logger.Log("Close inventory.", LogLevel.Info);
            HUDController.HideInventory();
            // Input.SetMouseMode(Input.MouseMode.Captured);
            await ToSignal(HUDController.InventoryController, nameof(InventoryController.ContentHidden));
            await ToSignal(GetTree(), "idle_frame");
            // Logger.Log("Inventory's content hidden.");
            GetTree().Paused = false;
            
            await ToSignal(HUDController.InventoryController, nameof(InventoryController.AnimationFinished));
            HUDController.InventoryController.Visible = false;
        }

        public void GoToMainMenu()
        {
            GetTree().ChangeScene("res://Resources/Scenes/MainMenu.scn");
            GetTree().Paused = false;
        }
    }
}