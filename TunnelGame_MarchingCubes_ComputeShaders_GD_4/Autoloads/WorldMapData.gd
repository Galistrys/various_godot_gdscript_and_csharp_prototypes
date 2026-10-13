extends Node

var tile_size: int = 1
var chunk_size: int = 16

var map_size_in_chunks: int = 16
var map_size_in_tiles: int = chunk_size * map_size_in_chunks
var _msit_2: int = map_size_in_tiles * map_size_in_tiles
var _msit_3: int = map_size_in_tiles * map_size_in_tiles * map_size_in_tiles

var _noise: FastNoiseLite = preload("res://Tunnel/TunnelNoise.res")

var _scenario_rid: RID
var _space_rid: RID

#var tunnel_walls_material: StandardMaterial3D = preload("res://Tunnel/Tunnel.material")
var tunnel_walls_material: StandardMaterial3D = preload("res://Tunnel/LowPoly.material")
#var tunnel_walls_material: ShaderMaterial = preload("res://Tunnel/cave.material")

var _node3d: Node3D

var _map_data: PackedFloat32Array = PackedFloat32Array()

func _init():
	_node3d = Node3D.new()
	add_child(_node3d)

func _ready():
	_scenario_rid = _node3d.get_world_3d().scenario
	_space_rid = _node3d.get_world_3d().space
	
	_map_data.resize(_msit_3)
	for i in range(_msit_3):
		_map_data[i] = 1.0

func _get_map_data_index(x: int, y: int, z: int) -> int:
	return z * _msit_2 + y * map_size_in_tiles + x

func set_map_data(x: int, y: int, z: int, value: float) -> void:
	if x < 0 or y < 0 or z < 0 \
		or x > map_size_in_tiles - 1 \
		or y > map_size_in_tiles - 1 \
		or z > map_size_in_tiles - 1:
		push_error(
			"Tried to set a value outside world! (%d, %d, %d)" % [
				x, y, z
			]
		)
	else:
		_map_data[_get_map_data_index(x, y, z)] = value

func get_map_data(x: int, y: int, z: int) -> float:
	if x < 0 or y < 0 or z < 0 \
		or x > map_size_in_tiles - 1 \
		or y > map_size_in_tiles - 1 \
		or z > map_size_in_tiles - 1:
		return 1.0
	return _map_data[_get_map_data_index(x, y, z)]

func get_map_data_v(pos: Vector3) -> float:
	@warning_ignore(narrowing_conversion)
	return get_map_data(pos.x, pos.y, pos.z)

func get_space() -> RID:
	return _space_rid

func get_scenario() -> RID:
	return _scenario_rid

func get_value(pos: Vector3) -> float:
#	_mutex.lock()
#	var val = _data.get(pos, null)
#	if val == null:
#		val = range_lerp(_noise.get_noise_3dv(pos), -1, 1, 0, 1)
#		_data[pos] = val
#	_mutex.unlock()
#	return val
	return remap(_noise.get_noise_3dv(pos), -1, 1, 0, 1)
