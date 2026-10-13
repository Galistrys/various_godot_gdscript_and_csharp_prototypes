class_name HexRenderer
extends Node3D

const TWEEN_TIME = 0.125

signal tile_coords_clicked(coords: Vector2i)

var _data: Array[HexTile] = []
var _map_size := 0
var _scenario := RID()

var _collision_plane = Plane(Vector3.UP, Vector3(0, 0.2, 0))

var _last_highlighted_coords := Vector2i(-1, -1)

#var _wait_to_finish_tweens: Dictionary = {}


func _ready() -> void:
	_map_size = get_parent().map_size
	_data.resize(_map_size * _map_size)
	_scenario = get_world_3d().scenario


func _process(_delta: float) -> void:
	var camera := get_window().get_camera_3d()
	var mouse_pos := get_window().get_mouse_position()
	
	var from := camera.project_ray_origin(mouse_pos)
	var normal := camera.project_ray_normal(mouse_pos)
	
	var collision_point := _collision_plane.intersects_ray(from, normal) as Vector3
	var coords := HexTools.world_pos_to_hex(
		Vector2(collision_point.x, collision_point.z),
		Constants.TILE_SIZE
	)
	_highlight_tile(coords)
	
	if Input.is_action_just_pressed("interact"):
		if _coords_in_map(coords):
			tile_coords_clicked.emit(coords)
	
	if _last_highlighted_coords != coords:
		_dehighlight_tile(_last_highlighted_coords)
	_last_highlighted_coords = coords
	
#	for tween in _wait_to_finish_tweens.keys():
#		if not (_wait_to_finish_tweens[tween] as Tween).is_running():
#			_wait_to_finish_tweens.erase(tween)


func _coords_to_index(coords: Vector2i) -> int:
	return coords.x * _map_size + coords.y


func _index_to_coords(index: int) -> Vector2i:
	@warning_ignore("integer_division")
	return Vector2i(index / _map_size, index % _map_size)


func _coords_in_map(coords: Vector2i) -> bool:
	return not (coords.x >= _map_size or coords.x < 0 \
		or coords.y >= _map_size or coords.y < 0)


func clean() -> void:
	for i in range(_data.size()):
		_data[i].hide_tile()


func _highlight_tile(coords: Vector2i) -> void:
#	if coords == _last_highlighted_coords:
#		return
	
	if _coords_in_map(coords):
		var tile := _get_tile(coords)
		tile.highlight()
		tile.highlighted = true


func _dehighlight_tile(coords: Vector2i) -> void:
	if _coords_in_map(coords):
		var tile := _get_tile(coords)
		tile.dehighlight()
		tile.highlighted = false


func _get_tile(coords: Vector2i) -> HexTile:
	if _coords_in_map(coords):
		return _data[_coords_to_index(coords)]
	printerr("Invalid coords: ", coords)
	return null


func add_tile(coords: Vector2i, tile_data: MapTileData, do_tween: bool = false) -> void:
	var idx := _coords_to_index(coords)
	var world_pos := HexTools.hex_to_world_pos(coords, Constants.TILE_SIZE)
	var hex_tile := tile_data.generate_hex_tile(
		_scenario, Vector3(world_pos.x, 0, world_pos.y)
	)
	_data[idx] = hex_tile
	tile_data.tile_changed.connect(hex_tile.update_tile)
	
	hex_tile.show_tile()
	if do_tween:
		hex_tile.set_scale(0.0)
		var tween := get_tree().create_tween()
		tween.tween_method(hex_tile.set_scale, 0.0, 0.0, TWEEN_TIME)
		tween.tween_method(hex_tile.set_scale, 0.0, 1.0, TWEEN_TIME)
#		_wait_to_finish_tweens[tween] = coords
#		_wait_to_finish_tweens.append(tween)


func remove_tile(coords: Vector2i) -> void:
	var idx := _coords_to_index(coords)
	var tile = _data[idx]
	_data[idx] = null
	
	var tween := get_tree().create_tween()
	tween.tween_method(tile.set_scale, 1.0, 0.0, TWEEN_TIME)
	tween.tween_callback(tile.hide_tile)
#	_wait_to_finish_tweens[tween] = coords
#	_wait_to_finish_tweens.append(tween)
#	_wait_to_finish_tweens[coords] = tween
#	tile.hide_tile()


func reload_tile(coords: Vector2i, tile_data: MapTileData) -> void:
	remove_tile(coords)
	add_tile(coords, tile_data, true)


func generate_tiles_from_map_data(map_data: MapData) -> void:
	for x in range(0, _map_size):
		for y in range(0, _map_size):
			var coords := Vector2i(x, y)
			var tile_data := map_data.get_tile_data(coords)
			add_tile(coords, tile_data)
