using Godot;
using SpaceShooter.Data;
using SpaceShooter.Entities.Ships;
using SpaceShooter.Managers;
using System;

public class ChooseBattleControl : Control
{
    public OptionButton PlayerShipOptions { get; private set; }
    public ColorPickerButton PlayerColor { get; private set; }
    public SpinBox PlayerAICount { get; private set; }
    public ColorPickerButton EnemyColor { get; private set; }
    public SpinBox EnemyAICount { get; private set; }

    public override void _Ready()
    {
        PlayerShipOptions = GetNode<OptionButton>("Cent/PanelContainer/VBoxContainer/Option/PlayerShip");
        PlayerColor = GetNode<ColorPickerButton>("Cent/PanelContainer/VBoxContainer/Option/PlayerColor");
        PlayerAICount = GetNode<SpinBox>("Cent/PanelContainer/VBoxContainer/Option/NumberOfPlayerAI");
        EnemyColor = GetNode<ColorPickerButton>("Cent/PanelContainer/VBoxContainer/Option/EnemyColor");
        EnemyAICount = GetNode<SpinBox>("Cent/PanelContainer/VBoxContainer/Option/NumberOfEnemyAI");

        Input.SetMouseMode(Input.MouseMode.Visible);
        Input.WarpMousePosition(Vector2.Zero);

        Init();
    }

    void Init()
    {
        PlayerShipOptions.Clear();

        Directory dir = new Directory();
        #if TOOLS
            Error err = dir.Open(ShipController.SHIPS_DIRECTORY);
        #else
            Error err = dir.Open(ProjectSettings.GlobalizePath(ShipController.SHIPS_DIRECTORY));
        #endif
        
        if (err != Error.Ok)
        {
            Logger.Log("Couldn't open ships directory: " + err.ToString(), LogLevel.Error);
            MainManager.Instance.ExitGame(ExitCode.DirectoryCouldNotBeOpened);
            return;
        }
        
        err = dir.ListDirBegin(true, true);
        if (err != Error.Ok)
        {
            Logger.Log("Couldn't list ships directory: " + err.ToString(), LogLevel.Error);
            MainManager.Instance.ExitGame(ExitCode.DirectoryListFailed);
            return;
        }

        string ship = string.Empty;
        while (!(ship = dir.GetNext()).Empty())
        {
            PlayerShipOptions.AddItem(ship);
        }
    }

    void OnPlayButtonPressed()
    {
        PackedScene playerShipPackedScene = ResourceLoader.Load<PackedScene>(ShipController.SHIPS_DIRECTORY + "/" + PlayerShipOptions.GetItemText(PlayerShipOptions.Selected) + "/" + PlayerShipOptions.GetItemText(PlayerShipOptions.Selected) + ShipController.SHIPS_EXT);

        Color playerColor = PlayerColor.Color;
        Color enemyColor = EnemyColor.Color;
        int playerAiCount = Convert.ToInt32(PlayerAICount.Value);
        int enemyAiCount = Convert.ToInt32(EnemyAICount.Value);

        GameConfiguration gameConfig = new GameConfiguration(200, new Color[] { playerColor, enemyColor });

        gameConfig.AddPlayerShipConfiguration(new ShipConfiguration(playerShipPackedScene, 0) { PrimaryWeaponPackedScene = ResourceLoader.Load<PackedScene>("res://Resources/Weapons/LaserGun/LaserGun.res") });
        // gameConfig.AddPlayerShipConfiguration(new ShipConfiguration(playerShipPackedScene, 0, playerColor));

        for (int i = 0; i < playerAiCount; i++)
        {
            gameConfig.AddAIShipConfiguration(new ShipConfiguration(playerShipPackedScene, 0) { PrimaryWeaponPackedScene = ResourceLoader.Load<PackedScene>("res://Resources/Weapons/LaserGun/LaserGun.res") });
            // gameConfig.AddAIShipConfiguration(new ShipConfiguration(playerShipPackedScene, 0, playerColor));
        }

        for (int i = 0; i < enemyAiCount; i++)
        {
            gameConfig.AddAIShipConfiguration(new ShipConfiguration(playerShipPackedScene, 1) { PrimaryWeaponPackedScene = ResourceLoader.Load<PackedScene>("res://Resources/Weapons/LaserGun/LaserGun.res") });
            // gameConfig.AddAIShipConfiguration(new ShipConfiguration(playerShipPackedScene, 1, enemyColor));
        }

        MainManager.Instance.GameConfiguration = gameConfig;
        MainManager.Instance.GoToBattleScene();
    }
}
