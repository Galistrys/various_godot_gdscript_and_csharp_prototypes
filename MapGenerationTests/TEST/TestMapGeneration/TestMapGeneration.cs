using Godot;
using System;
using System.Collections.Generic;

public class TestMapGeneration : Control
{
    public const int MAX_RIVER_ATTEMPTS = 10;
    public const float MIN_RIVER_HEIGHT = 0.6f;
    public const int MIN_RIVER_TURNS = 18;
    public const float MIN_RIVER_LENGTH = 20;
    public const int MAX_RIVER_INTERSECTIONS = 2;

    SpinBox _sizeSpinBox;

    SpinBox _octavesSpinBox;
    SpinBox _lacunaritySpinBox;
    SpinBox _periodSpinBox;
    SpinBox _persistenceSpinBox;

    SpinBox _riverCountSpinBox;

    int Size { get { return (int)Math.Floor(_sizeSpinBox.Value); } }

    int Octaves { get { return (int)Math.Floor(_octavesSpinBox.Value); } }
    float Lacunarity { get { return (float)_lacunaritySpinBox.Value; } }
    float Period { get { return (float)_periodSpinBox.Value; } }
    float Persistence { get { return (float)_persistenceSpinBox.Value; } }

    int RiverCount { get { return (int)Math.Floor(_riverCountSpinBox.Value); } }

    TextureRect _heightMapTextureRect;
    TextureRect _heatMapTextureRect;
    TextureRect _moistureMapTextureRect;
    TextureRect _biomeMapTextureRect;

    float DeepWaterValue = 0.2f;
    float ShallowWaterValue = 0.4f;  
    float SandValue = 0.5f;
    float GrassValue = 0.7f;
    float ForestValue = 0.8f;
    float RockValue = 0.9f;
    float SnowValue = 1;

    float ColdestValue = 0.1f;
    float ColderValue = 0.18f;
    float ColdValue = 0.4f;
    float WarmValue = 0.6f;
    float WarmerValue = 0.75f;

    float DryerValue = 0.27f;
    float DryValue = 0.4f;
    float WetValue = 0.6f;
    float WetterValue = 0.8f;
    float WettestValue = 0.9f;

