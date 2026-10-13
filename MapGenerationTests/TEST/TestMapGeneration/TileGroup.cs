using Godot;
using System;
using System.Collections.Generic;

public class TileGroup
{
    public TileGroupType Type { get; set; }
    public List<Tile> Tiles { get; private set; }

    public TileGroup()
    {
        Tiles = new List<Tile>();
    }
}
