using Godot;
using System;

public class GameManager : Node
{
    public TerrainManager TerrainManager;

    public ProgressBar progressBar;

    public override void _Ready()
    {
        SINGLETONS.MainManager.GameManager = this;
        TerrainManager = (TerrainManager)FindNode("TerrainManager");
        progressBar = GetNode<ProgressBar>("LoadCanvas/CentCont/VBox/Bar");

        TerrainManager.GenerateWorld();
    }
}