    readonly BiomeType[,] BiomeTable = new BiomeType[6,6]
    {   
        //COLDEST        //COLDER          //COLD                  //HOT                          //HOTTER                       //HOTTEST
        { BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYEST
        { BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYER
        { BiomeType.Ice, BiomeType.Tundra, BiomeType.Woodland,     BiomeType.Woodland,            BiomeType.Savanna,             BiomeType.Savanna },             //DRY
        { BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.Woodland,            BiomeType.Savanna,             BiomeType.Savanna },             //WET
        { BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.SeasonalForest,      BiomeType.TropicalRainforest,  BiomeType.TropicalRainforest },  //WETTER
        { BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.TemperateRainforest, BiomeType.TropicalRainforest,  BiomeType.TropicalRainforest }   //WETTEST
    };

    public override void _Ready()
    {
        _sizeSpinBox = GetNode<SpinBox>("HBoxContainer/PanelContainer/VBoxContainer/SizeSpinBox");

        _octavesSpinBox = GetNode<SpinBox>("HBoxContainer/PanelContainer/VBoxContainer/OctavesSpinBox");
        _lacunaritySpinBox = GetNode<SpinBox>("HBoxContainer/PanelContainer/VBoxContainer/LacunaritySpinBox");
        _periodSpinBox = GetNode<SpinBox>("HBoxContainer/PanelContainer/VBoxContainer/PeriodSpinBox");
        _persistenceSpinBox = GetNode<SpinBox>("HBoxContainer/PanelContainer/VBoxContainer/PersistenceSpinBox");

        _riverCountSpinBox = GetNode<SpinBox>("HBoxContainer/PanelContainer/VBoxContainer/RiverCountSpinBox");

        _heightMapTextureRect = GetNode<TextureRect>("HBoxContainer/ScrollContainer/PanelContainer/GridContainer/HeightMap");
        _heatMapTextureRect = GetNode<TextureRect>("HBoxContainer/ScrollContainer/PanelContainer/GridContainer/HeatMap");
        _moistureMapTextureRect = GetNode<TextureRect>("HBoxContainer/ScrollContainer/PanelContainer/GridContainer/MoistureMap");
        _biomeMapTextureRect = GetNode<TextureRect>("HBoxContainer/ScrollContainer/PanelContainer/GridContainer/BiomeMap");

        OS.WindowMaximized = true;
    }

    void OnGenerateButtonPressed()
    {
        GD.Randomize();
        int mapSize = Size;
        
        var heightNoise = new OpenSimplexNoise();
        heightNoise.Seed = (int)GD.Randi();
        heightNoise.Octaves = Octaves;
        heightNoise.Lacunarity = Lacunarity;
        heightNoise.Period = Period;
        heightNoise.Persistence = Persistence;

        var heatNoise = new OpenSimplexNoise();
        heatNoise.Seed = (int)GD.Randi();
        heatNoise.Octaves = 4;
        heatNoise.Lacunarity = 2;
        heatNoise.Period = 128;
        heatNoise.Persistence = 0.5f;

        var moistureNoise = new OpenSimplexNoise();
        moistureNoise.Seed = (int)GD.Randi();
        moistureNoise.Octaves = 4;
        moistureNoise.Lacunarity = 2;
        moistureNoise.Period = 64;
        moistureNoise.Persistence = 0.5f;

        var heatGradient = new Gradient();
        heatGradient.Offsets = new float[] { 0, 0.5f, 1};
        heatGradient.Colors = new Color[] { Colors.Black, Colors.White, Colors.Black };

        MapData mapData = new MapData(mapSize);
        Tile[,] tiles;
        List<TileGroup> waters = new List<TileGroup>();
        List<TileGroup> lands = new List<TileGroup>();
        List<River> rivers = new List<River>();
        List<RiverGroup> riverGroups = new List<RiverGroup>();

        GD.Print("Generate Map Data");
        GenerateMapData(mapData, heightNoise, heatNoise, moistureNoise, mapSize);
        GD.Print("Generate Tiles");
        GenerateTiles(mapData, mapSize, out tiles, heatGradient);

        GD.Print("Update Neighbours");
        UpdateNeighbours(tiles, mapSize);

        // GD.Print("Generate Rivers");
        // GenerateRivers(tiles, mapSize, out rivers);
        // GD.Print("Build River Groups");
        // BuildRiverGroups(tiles, riverGroups, mapSize);
        // GD.Print("Dig River Groups");
        // DigRiverGroups(riverGroups, tiles);
        // GD.Print("Adjust Moisture Map");
        // AdjustMoistureMap(tiles, mapData, mapSize);

        GD.Print("Update Bit Masks");
        UpdateBitMasks(tiles, mapSize);
        GD.Print("Flood Fill");
        FloodFill(lands, waters, tiles, mapSize);

        GD.Print("Generate biome map");
        GenerateBiomeMap(tiles, mapSize);
        
        GD.Print("Drawing...");
        DrawHeightMap(mapSize, tiles);
        DrawHeatMap(mapSize, tiles);
        DrawMoistureMap(mapSize, tiles);
        DrawBiomeMap(mapSize, tiles);
        GD.Print("Done!");
    }

    void GenerateMapData(MapData mapData, OpenSimplexNoise heightNoise, OpenSimplexNoise heatNoise, OpenSimplexNoise moistureNoise, int mapSize)
    {
        float mapSizeFloat = mapSize;
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                // float x1 = x / mapSizeFloat;
                // float y1 = y / mapSizeFloat;

                // float value = noise.GetNoise2d(x, y);
                // if (value > mapData.Max) mapData.Max = value;
                // if (value < mapData.Min) mapData.Min = value;

                // mapData.Data[x, y] = value;

                // Noise range
                float x1 = -1, x2 = 1;
                float y1 = -1, y2 = 1;
                float dx = x2 - x1;
                float dy = y2 - y1;

                // Sample noise at smaller intervals
                float s = x / mapSizeFloat;
                float t = y / mapSizeFloat;
                // float s = x;
                // float t = y;

                // Calculate 4D coords;
                float nx = x1 + Mathf.Cos(s * 2 * Mathf.Pi) * dx / (2 * Mathf.Pi);
                float ny = y1 + Mathf.Cos(t * 2 * Mathf.Pi) * dy / (2 * Mathf.Pi);
                float nz = x1 + Mathf.Sin(s * 2 * Mathf.Pi) * dx / (2 * Mathf.Pi);
                float nw = y1 + Mathf.Sin(t * 2 * Mathf.Pi) * dy / (2 * Mathf.Pi);

                // Max and Mins
                float heightValue = heightNoise.GetNoise4d(nx, ny, nz, nw);

                // MAKING IT "ISLANDISH"
                float d = new Vector2(mapSizeFloat / 2.0f, mapSizeFloat / 2.0f).DistanceTo(new Vector2(x, y)) / (Mathf.Sqrt2 * mapSizeFloat / 2.0f);
                if (d > 0.9f)
                {
                    float kx = x / mapSizeFloat - 0.5f;
                    float ky = y / mapSizeFloat - 0.5f;
                    d = 2 * Mathf.Max(Mathf.Abs(kx), Mathf.Abs(ky));
                    d = Mathf.Pow(d, 3) / (Mathf.Pow(d, 3) + Mathf.Pow(2.2f - 2.2f * d, 3));
                    heightValue = (1 + heightValue - d) / 2.0f;
                }
                
                // END OF "ISLANDISH"

                if (heightValue > mapData.MaxHeight) mapData.MaxHeight = heightValue;
                if (heightValue < mapData.MinHeight) mapData.MinHeight = heightValue;

                float heatValue = heatNoise.GetNoise2d(x, y);
                if (heatValue > mapData.MaxHeat) mapData.MaxHeat = heatValue;
                if (heatValue < mapData.MinHeat) mapData.MinHeat = heatValue;

                float moistureValue = moistureNoise.GetNoise2d(x, y);
                if (moistureValue > mapData.MaxMoisture) mapData.MaxMoisture = moistureValue;
                if (moistureValue < mapData.MinMoisture) mapData.MinMoisture = moistureValue;

                mapData.HeightData[x, y] = heightValue;
                mapData.HeatData[x, y] = heatValue;
                mapData.MoistureData[x, y] = moistureValue;
            }
        }
    }

