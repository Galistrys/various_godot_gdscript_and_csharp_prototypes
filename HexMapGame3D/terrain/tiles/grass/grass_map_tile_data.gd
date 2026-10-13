class_name GrassMapTileData
extends MapTileData


func generate_hex_tile(scenario: RID, world_pos: Vector3) -> HexTile:
	return GrassHexTile.new(scenario, world_pos)
