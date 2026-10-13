extends Node2D


func _ready():
	_fill_shallow_tilemap()

func _fill_shallow_tilemap():
	var island_tile_map: TileMap = $IslandTileMap
	var shallow_tile_map: TileMap = $ShallowTileMap
	
	for cell in island_tile_map.get_used_cells():
		var x = cell.x
		var y = cell.y
		
		shallow_tile_map.set_cell(x, y, 0)
		shallow_tile_map.set_cell(x, y + 1, 0)
		shallow_tile_map.set_cell(x, y - 1, 0)
		shallow_tile_map.set_cell(x + 1, y, 0)
		shallow_tile_map.set_cell(x + 1, y + 1, 0)
		shallow_tile_map.set_cell(x + 1, y - 1, 0)
		shallow_tile_map.set_cell(x - 1, y, 0)
		shallow_tile_map.set_cell(x - 1, y + 1, 0)
		shallow_tile_map.set_cell(x - 1, y - 1, 0)
	
	shallow_tile_map.update_bitmask_region()
