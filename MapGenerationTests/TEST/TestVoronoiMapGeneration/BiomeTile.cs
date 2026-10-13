using Godot;
using System;

public class BiomeTile
{
    public int biomeType = 0;
    public bool isWater = true;

    public Vector2 pos = Vector2.Zero;

    public BiomeTile Top;
    public BiomeTile Bottom;
    public BiomeTile Left;
    public BiomeTile Right;

    public BiomeTile()
    {

    }
}
