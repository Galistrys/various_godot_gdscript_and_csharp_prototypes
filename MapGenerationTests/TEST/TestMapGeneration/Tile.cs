using System.Collections.Generic;
using Godot;

public class Tile
{
    public float HeightValue { get; set; }
    public HeatType HeatType { get; set; }
    public HeightType HeightType { get; set; }
    public MoistureType MoistureType { get; set; }
    
    public BiomeType BiomeType { get; set; }

    public int X;
    public int Y;
    public int BitMask { get; private set; }

    public bool Collidable;
    public bool floodFilled;

    public Tile Left;
    public Tile Right;
    public Tile Top;
    public Tile Bottom;

    public List<River> Rivers = new List<River>();
    public int RiverSize { get; set; }

    public Tile() {}

    public void UpdateBitmask()
    {
        int count = 0;
        if (Top.HeightType == HeightType)
            count += 1;
        if (Right.HeightType == HeightType)
            count += 2;
        if (Bottom.HeightType == HeightType)
            count += 4;
        if (Left.HeightType == HeightType)
            count += 8;

        BitMask = count;
    }

    public static Tile GetTop(Tile t, Tile[,] tiles, int mapSize)
    {
        return tiles[t.X, Mathf.Clamp(t.Y - 1, 0, mapSize - 1)];
    }

    public static Tile GetBottom(Tile t, Tile[,] tiles, int mapSize)
    {
        return tiles[t.X, Mathf.Clamp(t.Y + 1, 0, mapSize - 1)];
    }

    public static Tile GetLeft(Tile t, Tile[,] tiles, int mapSize)
    {
        return tiles[Mathf.Clamp(t.X - 1, 0, mapSize - 1), t.Y];
    }

    public static Tile GetRight(Tile t, Tile[,] tiles, int mapSize)
    {
        return tiles[Mathf.Clamp(t.X + 1, 0, mapSize - 1), t.Y];
    }

    public Direction GetLowestNeighbor()
	{
		if (Left.HeightValue < Right.HeightValue && Left.HeightValue < Top.HeightValue && Left.HeightValue < Bottom.HeightValue)
			return Direction.Left;
		else if (Right.HeightValue < Left.HeightValue && Right.HeightValue < Top.HeightValue && Right.HeightValue < Bottom.HeightValue)
			return Direction.Right;
		else if (Top.HeightValue < Left.HeightValue && Top.HeightValue < Right.HeightValue && Top.HeightValue < Bottom.HeightValue)
			return Direction.Right;
		else if (Bottom.HeightValue < Left.HeightValue && Bottom.HeightValue < Top.HeightValue && Bottom.HeightValue < Right.HeightValue)
			return Direction.Right;
		else 
			return Direction.Bottom;
	}

    public int GetRiverNeighborCount(River river)
	{
		int count = 0;
		if (Left.Rivers.Count > 0 && Left.Rivers.Contains(river))
			count++;
		if (Right.Rivers.Count > 0 && Right.Rivers.Contains(river))
			count++;
		if (Top.Rivers.Count > 0 && Top.Rivers.Contains(river))
			count++;
		if (Bottom.Rivers.Count > 0 && Bottom.Rivers.Contains(river))
			count++;
		return count;
	}

    public void SetRiverPath(River river)
	{
		if (!Collidable)
			return;
		
		if (!Rivers.Contains (river)) {
			Rivers.Add (river);
		}
	}

	private void SetRiverTile(River river)
	{
		SetRiverPath (river);
		HeightType = HeightType.River;
		HeightValue = 0;
		Collidable = false;
	}

	public void DigRiver(River river, int size)
	{
		SetRiverTile (river);
		RiverSize = size;

		if (size == 1) {
			Bottom.SetRiverTile (river);
			Right.SetRiverTile (river);
			Bottom.Right.SetRiverTile (river);				
		}

		if (size == 2) {
			Bottom.SetRiverTile (river);
			Right.SetRiverTile (river);
			Bottom.Right.SetRiverTile (river);
			Top.SetRiverTile (river);
			Top.Left.SetRiverTile (river);
			Top.Right.SetRiverTile (river);
			Left.SetRiverTile (river);
			Left.Bottom.SetRiverTile (river);
		}

		if (size == 3) {
			Bottom.SetRiverTile (river);
			Right.SetRiverTile (river);
			Bottom.Right.SetRiverTile (river);
			Top.SetRiverTile (river);
			Top.Left.SetRiverTile (river);
			Top.Right.SetRiverTile (river);
			Left.SetRiverTile (river);
			Left.Bottom.SetRiverTile (river);
			Right.Right.SetRiverTile (river);
			Right.Right.Bottom.SetRiverTile (river);
			Bottom.Bottom.SetRiverTile (river);
			Bottom.Bottom.Right.SetRiverTile (river);
		}

		if (size == 4) {
			Bottom.SetRiverTile (river);
			Right.SetRiverTile (river);
			Bottom.Right.SetRiverTile (river);
			Top.SetRiverTile (river);
			Top.Right.SetRiverTile (river);
			Left.SetRiverTile (river);
			Left.Bottom.SetRiverTile (river);
			Right.Right.SetRiverTile (river);
			Right.Right.Bottom.SetRiverTile (river);
			Bottom.Bottom.SetRiverTile (river);
			Bottom.Bottom.Right.SetRiverTile (river);
			Left.Bottom.Bottom.SetRiverTile (river);
			Left.Left.Bottom.SetRiverTile (river);
			Left.Left.SetRiverTile (river);
			Left.Left.Top.SetRiverTile (river);
			Left.Top.SetRiverTile (river);
			Left.Top.Top.SetRiverTile (river);
			Top.Top.SetRiverTile (river);
			Top.Top.Right.SetRiverTile (river);
			Top.Right.Right.SetRiverTile (river);
		}	
	}
}