extends Node

const TERRAIN_DATA_DIR = "res://terrain_data/chunks"

var _terrain_noise_tools := TerrainNoiseTools.new()


func _ready():
#	_generate_data_chunked()
	pass


func _load_data() -> void:
	pass


func _generate_data_chunked() -> void:
	if not DirAccess.dir_exists_absolute(TERRAIN_DATA_DIR):
		DirAccess.make_dir_recursive_absolute(TERRAIN_DATA_DIR)
	
	for chunk_x in range(0, Constants.MAP_SIZE / Constants.CHUNK_SIZE):
		for chunk_y in range(0, Constants.MAP_SIZE / Constants.CHUNK_SIZE):
			var chunk_data := PackedFloat32Array()
			chunk_data.resize(Constants.CHUNK_SIZE_P_1_N * Constants.CHUNK_SIZE_P_1_N)
			for y in range(0, Constants.CHUNK_SIZE_P_1_N):
				for x in range(0, Constants.CHUNK_SIZE_P_1_N):
					chunk_data[Constants.CHUNK_SIZE_P_1_N * y + x] = _terrain_noise_tools.get_noise_for_coord(
						chunk_x * Constants.CHUNK_SIZE + x - 1,
						chunk_y * Constants.CHUNK_SIZE + y - 1
					)


func _generate_data() -> void:
	var file := FileAccess.open("user://terrain.dat", FileAccess.WRITE)
	for chunk_x in range(0, Constants.MAP_SIZE / Constants.CHUNK_SIZE):
		for chunk_y in range(0, Constants.MAP_SIZE / Constants.CHUNK_SIZE):
			var chunk_data := PackedFloat32Array()
			chunk_data.resize(Constants.CHUNK_SIZE * Constants.CHUNK_SIZE)
			for y in range(0, Constants.CHUNK_SIZE):
				for x in range(0, Constants.CHUNK_SIZE):
					chunk_data[Constants.CHUNK_SIZE * y + x] = _terrain_noise_tools.get_noise_for_coord(
						chunk_x * Constants.CHUNK_SIZE + x,
						chunk_y * Constants.CHUNK_SIZE + y
					)
					
			file.store_buffer(chunk_data.to_byte_array())
	file = null


func get_chunk_data(world_pos: Vector3) -> PackedFloat32Array:
	var result := PackedFloat32Array()
	result.resize(Constants.CHUNK_SIZE_P_1 * Constants.CHUNK_SIZE_P_1)
	
	for y in range(0, Constants.CHUNK_SIZE_P_1):
		for x in range(0, Constants.CHUNK_SIZE_P_1):
			var noise := _terrain_noise_tools.get_noise_for_coord(
				int(x + world_pos.x), int(y + world_pos.z)
			)
			result[y * Constants.CHUNK_SIZE_P_1 + x] = noise
	
	return result
