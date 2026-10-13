using System.Collections.Generic;

public class River  {

	public int Length;
	public List<Tile> Tiles;
	public int ID;

	public int Intersections;
	public float TurnCount;
	public Direction CurrentDirection;
	
	public River(int id)
	{
		ID = id;
		Tiles = new List<Tile> ();
	}
	
	public void AddTile(Tile tile)
	{
		tile.SetRiverPath(this);
		Tiles.Add (tile);
	}	
}