    void GenerateTiles(MapData mapData, int mapSize, out Tile[,] tiles, Gradient heatGradient)
    {
        float mapSizeFloat = mapSize;
        tiles = new Tile[mapSize, mapSize];
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                Tile t = new Tile();
                t.X = x;
                t.Y = y;

                float heightValue = mapData.HeightData[x, y];
                heightValue = (heightValue - mapData.MinHeight) / (mapData.MaxHeight - mapData.MinHeight);

                t.HeightValue = heightValue;
                tiles[x, y] = t;

                if (heightValue < DeepWaterValue)
                {
                    t.HeightType = HeightType.DeepWater;
                    t.Collidable = false;
                }
                else if (heightValue < ShallowWaterValue)
                {
                    t.HeightType = HeightType.ShallowWater;
                    t.Collidable = false;
                }
                else if (heightValue < SandValue)
                {
                    t.HeightType = HeightType.Sand;
                    t.Collidable = true;
                }
                else if (heightValue < GrassValue)
                {
                    t.HeightType = HeightType.Grass;
                    t.Collidable = true;
                }
                else if (heightValue < ForestValue)
                {
                    t.HeightType = HeightType.Forest;
                    t.Collidable = true;
                }
                else if (heightValue < RockValue)
                {
                    t.HeightType = HeightType.Rock;
                    t.Collidable = true;
                }
                else if (heightValue <= SnowValue)
                {
                    t.HeightType = HeightType.Snow;
                    t.Collidable = true;
                }
                
                float heatGradientValue = heatGradient.Interpolate(y / mapSizeFloat).r;
                float heatValue = mapData.HeatData[x, y];
                heatValue = (heatValue - mapData.MinHeat) / (mapData.MaxHeat - mapData.MinHeat);
                // heatValue *= heatGradientValue;
                heatValue = Mathf.Lerp(heatGradientValue, heatValue, 0.25f);

                if (t.HeightType == HeightType.Grass)
                    heatValue -= 0.1f * t.HeightValue;
                else if (t.HeightType == HeightType.Forest)
                    heatValue -= 0.2f * t.HeightValue;
                else if (t.HeightType == HeightType.Rock)
                    heatValue -= 0.35f * t.HeightValue;
                else if (t.HeightType == HeightType.Snow)
                    heatValue -= 0.45f * t.HeightValue;

                if (heatValue < ColdestValue) 
                    t.HeatType = HeatType.Coldest;
                else if (heatValue < ColderValue)
                    t.HeatType = HeatType.Colder;
                else if (heatValue < ColdValue) 
                    t.HeatType = HeatType.Cold;
                else if (heatValue < WarmValue) 
                    t.HeatType = HeatType.Warm;
                else if (heatValue < WarmerValue) 
                    t.HeatType = HeatType.Warmer;
                else
                    t.HeatType = HeatType.Warmest;

                float moistureValue = mapData.MoistureData[x, y];
                moistureValue = (moistureValue - mapData.MinMoisture) / (mapData.MaxMoisture - mapData.MinMoisture);
                if (t.HeightType == HeightType.DeepWater)
                    moistureValue += 8f * t.HeightValue;
                else if (t.HeightType == HeightType.ShallowWater)
                    moistureValue += 3f * t.HeightValue;
                else if (t.HeightType == HeightType.Shore)
                    moistureValue += 1f * t.HeightValue;
                else if (t.HeightType == HeightType.Sand)
                    moistureValue += 0.25f * t.HeightValue;

                if (moistureValue < DryerValue)
                    t.MoistureType = MoistureType.Dryest;
                else if (moistureValue < DryValue)
                    t.MoistureType = MoistureType.Dryer;
                else if (moistureValue < WetValue)
                    t.MoistureType = MoistureType.Dry;
                else if (moistureValue < WetterValue)
                    t.MoistureType = MoistureType.Wet;
                else if (moistureValue < WettestValue)
                    t.MoistureType = MoistureType.Wetter;
                else
                    t.MoistureType = MoistureType.Wettest;
            }
        }
    }

    void UpdateNeighbours(Tile[,] tiles, int mapSize)
    {
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                Tile t = tiles[x, y];

                t.Top = Tile.GetTop(t, tiles, mapSize);
                t.Bottom = Tile.GetBottom(t, tiles, mapSize);
                t.Left = Tile.GetLeft(t, tiles, mapSize);
                t.Right = Tile.GetRight(t, tiles, mapSize);
            }
        }
    }

    void UpdateBitMasks(Tile[,] tiles, int mapSize)
    {
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                tiles[x, y].UpdateBitmask();
            }
        }
    }

    void FloodFill(List<TileGroup> lands, List<TileGroup> waters, Tile[,] tiles, int mapSize)
    {
        Stack<Tile> stack = new Stack<Tile>();

        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                Tile t = tiles[x, y];

                if (t.floodFilled) continue;
                if (t.Collidable)
                {
                    TileGroup group = new TileGroup();
                    group.Type = TileGroupType.Land;
                    stack.Push(t);

                    while (stack.Count > 0)
                        FloodFill(stack.Pop(), ref group, ref stack, tiles, mapSize);
                    
                    if (group.Tiles.Count > 0)
                        lands.Add(group);
                }
                else
                {
                    TileGroup group = new TileGroup();
                    group.Type = TileGroupType.Water;
                    stack.Push(t);

                    while (stack.Count > 0)
                        FloodFill(stack.Pop(), ref group, ref stack, tiles, mapSize);
                    
                    if (group.Tiles.Count > 0)
                        waters.Add(group);
                }
            }
        }
    }

    void FloodFill(Tile tile, ref TileGroup tileGroup, ref Stack<Tile> stack, Tile[,] tiles, int mapSize)
    {
        if (tile.floodFilled)
            return;
        if (tileGroup.Type == TileGroupType.Land && !tile.Collidable)
            return;
        if (tileGroup.Type == TileGroupType.Water && tile.Collidable)
            return;
        
        tileGroup.Tiles.Add(tile);
        tile.floodFilled = true;

        Tile t = Tile.GetTop(tile, tiles, mapSize);
        if (!t.floodFilled && tile.Collidable == t.Collidable)
            stack.Push(t);
        t = Tile.GetBottom(tile, tiles, mapSize);
        if (!t.floodFilled && tile.Collidable == t.Collidable)
            stack.Push(t);
        t = Tile.GetLeft(tile, tiles, mapSize);
        if (!t.floodFilled && tile.Collidable == t.Collidable)
            stack.Push(t);
        t = Tile.GetRight(tile, tiles, mapSize);
        if (!t.floodFilled && tile.Collidable == t.Collidable)
            stack.Push(t);
    }

    void GenerateRivers(Tile[,] tiles, int mapSize, out List<River> rivers)
    {
        int attempts = 0;
        int maxRiverCount = RiverCount;
        int riverCount = maxRiverCount;
        rivers = new List<River>();

        while (riverCount > 0 && attempts < MAX_RIVER_ATTEMPTS)
        {
            GD.PrintT($"River count: {riverCount}/{maxRiverCount},", $"Attepts: {attempts}/{MAX_RIVER_ATTEMPTS}");
            int x = ((int)Math.Floor(GD.RandRange(0, mapSize)));
            int y = ((int)Math.Floor(GD.RandRange(0, mapSize)));

            Tile tile;
            tile = tiles[x, y];

            if (!tile.Collidable) continue;
            if (tile.Rivers.Count > 0) continue;

            if (tile.HeightValue > MIN_RIVER_HEIGHT)
            {
                River river = new River(riverCount);
                river.CurrentDirection = tile.GetLowestNeighbor();

                FindPathToWater(tile, river.CurrentDirection, tiles, mapSize, ref river);

                if (river.TurnCount < MIN_RIVER_TURNS || river.Tiles.Count < MIN_RIVER_LENGTH || river.Intersections > MAX_RIVER_INTERSECTIONS)
                {
                    for (int i = 0; i < river.Tiles.Count; i++)
                    {
                        Tile t = river.Tiles[i];
                        t.Rivers.Remove(river);
                    }
                }
                else if (river.Tiles.Count >= MIN_RIVER_LENGTH)
                {
                    rivers.Add(river);
                    tile.Rivers.Add(river);
                    riverCount--;
                }
            }

            attempts++;
        }
    }

    void FindPathToWater(Tile tile, Direction direction, Tile[,] tiles, int mapSize, ref River river)
    {
        if (tile.Rivers.Contains (river))
            return;
    
        // check if there is already a river on this tile
        if (tile.Rivers.Count > 0)
            river.Intersections++;
    
        river.AddTile (tile);
    
        // get neighbors
        Tile left = Tile.GetLeft(tile, tiles, mapSize);
        Tile right = Tile.GetRight(tile, tiles, mapSize);
        Tile top = Tile.GetTop(tile, tiles, mapSize);
        Tile bottom = Tile.GetBottom(tile, tiles, mapSize);
        
        float leftValue = int.MaxValue;
        float rightValue = int.MaxValue;
        float topValue = int.MaxValue;
        float bottomValue = int.MaxValue;
        
        // query height values of neighbors
        if (left.GetRiverNeighborCount(river) < 2 && !river.Tiles.Contains(left)) 
            leftValue = left.HeightValue;
        if (right.GetRiverNeighborCount(river) < 2 && !river.Tiles.Contains(right)) 
            rightValue = right.HeightValue;
        if (top.GetRiverNeighborCount(river) < 2 && !river.Tiles.Contains(top)) 
            topValue = top.HeightValue;
        if (bottom.GetRiverNeighborCount(river) < 2 && !river.Tiles.Contains(bottom)) 
            bottomValue = bottom.HeightValue;
        
        // if neighbor is existing river that is not this one, flow into it
        if (bottom.Rivers.Count == 0 && !bottom.Collidable)
            bottomValue = 0;
        if (top.Rivers.Count == 0 && !top.Collidable)
            topValue = 0;
        if (left.Rivers.Count == 0 && !left.Collidable)
            leftValue = 0;
        if (right.Rivers.Count == 0 && !right.Collidable)
            rightValue = 0;
        
        // override flow direction if a tile is significantly lower
        if (direction == Direction.Left)
            if (Mathf.Abs (rightValue - leftValue) < 0.1f)
                rightValue = int.MaxValue;
        if (direction == Direction.Right)
            if (Mathf.Abs (rightValue - leftValue) < 0.1f)
                leftValue = int.MaxValue;
        if (direction == Direction.Top)
            if (Mathf.Abs (topValue - bottomValue) < 0.1f)
                bottomValue = int.MaxValue;
        if (direction == Direction.Bottom)
            if (Mathf.Abs (topValue - bottomValue) < 0.1f)
                topValue = int.MaxValue;
        
        // find mininum
        float min = Mathf.Min (Mathf.Min (Mathf.Min (leftValue, rightValue), topValue), bottomValue);
        
        // if no minimum found - exit
        if (min == int.MaxValue)
            return;
        
        //Move to next neighbor
        if (min == leftValue) {
            if (left.Collidable)
            {
                if (river.CurrentDirection != Direction.Left){
                    river.TurnCount++;
                    river.CurrentDirection = Direction.Left;
                }
                FindPathToWater(left, direction, tiles, mapSize, ref river);
            }
        } else if (min == rightValue) {
            if (right.Collidable)
            {
                if (river.CurrentDirection != Direction.Right){
                    river.TurnCount++;
                    river.CurrentDirection = Direction.Right;
                }
                FindPathToWater(right, direction, tiles, mapSize, ref river);
            }
        } else if (min == bottomValue) {
            if (bottom.Collidable)
            {
                if (river.CurrentDirection != Direction.Bottom){
                    river.TurnCount++;
                    river.CurrentDirection = Direction.Bottom;
                }
                FindPathToWater(bottom, direction, tiles, mapSize, ref river);
            }
        } else if (min == topValue) {
            if (top.Collidable)
            {if (river.CurrentDirection != Direction.Top){
                    river.TurnCount++;
                    river.CurrentDirection = Direction.Top;
                }
                FindPathToWater(top, direction, tiles, mapSize, ref river);
            }
        }
    }

    void BuildRiverGroups(Tile[,] tiles, List<RiverGroup> riverGroups, int mapSize)
    {
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                Tile t = tiles[x, y];
 
                if (t.Rivers.Count > 1)
                {
                    // multiple rivers == intersection
                    RiverGroup group = null;
    
                    // Does a rivergroup already exist for this group?
                    for (int n=0; n < t.Rivers.Count; n++)
                    {
                        River tileriver = t.Rivers[n];
                        for (int i = 0; i < riverGroups.Count; i++)
                        {
                            for (int j = 0; j < riverGroups[i].Rivers.Count; j++)
                            {
                                River river = riverGroups[i].Rivers[j];
                                if (river.ID == tileriver.ID)
                                {
                                    group = riverGroups[i];
                                }
                                if (group != null) break;
                            }
                            if (group != null) break;
                        }
                        if (group != null) break;
                    }
    
                    // existing group found -- add to it
                    if (group != null)
                    {
                        for (int n=0; n < t.Rivers.Count; n++)
                        {
                            if (!group.Rivers.Contains(t.Rivers[n]))
                                group.Rivers.Add(t.Rivers[n]);
                        }
                    }
                    else   //No existing group found - create a new one
                    {
                        group = new RiverGroup();
                        for (int n=0; n < t.Rivers.Count; n++)
                        {
                            group.Rivers.Add(t.Rivers[n]);
                        }
                        riverGroups.Add(group);
                    }
                }
            }
        }
    }

    void DigRiverGroups(List<RiverGroup> riverGroups, Tile[,] tiles)
    {
        for (int i = 0; i < riverGroups.Count; i++)
        {
            GD.Print($"Digging River: {i + 1}/{riverGroups.Count}");
            RiverGroup group = riverGroups[i];
            River longest = null;
    
            //Find longest river in this group
            for (int j = 0; j < group.Rivers.Count; j++)
            {
                River river = group.Rivers[j];
                if (longest == null)
                    longest = river;
                else if (longest.Tiles.Count < river.Tiles.Count)
                    longest = river;
            }
    
            if (longest != null)
            {               
                //Dig out longest path first
                DigRiver (longest);
    
                for (int j = 0; j < group.Rivers.Count; j++)
                {
                    River river = group.Rivers[j];
                    if (river != longest)
                    {
                        DigRiver(river, longest);
                    }
                }
            }
        }
    }

    void DigRiver(River river)
    {
        int counter = 0;
        
        // How wide are we digging this river?
        int size = (int)GD.Randi() % 4 + 1;
        river.Length = river.Tiles.Count;  
    
        // randomize size change
        int two = river.Length / 2;
        int three = two / 2;
        int four = three / 2;
        int five = four / 2;
        
        int twomin = two / 3;
        int threemin = three / 3;
        int fourmin = four / 3;
        int fivemin = five / 3;
    
        // randomize lenght of each size
        int count1 = (int)Math.Floor(GD.RandRange(fivemin, five));
        if (size < 4) {
            count1 = 0;
        }
        int count2 = count1 + (int)Math.Floor(GD.RandRange(fourmin, four)); 
        if (size < 3) {
            count2 = 0;
            count1 = 0;
        }
        int count3 = count2 + (int)Math.Floor(GD.RandRange(threemin, three)); 
        if (size < 2) {
            count3 = 0;
            count2 = 0;
            count1 = 0;
        }
        int count4 = count3 + (int)Math.Floor(GD.RandRange(twomin, two));  
        
        // Make sure we are not digging past the river path
        if (count4 > river.Length) {
            int extra = count4 - river.Length;
            while (extra > 0)
            {
                if (count1 > 0) { count1--; count2--; count3--; count4--; extra--; }
                else if (count2 > 0) { count2--; count3--; count4--; extra--; }
                else if (count3 > 0) { count3--; count4--; extra--; }
                else if (count4 > 0) { count4--; extra--; }
            }
        }
    
        // Dig it out
        for (int i = river.Tiles.Count - 1; i >= 0 ; i--)
        {
            Tile t = river.Tiles[i];
    
            if (counter < count1) {
                t.DigRiver (river, 4);              
            }
            else if (counter < count2) {
                t.DigRiver (river, 3);              
            } 
            else if (counter < count3) {
                t.DigRiver (river, 2);              
            } 
            else if ( counter < count4) {
                t.DigRiver (river, 1);
            }
            else {
                t.DigRiver(river, 0);
            }           
            counter++;          
        }
    }

    void DigRiver(River river, River parent)
    {
        int intersectionID = 0;
        int intersectionSize = 0;

        // determine point of intersection
        for (int i = 0; i < river.Tiles.Count; i++) {
            Tile t1 = river.Tiles[i];
            for (int j = 0; j < parent.Tiles.Count; j++) {
                Tile t2 = parent.Tiles[j];
                if (t1 == t2)
                {
                    intersectionID = i;
                    intersectionSize = t2.RiverSize;
                }
            }
        }

        int counter = 0;
        int intersectionCount = river.Tiles.Count - intersectionID;
        int size = (int)Math.Floor(GD.RandRange(intersectionSize, 5));
        river.Length = river.Tiles.Count;  

        // randomize size change
        int two = river.Length / 2;
        int three = two / 2;
        int four = three / 2;
        int five = four / 2;
        
        int twomin = two / 3;
        int threemin = three / 3;
        int fourmin = four / 3;
        int fivemin = five / 3;
        
        // randomize length of each size
        int count1 = (int)Math.Floor(GD.RandRange(fivemin, five));
        if (size < 4) {
            count1 = 0;
        }
        int count2 = count1 + (int)Math.Floor(GD.RandRange(fourmin, four)); 
        if (size < 3) {
            count2 = 0;
            count1 = 0;
        }
        int count3 = count2 + (int)Math.Floor(GD.RandRange(threemin, three)); 
        if (size < 2) {
            count3 = 0;
            count2 = 0;
            count1 = 0;
        }
        int count4 = count3 + (int)Math.Floor(GD.RandRange(twomin, two));  

        // Make sure we are not digging past the river path
        if (count4 > river.Length) {
            int extra = count4 - river.Length;
            while (extra > 0)
            {
                if (count1 > 0) { count1--; count2--; count3--; count4--; extra--; }
                else if (count2 > 0) { count2--; count3--; count4--; extra--; }
                else if (count3 > 0) { count3--; count4--; extra--; }
                else if (count4 > 0) { count4--; extra--; }
            }
        }
                
        // adjust size of river at intersection point
        if (intersectionSize == 1) {
            count4 = intersectionCount;
            count1 = 0;
            count2 = 0;
            count3 = 0;
        } else if (intersectionSize == 2) {
            count3 = intersectionCount;        
            count1 = 0;
            count2 = 0;
        } else if (intersectionSize == 3) {
            count2 = intersectionCount;
            count1 = 0;
        } else if (intersectionSize == 4) {
            count1 = intersectionCount;
        } else {
            count1 = 0;
            count2 = 0;
            count3 = 0;
            count4 = 0;
        }

        // dig out the river
        for (int i = river.Tiles.Count - 1; i >= 0; i--) {

            Tile t = river.Tiles [i];

            if (counter < count1) {
                t.DigRiver(river, 4);                
            } else if (counter < count2) {
                t.DigRiver(river, 3);                
            } else if (counter < count3) {
                t.DigRiver(river, 2);                
            } 
            else if ( counter < count4) {
                t.DigRiver(river, 1);
            }
            else {
                t.DigRiver(river, 0);
            }            
            counter++;            
        }
    }

    void AdjustMoistureMap(Tile[,] tiles, MapData mapData, int mapSize)
    {
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                Tile t = tiles[x, y];
                if (t.HeightType == HeightType.River)
                {
                    AddMoisture(t, 60, tiles, mapData, mapSize);
                }
            }
        }
    }

    void AddMoisture(Tile t, int radius, Tile[,] tiles, MapData mapData, int mapSize)
    {
        int startx = Mathf.Clamp(t.X - radius, 0, mapSize - 1);
        int endx = Mathf.Clamp(t.X + radius, 0, mapSize - 1);
        Vector2 center = new Vector2(t.X, t.Y);
        int curr = radius;
    
        while (curr > 0) {
    
            int x1 = Mathf.Clamp(t.X - curr, 0, mapSize - 1);
            int x2 = Mathf.Clamp(t.X + curr, 0, mapSize - 1);
            int y = t.Y;
    
            AddMoisture(tiles[x1, y], 0.025f / (center - new Vector2(x1, y)).Length(), mapData);
    
            for (int i = 0; i < curr; i++)
            {
                AddMoisture (tiles[x1, Mathf.Clamp(y + i + 1, 0, mapSize - 1)], 0.025f / (center - new Vector2(x1, Mathf.Clamp(y + i + 1, 0, mapSize - 1))).Length(), mapData);
                AddMoisture (tiles[x1, Mathf.Clamp(y - (i + 1), 0, mapSize - 1)], 0.025f / (center - new Vector2(x1, Mathf.Clamp(y - (i + 1), 0, mapSize - 1))).Length(), mapData);
    
                AddMoisture (tiles[x2, Mathf.Clamp(y + i + 1, 0, mapSize - 1)], 0.025f / (center - new Vector2(x2, Mathf.Clamp(y + i + 1, 0, mapSize - 1))).Length(), mapData);
                AddMoisture (tiles[x2, Mathf.Clamp(y - (i + 1), 0, mapSize - 1)], 0.025f / (center - new Vector2(x2, Mathf.Clamp(y - (i + 1), 0, mapSize - 1))).Length(), mapData);
            }
            curr--;
        }
    }

    private void AddMoisture(Tile t, float amount, MapData mapData)
	{
        float moistureValue = mapData.MoistureData[t.X, t.Y];
		moistureValue += amount;
        
		if (moistureValue > 1)
			moistureValue = 1;
        mapData.MoistureData[t.X, t.Y] = moistureValue;

		if (moistureValue < DryerValue) t.MoistureType = MoistureType.Dryest;
		else if (moistureValue < DryValue) t.MoistureType = MoistureType.Dryer;
		else if (moistureValue < WetValue) t.MoistureType = MoistureType.Dry;
		else if (moistureValue < WetterValue) t.MoistureType = MoistureType.Wet;
		else if (moistureValue < WettestValue) t.MoistureType = MoistureType.Wetter;
		else t.MoistureType = MoistureType.Wettest;
	}

    void GenerateBiomeMap(Tile[,] tiles, int mapSize)
    {
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                Tile t = tiles[x, y];
                if (!t.Collidable) t.BiomeType = BiomeType.Water;
                t.BiomeType = GetBiomeType(t);
            }
        }
    }

    void DrawHeightMap(int mapSize, Tile[,] tiles)
    {
        Color DeepColor = new Color(0, 0, 0.5f, 1);
        Color ShallowColor = new Color(25/255f, 25/255f, 150/255f, 1);
        Color SandColor = new Color(240 / 255f, 240 / 255f, 64 / 255f, 1);
        Color GrassColor = new Color(50 / 255f, 220 / 255f, 20 / 255f, 1);
        Color ForestColor = new Color(16 / 255f, 160 / 255f, 0, 1);
        Color RockColor = new Color(0.5f, 0.5f, 0.5f, 1);            
        Color SnowColor = new Color(1, 1, 1, 1);

        var heightMapImage = new Image();
        heightMapImage.Create(mapSize, mapSize, false, Image.Format.Rgb8);

        heightMapImage.Lock();
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                Tile tile = tiles[x, y];
                HeightType type = tile.HeightType;
                Color color = DeepColor;
                if (type == HeightType.DeepWater)
                    color = DeepColor;
                else if (type == HeightType.ShallowWater)
                    color = ShallowColor;
                else if (type == HeightType.Sand)
                    color = SandColor;
                else if (type == HeightType.Grass)
                    color = GrassColor;
                else if (type == HeightType.Forest)
                    color = ForestColor;
                else if (type == HeightType.Rock)
                    color = RockColor;
                else
                    color = SnowColor;
                
                if (tile.BitMask != 15)
                {
                    Vector3 vec = new Vector3(color.r, color.g, color.b).LinearInterpolate(Vector3.Zero, 0.4f);
                    color = new Color(vec.x, vec.y, vec.z, 1);
                }

                heightMapImage.SetPixel(x, y, color);
            }
        }
        heightMapImage.Unlock();

        var heightMapTexture = new ImageTexture();
        heightMapTexture.CreateFromImage(heightMapImage, 0);
        _heightMapTextureRect.Texture = heightMapTexture;
    }

    void DrawHeatMap(int mapSize, Tile[,] tiles)
    {
        Color ColdestColor = new Color(0, 1, 1, 1);
        Color ColderColor = new Color(170/255f, 1, 1, 1);
        Color ColdColor = new Color(0, 229/255f, 133/255f, 1);
        Color WarmColor = new Color(1, 1, 100/255f, 1);
        Color WarmerColor = new Color(1, 100/255f, 0, 1);
        Color WarmestColor = new Color(241/255f, 12/255f, 0, 1);

        var heatMapImage = new Image();
        heatMapImage.Create(mapSize, mapSize, false, Image.Format.Rgb8);

        heatMapImage.Lock();
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                Tile tile = tiles[x, y];
                HeatType type = tile.HeatType;
                Color color = ColdestColor;

                if (type == HeatType.Coldest)
                    color = ColdestColor;
                else if (type == HeatType.Colder)
                    color = ColderColor;
                else if (type == HeatType.Cold)
                    color = ColdColor;
                else if (type == HeatType.Warm)
                    color = WarmColor;
                else if (type == HeatType.Warmer)
                    color = WarmerColor;
                else
                    color = WarmestColor;
                
                if (tile.BitMask != 15)
                {
                    Vector3 vec = new Vector3(color.r, color.g, color.b).LinearInterpolate(Vector3.Zero, 0.4f);
                    color = new Color(vec.x, vec.y, vec.z, 1);
                }

                heatMapImage.SetPixel(x, y, color);
            }
        }
        heatMapImage.Unlock();

        var heatMapTexture = new ImageTexture();
        heatMapTexture.CreateFromImage(heatMapImage, 0);
        _heatMapTextureRect.Texture = heatMapTexture;
    }

    void DrawMoistureMap(int mapSize, Tile[,] tiles)
    {
        Color DryestColor = new Color(255/255f, 139/255f, 17/255f, 1);
        Color DryerColor = new Color(245/255f, 245/255f, 23/255f, 1);
        Color DryColor = new Color(80/255f, 255/255f, 0/255f, 1);
        Color WetColor = new Color(85/255f, 255/255f, 255/255f, 1);
        Color WetterColor = new Color(20/255f, 70/255f, 255/255f, 1);
        Color WettestColor = new Color(0/255f, 0/255f, 100/255f, 1);

        var moistureMapImage = new Image();
        moistureMapImage.Create(mapSize, mapSize, false, Image.Format.Rgb8);

        moistureMapImage.Lock();
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                Tile tile = tiles[x, y];
                MoistureType type = tile.MoistureType;
                Color color = DryestColor;

                if (type == MoistureType.Dryest)
                    color = DryestColor;
                else if (type == MoistureType.Dryer)
                    color = DryerColor;
                else if (type == MoistureType.Dry)
                    color = DryColor;
                else if (type == MoistureType.Wet)
                    color = WetColor;
                else if (type == MoistureType.Wetter)
                    color = WetterColor;
                else
                    color = WettestColor;
                
                if (tile.BitMask != 15)
                {
                    Vector3 vec = new Vector3(color.r, color.g, color.b).LinearInterpolate(Vector3.Zero, 0.4f);
                    color = new Color(vec.x, vec.y, vec.z, 1);
                }

                moistureMapImage.SetPixel(x, y, color);
            }
        }
        moistureMapImage.Unlock();

        var moistureMapTexture = new ImageTexture();
        moistureMapTexture.CreateFromImage(moistureMapImage, 0);
        _moistureMapTextureRect.Texture = moistureMapTexture;
    }

    void DrawBiomeMap(int mapSize, Tile[,] tiles)
    {
        Color Ice = Colors.White;
        Color Desert = new Color(238/255f, 218/255f, 130/255f, 1);
        Color Savanna = new Color(177/255f, 209/255f, 110/255f, 1);
        Color TropicalRainforest = new Color(66/255f, 123/255f, 25/255f, 1);
        Color Tundra = new Color(96/255f, 131/255f, 112/255f, 1);
        Color TemperateRainforest = new Color(29/255f, 73/255f, 40/255f, 1);
        Color Grassland = new Color(164/255f, 225/255f, 99/255f, 1);
        Color SeasonalForest = new Color(73/255f, 100/255f, 35/255f, 1);
        Color BorealForest = new Color(95/255f, 115/255f, 62/255f, 1);
        Color Woodland = new Color(139/255f, 175/255f, 90/255f, 1);

        Color DeepColor = new Color(0, 0, 0.5f, 1);
        Color ShallowColor = new Color(25/255f, 25/255f, 150/255f, 1);

        var biomeMapImage = new Image();
        biomeMapImage.Create(mapSize, mapSize, false, Image.Format.Rgb8);

        biomeMapImage.Lock();
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                Tile tile = tiles[x, y];
                var biomeType = tile.BiomeType;

                Color color = Ice;
                if (biomeType == BiomeType.Water)
                {
                    if (tile.HeightType == HeightType.DeepWater)
                        color = DeepColor;
                    else if (tile.HeightType == HeightType.ShallowWater)
                        color = ShallowColor;
                }
                else if (biomeType == BiomeType.Ice)
                    color = Ice;
                else if (biomeType == BiomeType.BorealForest)
                    color = BorealForest;
                else if (biomeType == BiomeType.Desert)
                    color = Desert;
                else if (biomeType == BiomeType.Grassland)
                    color = Grassland;
                else if (biomeType == BiomeType.SeasonalForest)
                    color = SeasonalForest;
                else if (biomeType == BiomeType.Tundra)
                    color = Tundra;
                else if (biomeType == BiomeType.Savanna)
                    color = Savanna;
                else if (biomeType == BiomeType.TemperateRainforest)
                    color = TemperateRainforest;
                else if (biomeType == BiomeType.TropicalRainforest)
                    color = TropicalRainforest;
                else if (biomeType == BiomeType.Woodland)
                    color = Woodland;
                
                if (tile.HeightType == HeightType.DeepWater)
                    color = DeepColor;
                else if (tile.HeightType == HeightType.ShallowWater)
                    color = ShallowColor;
                
                if (tile.BitMask != 15)
                {
                    Vector3 vec = new Vector3(color.r, color.g, color.b).LinearInterpolate(Vector3.Zero, 0.4f);
                    color = new Color(vec.x, vec.y, vec.z, 1);
                }

                biomeMapImage.SetPixel(x, y, color);
            }
        }
        biomeMapImage.Unlock();

        var biomeMapTexture = new ImageTexture();
        biomeMapTexture.CreateFromImage(biomeMapImage, 0);
        _biomeMapTextureRect.Texture = biomeMapTexture;
    }

    BiomeType GetBiomeType(Tile tile)
    {
        return BiomeTable[(int)tile.MoistureType, (int)tile.HeatType];
    }
}
