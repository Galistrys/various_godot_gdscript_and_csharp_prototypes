class_name MapData
extends RefCounted


var _data: Array[MapTileData] = []
var _map_size: int = 0


func _init(map_size: int):
	_map_size = map_size
	_data.resize(map_size * map_size)


func clean() -> void:
	pass


func _coords_to_index(coords: Vector2i) -> int:
	return coords.x * _map_size + coords.y


func _index_to_coords(index: int) -> Vector2i:
	@warning_ignore("integer_division")
	return Vector2i(index / _map_size, index % _map_size)


func _coords_in_map(coords: Vector2i) -> bool:
	return not (coords.x >= _map_size or coords.x < 0 \
		or coords.y >= _map_size or coords.y < 0)


func update_tile(coords: Vector2i) -> bool:
	var neighbors := get_neighbor_tiles(coords)
	return get_tile_data(coords).update_tile(neighbors)


#func get_neighbor_tiles(coords: Vector2i) -> Array[MapTileData]:
#	var results: Array[MapTileData] = []
#	var cubes := HexTools.cube_ring(
#		HexTools.hex_to_cube(coords), 1
#	)
#	for cube in cubes:
#		var hex := HexTools.cube_to_hex(cube)
#		if _coords_in_map(hex):
#			results.append(get_tile_data(hex))
#
#	return results


func get_neighbor_coords(coords: Vector2i) -> Array[Vector2i]:
	var results: Array[Vector2i] = []
	for i in range(0, 6):
		var neighbor := HexTools.hex_neighbor(coords, i)
		if _coords_in_map(neighbor):
			results.append(neighbor)
	return results


func get_neighbor_tiles(coords: Vector2i) -> Array[MapTileData]:
	var results: Array[MapTileData] = []
	for i in range(0, 6):
		var neighbor := HexTools.hex_neighbor(coords, i)
		if _coords_in_map(neighbor):
			results.append(get_tile_data(neighbor))
		else:
			results.append(null)
	return results


#func update_tiles_around(coords: Vector2i, radius: int) -> void:
#	var to_update: Array[Vector2i] = []
#	for r in range(1, radius + 1):
#		var cubes := HexTools.cube_ring(
#			HexTools.hex_to_cube(coords), r
#		)
#		for cube in cubes:
#			var hex := HexTools.cube_to_hex(cube)
#			if _coords_in_map(hex):
#				if update_tile(hex):
#					to_update.append(hex)


func _on_tile_changed(radius: int, tile_emit_data: Dictionary, coords: Vector2i) -> void:
	for r in range(1, radius + 1):
		var cubes := HexTools.cube_ring(
			HexTools.hex_to_cube(coords), r
		)
		for cube in cubes:
			var hex := HexTools.cube_to_hex(cube)
			if _coords_in_map(hex):
				get_tile_data(hex).update_tile(get_neighbor_tiles(hex), tile_emit_data)
#	if radius == 1:
#		for tile in get_neighbor_coords(coords):
#			get_tile_data(tile).update_tile(get_neighbor_tiles(tile))


func change_tile(coords: Vector2i, tile_data: MapTileData) -> void:
	add_tile(coords, tile_data)
	
	update_tile(coords)
	_on_tile_changed.call_deferred(1, {}, coords)


func get_tile_data(coords: Vector2i) -> MapTileData:
	var idx := _coords_to_index(coords)
	if idx >= _data.size() \
	or coords.x >= _map_size or coords.x < 0 \
	or coords.y >= _map_size or coords.y < 0:
		printerr("Invalid coords: ", coords)
		return null
	return _data[idx]


func add_tile(coords: Vector2i, tile_data: MapTileData) -> void:
	var idx := _coords_to_index(coords)
	tile_data.tile_changed.connect(_on_tile_changed.bind(coords))
	_data[idx] = tile_data


func generate_map_data() -> void:
	for x in range(0, _map_size):
		for y in range(0, _map_size):
			var coords := Vector2i(x, y)
			var tile_data := GrassMapTileData.new()
			add_tile(coords, tile_data)
