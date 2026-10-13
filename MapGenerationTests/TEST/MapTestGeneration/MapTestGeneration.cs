using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class MapTestGeneration : Control
{
    public const int BORDER_BUFFER = 2;
    public const float INITIAL_OCEAN_CHANCE = 0.8f;

    SpinBox _mapSizeSpinBox;
    SpinBox _gridSizeSpinBox;

    TextureRect _simpleTextureRect;
    TextureRect _detailedTextureRect;

    int MapSize { get { return (int)Math.Floor(_mapSizeSpinBox.Value); } }
    int GridSize { get { return (int)Math.Floor(_gridSizeSpinBox.Value); } }

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

        _simpleTextureRect = GetNode<TextureRect>("HBoxContainer/ScrollContainer/PanelContainer/GridContainer/SimpleTextureRect");
        _detailedTextureRect = GetNode<TextureRect>("HBoxContainer/ScrollContainer/PanelContainer/GridContainer/DetailedTextureRect");

        OS.WindowMaximized = true;
    }

    void OnGenerateButtonPressed()
    {
        GD.Randomize();

        int gridSize = GridSize;
        int[] tiles = new int[gridSize * gridSize];
        int mapSize = MapSize;

        for (int i = 0; i < tiles.Length; i++)
            tiles[i] = -2;
        
        CreateOcean(ref tiles, gridSize);
        PopulateBiomes(tiles, gridSize);
        var points = GeneratePoints(gridSize, mapSize);
        var colorData = GenerateColorData(tiles, points, gridSize, mapSize);

        GenerateSimpleTexture(tiles, gridSize, mapSize);
        GenerateDetailedTexture(colorData, mapSize);
    }

    void CreateOcean(ref int[] tiles, int gridSize)
    {
        GD.Print("Create Ocean");
        int halfGridSize = gridSize / 2;

        for (int x = 0; x < gridSize; x++)
        {
            tiles[PosToIdx(x, 0, gridSize)] = GD.Randf() < INITIAL_OCEAN_CHANCE ? -1 : -2;
            tiles[PosToIdx(x, gridSize - 1, gridSize)] = GD.Randf() < INITIAL_OCEAN_CHANCE ? -1 : -2;
        }

        for (int y = 0; y < gridSize; y++)
        {
            tiles[PosToIdx(0, y, gridSize)] = GD.Randf() < INITIAL_OCEAN_CHANCE ? -1 : -2;
            tiles[PosToIdx(gridSize - 1, y, gridSize)] = GD.Randf() < INITIAL_OCEAN_CHANCE ? -1 : -2;
        }

        tiles[PosToIdx(0, 0, gridSize)] = -1;
        tiles[PosToIdx(0, gridSize - 1, gridSize)] = -1;
        tiles[PosToIdx(gridSize - 1, 0, gridSize)] = -1;
        tiles[PosToIdx(gridSize - 1, gridSize - 1, gridSize)] = -1;

        int[] oldTiles = tiles;
        for (int i = 0; i < halfGridSize; i++)
        {
            tiles = new int[tiles.Length];
            for (int j = 0; j < tiles.Length; j++)
            {
                tiles[j] = oldTiles[j];
            }

            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    int idx = PosToIdx(x, y, gridSize);

                    if (oldTiles[idx] == -1)
                    {
                        float distance = Mathf.Min(Mathf.Abs(halfGridSize - x), Mathf.Abs(halfGridSize - y));

                        int neighbours = 0;
                        if (x > 0)
                            if (tiles[PosToIdx(x - 1, y, gridSize)] == -1) neighbours++;
                        if (y > 0)
                            if (tiles[PosToIdx(x, y - 1, gridSize)] == -1) neighbours++;
                        if (x < gridSize - 1)
                            if (tiles[PosToIdx(x + 1, y, gridSize)] == -1) neighbours++;
                        if (y < gridSize - 1)
                            if (tiles[PosToIdx(x, y + 1, gridSize)] == -1) neighbours++;
                        if (neighbours == 0)
                            neighbours = 1;

                        if (GD.Randf() < Mathf.Lerp(0.1f, 0.95f, Mathf.Pow((float)distance / halfGridSize, 2) / neighbours))
                        {
                            if (x > 0)
                                tiles[PosToIdx(x - 1, y, gridSize)] = -1;
                            if (y > 0)
                                tiles[PosToIdx(x, y - 1, gridSize)] = -1;
                            if (x < gridSize - 1)
                                tiles[PosToIdx(x + 1, y, gridSize)] = -1;
                            if (y < gridSize - 1)
                                tiles[PosToIdx(x, y + 1, gridSize)] = -1;
                        }
                    }
                }
            }

            oldTiles = tiles;
        }

        // CLEANUP
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                int idx = PosToIdx(x, y, gridSize);

                if (tiles[idx] == -2)
                {
                    int neighbours = 0;
                    if (x > 0)
                        if (tiles[PosToIdx(x - 1, y, gridSize)] == -2) neighbours++;
                    if (y > 0)
                        if (tiles[PosToIdx(x, y - 1, gridSize)] == -2) neighbours++;
                    if (x < gridSize - 1)
                        if (tiles[PosToIdx(x + 1, y, gridSize)] == -2) neighbours++;
                    if (y < gridSize - 1)
                        if (tiles[PosToIdx(x, y + 1, gridSize)] == -2) neighbours++;
                    
                    if (neighbours == 0)
                        tiles[idx] = -1;
                }
            }
        }
    }

    void PopulateBiomes(int[] tiles, int gridSize)
    {
        GD.Print("PopulateBiomes");
        var random = new Random();
        var biomeGroups = new List<List<int>>();
        var tilesLeft = new List<int>();
        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] == -2)
                tilesLeft.Add(i);
        }

        for (int biomeType = 0; biomeType < BiomesStartPointsCount.Length; biomeType++)
        {
            for (int i = 0; i < BiomesStartPointsCount[biomeType]; i++)
            {
                var tileIdx = tilesLeft[random.Next(tilesLeft.Count)];
                tilesLeft.Remove(tileIdx);
                tiles[tileIdx] = biomeType;
                biomeGroups.Add(new List<int>() { tileIdx });
            }
        }

        int maxPatchSize = BiomesPatchSizes.Max();
        int last = -1;

        while (tilesLeft.Count > 0)
        {
            if (tilesLeft.Count == last)
            {
                foreach (var tileIdx in tilesLeft)
                {
                    tiles[tileIdx] = -1;
                }
                break;
            }

            last = tilesLeft.Count;
            GD.Print($"Tiles left: {tilesLeft.Count}");
            for (int biomeGroupIdx = 0; biomeGroupIdx < biomeGroups.Count; biomeGroupIdx++)
            {
                // GD.Print($"\tBiome group idx: {biomeGroupIdx + 1}/{biomeGroups.Count}");
                var biomeGroup = biomeGroups[biomeGroupIdx];
                for (int i = 0; i < BiomesPatchSizes[tiles[biomeGroup[0]]]; i++)
                {
                    // GD.Print($"\t\tI: {i + 1}/{BiomesPatchSizes[tiles[biomeGroup[0]]]}");
                    bool foundFlag = false;
                    for (int j = 0; j < biomeGroup.Count && !foundFlag; j++)
                    {
                        // GD.Print($"\t\tTile in biome group: {j + 1}/{biomeGroup.Count}");
                        var tileIdx = biomeGroup[j];

                        var pos = IdxToPos(tileIdx, gridSize);
                        int x = pos.Item1;
                        int y = pos.Item2;

                        if (x > 0 && tiles[PosToIdx(x - 1, y, gridSize)] == -2)
                        {
                            var oTileIdx = PosToIdx(x - 1, y, gridSize);
                            tiles[oTileIdx] = tiles[tileIdx];
                            tilesLeft.Remove(oTileIdx);
                            biomeGroup.Add(oTileIdx);
                            foundFlag = true;
                        }
                        else if (y > 0 && tiles[PosToIdx(x, y - 1, gridSize)] == -2)
                        {
                            var oTileIdx = PosToIdx(x, y - 1, gridSize);
                            tiles[oTileIdx] = tiles[tileIdx];
                            tilesLeft.Remove(oTileIdx);
                            biomeGroup.Add(oTileIdx);
                            foundFlag = true;
                        }
                        if (x < gridSize - 1 && tiles[PosToIdx(x + 1, y, gridSize)] == -2)
                        {
                            var oTileIdx = PosToIdx(x + 1, y, gridSize);
                            tiles[oTileIdx] = tiles[tileIdx];
                            tilesLeft.Remove(oTileIdx);
                            biomeGroup.Add(oTileIdx);
                            foundFlag = true;
                        }
                        if (y < gridSize - 1 && tiles[PosToIdx(x, y + 1, gridSize)] == -2)
                        {
                            var oTileIdx = PosToIdx(x, y + 1, gridSize);
                            tiles[oTileIdx] = tiles[tileIdx];
                            tilesLeft.Remove(oTileIdx);
                            biomeGroup.Add(oTileIdx);
                            foundFlag = true;
                        }
                    }
                }
            }
        }
    }

    Tuple<int, int>[] GeneratePoints(int gridSize, int mapSize)
    {
        GD.Print("Generate Points");
        Tuple<int, int>[] points = new Tuple<int, int>[gridSize * gridSize];
        int cellSize = mapSize / gridSize;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                points[PosToIdx(x, y, gridSize)] = new Tuple<int, int>(
                    x * cellSize + Mathf.FloorToInt(GD.Randf() * cellSize),
                    y * cellSize + Mathf.FloorToInt(GD.Randf() * cellSize)
                );
            }
        }

        return points;
    }

    void GenerateSimpleTexture(int[] tiles, int gridSize, int mapSize)
    {
        GD.Print("Generate Simple Texture");
        var image = new Image();
        image.Create(gridSize, gridSize, false, Image.Format.Rgb8);

        image.Lock();
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                int idx = PosToIdx(x, y, gridSize);
                image.SetPixel(x, y, tiles[idx] == -1 ? Colors.Blue : (tiles[idx] == -2 ? Colors.Black : BiomesColors[tiles[idx]]));
            }
        }
        image.Unlock();

        var texture = new ImageTexture();
        texture.CreateFromImage(image, 0);
        _simpleTextureRect.Texture = texture;
        _simpleTextureRect.RectMinSize = new Vector2(mapSize, mapSize);
    }

    Color[] GenerateColorData(int[] tiles, Tuple<int, int>[] points, int gridSize, int mapSize)
    {
        GD.Print("Generate Color Data");
        Color[] colors = new Color[mapSize * mapSize];

        // for (int x = 0; x < mapSize; x++)
        // {
        //     GD.Print($"{x + 1}/{mapSize}");
        //     for (int y = 0; y < mapSize; y++)
        //     {
        //         float minDistance = float.MaxValue;
        //         int pointIdx = -1;

        //         for (int i = 0; i < points.Length; i++)
        //         {
        //             float dist = new Vector2(x, y).DistanceTo(new Vector2(points[i].Item1, points[i].Item2));
        //             if (dist < minDistance)
        //             {
        //                 minDistance = dist;
        //                 pointIdx = i;
        //             }
        //         }

        //         colors[PosToIdx(x, y, mapSize)] = tiles[pointIdx] == -1 ? Colors.Blue : BiomesColors[tiles[pointIdx]];
        //     }
        // }

        int globalX = 0;

        var result = Parallel.For(0, 10, (j) => {
            if (j != 0)
            {
                int x = Interlocked.Increment(ref globalX) - 1;
                while (x < mapSize)
                {
                    for (int y = 0; y < mapSize; y++)
                    {
                        float minDistance = float.MaxValue;
                        int pointIdx = -1;

                        for (int i = 0; i < points.Length; i++)
                        {
                            float dist = new Vector2(x, y).DistanceTo(new Vector2(points[i].Item1, points[i].Item2));
                            if (dist < minDistance)
                            {
                                minDistance = dist;
                                pointIdx = i;
                            }
                        }

                        colors[PosToIdx(x, y, mapSize)] = tiles[pointIdx] == -1 ? Colors.Blue : BiomesColors[tiles[pointIdx]];
                    }

                    x = Interlocked.Increment(ref globalX) - 1;
                }
            }
            else
            {
                while (globalX < mapSize)
                {
                    GD.Print($"{globalX}/{mapSize}");
                }
            }
        });

        return colors;
    }

    void GenerateDetailedTexture(Color[] colorData, int mapSize)
    {
        GD.Print("Generate Detailed Texture");
        var image = new Image();
        image.Create(mapSize, mapSize, false, Image.Format.Rgb8);
        image.Lock();
        
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                image.SetPixel(x, y, colorData[PosToIdx(x, y, mapSize)]);
            }
        }

        image.Unlock();

        var texture = new ImageTexture();
        texture.CreateFromImage(image, 0);
        _detailedTextureRect.Texture = texture;
    }

    int PosToIdx(int x, int y, int size)
    {
        return y * size + x;
    }

    Tuple<int, int> IdxToPos(int idx, int size)
    {
        return new Tuple<int, int>(idx % size, idx / size);
    }
}
