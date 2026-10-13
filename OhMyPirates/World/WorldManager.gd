extends Node2D
class_name WorldManager

export(int, 0, 16384) var map_size_tiles: int = 16

onready var _ocean_tile_map: TileMap = $OceanTileMap

func _ready():
	_fill_ocean()

func _fill_ocean():
	for x in range(map_size_tiles):
		for y in range(map_size_tiles):
			_ocean_tile_map.set_cell(x, y, 0)
