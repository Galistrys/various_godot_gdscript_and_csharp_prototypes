using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class TestVoronoiMapGeneration : Control
{
    public const int BORDER_BUFFER = 2;

    SpinBox _mapSizeSpinBox;
    SpinBox _gridSizeSpinBox;

    TextureRect _waterAndLandTextureRect;
    TextureRect _biomesTextureRect;

    int MapSize { get { return (int)Math.Floor(_mapSizeSpinBox.Value); } }
    int GridSize { get { return (int)Math.Floor(_gridSizeSpinBox.Value); } }

    // readonly int[,] BiomesData = new int[,] // [ BIOME, [ START_POINTS_COUNT, PATCH_SIZE ] ]
    // {
    //     // Desert
    //     { 0, 0 },
    //     // Savanna
    //     { 0, 0 },
    //     // Plains
    //     { 0, 0 },
    //     // Grassland
    //     { 0, 0 },
    //     // Leaf Forest
    //     { 0, 0 },
    //     // Confierous Forest
    //     { 0, 0 },
    //     // Mixed Forest
    //     { 0, 0 },
    //     // Dark Forest
    //     { 0, 0 },
    //     // Swamps
    //     { 0, 0 },
    //     // Rocks
    //     { 0, 0 },
    //     // Snow
    //     { 0, 0 },
    //     // Dead Land
    //     { 0, 0 }
    // };

    [Export]
    public string[] BiomesNames = new string[]
    {
        "Lake",
        "Desert",
        "Savanna",
        "Plains",
        "Grassland",
        "Leaf Forest",
        "Confierous Forest",
        "Mixed Forest",
        "Dark Forest",
        "Swamps",
        "Rocks",
        "Snow",
        "Dead Land"
    };

    [Export]
    public int[] BiomesStartPointsCount = new int[]
    {
        4, // Lake
        3, // Desert
        4, // Savanna
        5, // Plains
        5, // Grassland
        5, // Leaf Forest
        5, // Confierous Forest
        5, // Mixed Forest
        2, // Dark Forest
        3, // Swamps
        5, // Rocks
        3, // Snow
        1  // Dead Land
    };

    [Export]
    public int[] BiomesPatchSizes = new int[]
    {
        4, // Lake
        4, // Desert
        5, // Savanna
        6, // Plains
        6, // Grassland
        5, // Leaf Forest
        5, // Confierous Forest
        5, // Mixed Forest
        4, // Dark Forest
        4, // Swamps
        4, // Rocks
        4, // Snow
        3  // Dead Land
    };

    [Export]
    public Color[] BiomesColors = new Color[]
    {
        Colors.Cyan, // Lake
        new Color("ECDCB8"), // Desert
        new Color("B1CC67"), // Savanna
        new Color("94DC6A"), // Plains
        new Color("2ECC71"), // Grassland
        new Color("38B328"), // Leaf Forest
        new Color("28B36F"), // Confierous Forest
        new Color("28B348"), // Mixed Forest
        new Color("114E2A"), // Dark Forest
        new Color("1C8345"), // Swamps
        new Color("CCCCCC"), // Rocks
        Colors.White, // Snow
        new Color("816E5A")  // Dead Land
    };

    public override void _Ready()
    {
        _mapSizeSpinBox = GetNode<SpinBox>("HBoxContainer/PanelContainer/VBoxContainer/SizeSpinBox");
        _gridSizeSpinBox = GetNode<SpinBox>("HBoxContainer/PanelContainer/VBoxContainer/GridSizeSpinBox");

        _waterAndLandTextureRect = GetNode<TextureRect>("HBoxContainer/ScrollContainer/PanelContainer/GridContainer/WaterAndLand");
        _biomesTextureRect = GetNode<TextureRect>("HBoxContainer/ScrollContainer/PanelContainer/GridContainer/Biomes");

        OS.WindowMaximized = true;
    }

    void OnGenerateButtonPressed()
    {
        GD.Randomize();

        GenerateMap();
    }

    void GenerateMap()
    {
        _waterAndLandTextureRect.Texture = null;
        _biomesTextureRect.Texture = null;

        int mapSize = MapSize;
        int gridSize = GridSize + BORDER_BUFFER * 2;
        int cellSize = mapSize / gridSize;
        int pointsCount = gridSize * gridSize;

        BiomeTile[] tiles = new BiomeTile[pointsCount];

        GD.Print("Generate Tiles...");
        GenerateTiles(mapSize, gridSize, cellSize, pointsCount, tiles);

        // GD.Print("Make it more waterish...");
        // MakeItMoreWaterish(mapSize, gridSize, ref tiles);

        GD.Print("Populating Biomes...");
        PopulateBiomes_OLD(tiles);
        // PopulateBiomes(tiles, mapSize);
        
        GD.Print("Drawing...");
        DrawWaterAndLand(mapSize, tiles);
        DrawBiomes(mapSize, tiles);
        GD.Print("Drawing done!");
    }

    void GenerateTiles(int mapSize, int gridSize, int cellSize, int pointsCount, BiomeTile[] tiles)
    {
        for (int i = 0; i < pointsCount; i++)
        {
            int cellX = 0;
            while (cellX < gridSize)
            {
                int cellY = 0;
                while (cellY < gridSize)
                {
                    int currentIdx = cellX * gridSize + cellY;
                    int posx = Mathf.FloorToInt((float)GD.RandRange(0, cellSize)) + cellX * cellSize;
                    int posy = Mathf.FloorToInt((float)GD.RandRange(0, cellSize)) + cellY * cellSize;
                    
                    // float distanceToBorder = Mathf.Min(Mathf.Min(posx, posy), Mathf.Min(mapSize - posx - 1, mapSize - posy - 1)) / ((float)mapSize / 2);
                    // float random = GD.Randf();
                    // float waterThreshold = Mathf.Pow(distanceToBorder - 1, 6);

                    // float distanceToCenter = new Vector2(posx, posy).DistanceTo(new Vector2(mapSize / 2, mapSize / 2));
                    // float d = Mathf.Max(1 - (distanceToCenter / (mapSize / 2.0f)), 0);
                    // float random = GD.Randf();
                    // float waterThreshold = Mathf.Pow(d - 1, 6);

                    float distanceToCenter = new Vector2(posx, posy).DistanceTo(new Vector2(mapSize / 2, mapSize / 2));

                    tiles[currentIdx] = new BiomeTile()
                    {
                        pos = new Vector2(posx, posy),
                        // isWater = cellX < BORDER_BUFFER || cellY < BORDER_BUFFER || cellX > gridSize - BORDER_BUFFER || cellY > gridSize - BORDER_BUFFER || random < waterThreshold,
                        isWater = cellX < BORDER_BUFFER || cellY < BORDER_BUFFER || cellX > gridSize - BORDER_BUFFER || cellY > gridSize - BORDER_BUFFER || distanceToCenter > mapSize / 2,
                        biomeType = -1
                    };

                    cellY++;
                }
                cellX++;
            }
        }

        for (int i = 0; i < tiles.Length; i++)
        {
            var curTile = tiles[i];
            int x = GetXFromGridIdx(i, gridSize);
            int y = GetYFromGridIdx(i, gridSize);

            curTile.Top = GetTile(x, y - 1, gridSize, tiles);
            curTile.Bottom = GetTile(x, y + 1, gridSize, tiles);
            curTile.Left = GetTile(x - 1, y, gridSize, tiles);
            curTile.Right = GetTile(x + 1, y, gridSize, tiles);
        }
    }

    BiomeTile GetTile(int gridX, int gridY, int gridSize, BiomeTile[] tiles)
    {
        if (gridX < 0 || gridY < 0 || gridX >= gridSize || gridY >= gridSize)
            return null;
        
        int idx = gridX * gridSize + gridY;
        return tiles[idx];
    }

    int GetXFromGridIdx(int gridIdx, int gridSize)
    {
        return gridIdx / gridSize;
    }

    int GetYFromGridIdx(int gridIdx, int gridSize)
    {
        return gridIdx % gridSize;
    }

    void MakeItMoreWaterish(int mapSize, int gridSize, ref BiomeTile[] tiles)
    {
        Random rand = new Random();
        for (int k = 0; k < 0; k++)
        {
            var oldTiles = tiles;
            tiles = new BiomeTile[oldTiles.Length];
            for (int i = 0; i < tiles.Length; i++)
            {
                var tile = oldTiles[i];
                var newTile = new BiomeTile()
                {
                    pos = tile.pos,
                    isWater = tile.isWater,
                    biomeType = tile.biomeType
                };

                if (!tile.isWater
                    && (tile.Top != null && tile.Top.isWater
                        || tile.Bottom != null && tile.Bottom.isWater
                        || tile.Left != null && tile.Left.isWater
                        || tile.Right != null && tile.Right.isWater)
                    )
                {
                    float distanceToCenter = tile.pos.DistanceTo(new Vector2(mapSize / 2, mapSize / 2));
                    float d = 1 - distanceToCenter / (mapSize / 2.0f);
                    double random = rand.NextDouble();
                    float waterThreshold = Mathf.Abs(Mathf.Pow(d - 1, 4));

                    if (random < waterThreshold)
                        newTile.isWater = true;
                }

                tiles[i] = newTile;
            }

            for (int i = 0; i < tiles.Length; i++)
            {
                var curTile = tiles[i];
                int x = GetXFromGridIdx(i, gridSize);
                int y = GetYFromGridIdx(i, gridSize);

                curTile.Top = GetTile(x, y - 1, gridSize, tiles);
                curTile.Bottom = GetTile(x, y + 1, gridSize, tiles);
                curTile.Left = GetTile(x - 1, y, gridSize, tiles);
                curTile.Right = GetTile(x + 1, y, gridSize, tiles);
            }
        }
    }

    void PopulateBiomes(BiomeTile[] tiles, int mapSize)
    {
        Vector2 mapCenter = new Vector2(mapSize / 2, mapSize / 2);
        var random = new Random();
        var biomeGroups = new List<List<BiomeTile>>();
        var tilesLeft = new List<BiomeTile>();
        for (int i = 0; i < tiles.Length; i++)
        {
            if (!tiles[i].isWater)
                tilesLeft.Add(tiles[i]);
        }

        for (int biomeType = 0; biomeType < BiomesStartPointsCount.Length; biomeType++)
        {
            for (int i = 0; i < BiomesStartPointsCount[biomeType]; i++)
            {
                var tile = tilesLeft[random.Next(tilesLeft.Count)];
                tilesLeft.Remove(tile);
                tile.biomeType = biomeType;
                biomeGroups.Add(new List<BiomeTile>() { tile });
            }
        }

        int maxPatchSize = BiomesPatchSizes.Max();
        int last = -1;
        while (tilesLeft.Count > 0)
        {
            if (tilesLeft.Count == last)
            {
                foreach (var tile in tilesLeft)
                {
                    tile.isWater = true;
                }
                break;
            }
            last = tilesLeft.Count;

            GD.Print($"Tiles left: {tilesLeft.Count}");
            for (int biomeGroupIdx = 0; biomeGroupIdx < biomeGroups.Count; biomeGroupIdx++)
            {
                var biomeGroup = biomeGroups[biomeGroupIdx];
                for (int iteration = 0; iteration < BiomesPatchSizes[biomeGroup[0].biomeType]; iteration++)
                {
                    var closestToCenter = biomeGroup[0];
                    float min_dist = closestToCenter.pos.DistanceTo(mapCenter);
                    for (int i = 0; i < biomeGroup.Count; i++)
                    {
                        var cur = biomeGroup[i];
                        float dist = cur.pos.DistanceTo(mapCenter);
                        if (dist < min_dist)
                        {
                            closestToCenter = cur;
                            min_dist = dist;
                        }
                    }

                    Vector2 dirToCenter = closestToCenter.pos.DirectionTo(mapCenter);
                    float r_val = Mathf.Max(0, mapSize / 2 - min_dist);
                    float rc_val = Mathf.Pow(r_val - 1, 4);
                    float angl_add = GD.Randf() * Mathf.Pi * rc_val;
                    Vector2 res = Vector2.Zero;

                    BiomeTile target = null;
                    if (random.Next(2) == 0)
                        res = dirToCenter.Rotated(angl_add);
                    else
                        res = dirToCenter.Rotated(-angl_add);
                    
                    if (Mathf.Abs(res.x) > Mathf.Abs(res.y))
                    {
                        if (res.x > 0)
                        {
                            target = closestToCenter.Right;
                        }
                        else
                        {
                            target = closestToCenter.Left;
                        }
                    }
                    else
                    {
                        if (res.y > 0)
                        {
                            target = closestToCenter.Bottom;
                        }
                        else
                        {
                            target = closestToCenter.Top;
                        }
                    }

                    if (target.biomeType == -1)
                    {
                        target.biomeType = closestToCenter.biomeType;
                        biomeGroup.Add(target);
                    }
                    else
                    {
                        bool foundFlag = false;
                        for (int biomeTileInGroup = 0; biomeTileInGroup < biomeGroup.Count && !foundFlag; biomeTileInGroup++)
                        {
                            var tile = biomeGroup[biomeTileInGroup];

                            if (tile.Top != null && !tile.Top.isWater && tile.Top.biomeType == -1)
                            {
                                tile.Top.biomeType = tile.biomeType;
                                tilesLeft.Remove(tile.Top);
                                biomeGroup.Add(tile.Top);
                                foundFlag = true;
                            }
                            else if (tile.Bottom != null && !tile.Bottom.isWater && tile.Bottom.biomeType == -1)
                            {
                                tile.Bottom.biomeType = tile.biomeType;
                                tilesLeft.Remove(tile.Bottom);
                                biomeGroup.Add(tile.Bottom);
                                foundFlag = true;
                            }
                            else if (tile.Left != null && !tile.Left.isWater && tile.Left.biomeType == -1)
                            {
                                tile.Left.biomeType = tile.biomeType;
                                tilesLeft.Remove(tile.Left);
                                biomeGroup.Add(tile.Left);
                                foundFlag = true;
                            }
                            else if (tile.Right != null && !tile.Right.isWater && tile.Right.biomeType == -1)
                            {
                                tile.Right.biomeType = tile.biomeType;
                                tilesLeft.Remove(tile.Right);
                                biomeGroup.Add(tile.Right);
                                foundFlag = true;
                            }
                        }
                    }

                    
                }
            }
        }
    }

    void PopulateBiomes_OLD(BiomeTile[] tiles)
    {
        var random = new Random();
        var biomeGroups = new List<List<BiomeTile>>();
        var tilesLeft = new List<BiomeTile>();
        for (int i = 0; i < tiles.Length; i++)
        {
            if (!tiles[i].isWater)
                tilesLeft.Add(tiles[i]);
        }

        for (int biomeType = 0; biomeType < BiomesStartPointsCount.Length; biomeType++)
        {
            for (int i = 0; i < BiomesStartPointsCount[biomeType]; i++)
            {
                var tile = tilesLeft[random.Next(tilesLeft.Count)];
                tilesLeft.Remove(tile);
                tile.biomeType = biomeType;
                biomeGroups.Add(new List<BiomeTile>() { tile });
            }
        }

        int maxPatchSize = BiomesPatchSizes.Max();
        int last = -1;
        while (tilesLeft.Count > 0)
        {
            if (tilesLeft.Count == last)
            {
                foreach (var tile in tilesLeft)
                {
                    tile.isWater = true;
                }
                break;
            }
            last = tilesLeft.Count;
            GD.Print($"Tiles left: {tilesLeft.Count}");
            for (int biomeGroupIdx = 0; biomeGroupIdx < biomeGroups.Count; biomeGroupIdx++)
            {
                // GD.Print($"\tBiome group idx: {biomeGroupIdx + 1}/{biomeGroups.Count}");
                var biomeGroup = biomeGroups[biomeGroupIdx];
                for (int i = 0; i < BiomesPatchSizes[biomeGroup[0].biomeType]; i++)
                {
                    // GD.Print($"\t\tI: {i + 1}/{BiomesPatchSizes[biomeGroup[0].biomeType]}");
                    bool foundFlag = false;
                    for (int j = 0; j < biomeGroup.Count && !foundFlag; j++)
                    {
                        // GD.Print($"\t\tTile in biome group: {j + 1}/{biomeGroup.Count}");
                        var tile = biomeGroup[j];

                        if (tile.Top != null && !tile.Top.isWater && tile.Top.biomeType == -1)
                        {
                            tile.Top.biomeType = tile.biomeType;
                            tilesLeft.Remove(tile.Top);
                            biomeGroup.Add(tile.Top);
                            foundFlag = true;
                        }
                        else if (tile.Bottom != null && !tile.Bottom.isWater && tile.Bottom.biomeType == -1)
                        {
                            tile.Bottom.biomeType = tile.biomeType;
                            tilesLeft.Remove(tile.Bottom);
                            biomeGroup.Add(tile.Bottom);
                            foundFlag = true;
                        }
                        else if (tile.Left != null && !tile.Left.isWater && tile.Left.biomeType == -1)
                        {
                            tile.Left.biomeType = tile.biomeType;
                            tilesLeft.Remove(tile.Left);
                            biomeGroup.Add(tile.Left);
                            foundFlag = true;
                        }
                        else if (tile.Right != null && !tile.Right.isWater && tile.Right.biomeType == -1)
                        {
                            tile.Right.biomeType = tile.biomeType;
                            tilesLeft.Remove(tile.Right);
                            biomeGroup.Add(tile.Right);
                            foundFlag = true;
                        }
                    }
                }
            }
        }
    }

    void DrawWaterAndLand(int mapSize, BiomeTile[] tiles)
    {
        var image = new Image();
        image.Create(mapSize, mapSize, false, Image.Format.Rgb8);

        image.Lock();
        for (int x = 0; x < mapSize; x++)
        {
            GD.Print($"{x}/{mapSize}");
            for (int y = 0; y < mapSize; y++)
            {
                float minDistance = Mathf.Inf;
                int bestIdx = -1;
                for (int i = 0; i < tiles.Length; i++)
                {
                    var tile = tiles[i];
                    float curDistance = new Vector2(x, y).DistanceSquaredTo(tile.pos);
                    if (curDistance < minDistance)
                    {
                        minDistance = curDistance;
                        bestIdx = i;
                    }
                }

                var bestTile = tiles[bestIdx];
                if (bestTile.isWater || bestTile.biomeType == -1)
                    image.SetPixel(x, y, Colors.Aqua);
                else
                    image.SetPixel(x, y, Colors.Bisque);
            }
        }
        image.Unlock();

        var texture = new ImageTexture();
        texture.CreateFromImage(image, 0);
        _waterAndLandTextureRect.Texture = texture;
    }

    void DrawBiomes(int mapSize, BiomeTile[] tiles)
    {
        var image = new Image();
        image.Create(mapSize, mapSize, false, Image.Format.Rgb8);

        image.Lock();
        for (int x = 0; x < mapSize; x++)
        {
            GD.Print($"{x}/{mapSize}");
            for (int y = 0; y < mapSize; y++)
            {
                float minDistance = Mathf.Inf;
                int bestIdx = -1;
                for (int i = 0; i < tiles.Length; i++)
                {
                    var tile = tiles[i];
                    float curDistance = new Vector2(x, y).DistanceSquaredTo(tile.pos);
                    if (curDistance < minDistance)
                    {
                        minDistance = curDistance;
                        bestIdx = i;
                    }
                }

                var bestTile = tiles[bestIdx];
                if (bestTile.isWater || bestTile.biomeType == -1)
                    image.SetPixel(x, y, Colors.Blue);
                else
                    image.SetPixel(x, y, BiomesColors[bestTile.biomeType]);
            }
        }
        image.Unlock();

        var texture = new ImageTexture();
        texture.CreateFromImage(image, 0);
        _biomesTextureRect.Texture = texture;
    }
}
