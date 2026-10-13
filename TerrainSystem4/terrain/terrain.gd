class_name Terrain
extends Node3D

var _terrain_noise_tools := TerrainNoiseTools.new()
var _map_data: Array = []
var _chunk: Chunk

@export_range(0, 6, 1) var level_of_detail: int = 0:
	get:
		return level_of_detail
	set(value):
		level_of_detail = value
		test_generate()

func _ready():
	test_generate()


func _exit_tree():
	_chunk.remove_from_world()
	_chunk.free()


func test_generate() -> void:
	if _chunk != null:
		_chunk.remove_from_world()
		_chunk.free()
	
	var test_terrain_data: PackedFloat32Array = []
	test_terrain_data.resize(Constants.CHUNK_SIZE_P_1 * Constants.CHUNK_SIZE_P_1)
	for x in range(Constants.CHUNK_SIZE_P_1):
		for y in range(Constants.CHUNK_SIZE_P_1):
			var height := _terrain_noise_tools.get_noise_for_coord(x, y)
			test_terrain_data[y * Constants.CHUNK_SIZE_P_1 + x] = height
	
	_chunk = Chunk.new("Test")
	_chunk.generate(level_of_detail, test_terrain_data)
	_chunk.add_to_world(Vector3.ZERO, get_world_3d().scenario, RID())
