using Godot;
using System;

public partial class GameManager : Node
{
    public GameManager()
    {
        var noise = ResourceLoader.Load<FastNoiseLite>("res://voxel_terrain/biomes/default_noise.res");

        MeshUtils._noise = noise;
        BlocksDatabase.Instance.LoadData();
    }
}
