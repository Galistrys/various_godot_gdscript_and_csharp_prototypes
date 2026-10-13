class_name Terrain
extends Node3D

@export_node_path(Node3D) var _target_node_path := NodePath()
@onready var _target := get_node(_target_node_path) as Node3D

var _terrain_chunk_dict := {}

var _old_player_chunk_coord := Vector2i(
	9223372036854775807, 9223372036854775807
)
var _current_player_chunk_coord := Vector2i(
	0, 0
)

var _scenario: RID
var _space: RID

#var _flat_map_data: PackedFloat32Array

var _main_thread := Thread.new()
var _main_should_end := false
var _workers_should_end := false

var _noise := preload("res://world/terrain/default_noise.res") as FastNoiseLite


func _ready() -> void:
	_scenario = get_world_3d().scenario
	_space = get_world_3d().space
	
#	if not Engine.is_editor_hint():
#		var res := _main_thread.start(
#			Callable(self, "_master_thread_loop")
#		)
#		if res != OK:
#			push_error("Failed to start thread: " + str(res))


func _process(_delta) -> void:
	_current_player_chunk_coord = Vector2i(
		roundi(_target.global_position.x / Constants.CHUNK_SIZE),
		roundi(_target.global_position.z / Constants.CHUNK_SIZE)
	)


func _exit_tree() -> void:
	_main_should_end = true
	_main_thread.wait_to_finish()


func _prepare_terrain_dict_worker(
	idx: int, chunks_list: Array
) -> void:
	var chunk_pos := chunks_list[idx] as Vector2i
	var chunk_obj := _terrain_chunk_dict[chunk_pos] as TerrainChunk
	chunk_obj.height_data = _get_chunk_data(chunk_pos * Constants.CHUNK_SIZE)
	chunk_obj.create_instance_and_body(_scenario, _space)
	chunk_obj.generate_collision_shape()


func prepare_terrain_dict_threaded() -> void:
	for x in range(0, Constants.MAP_SIZE / Constants.CHUNK_SIZE):
		for y in range(0, Constants.MAP_SIZE / Constants.CHUNK_SIZE):
			var chunk_pos := Vector2i(x, y)
			var chunk := TerrainChunk.new(chunk_pos)
			_terrain_chunk_dict[chunk_pos] = chunk
	
	var group_id: int = WorkerThreadPool.add_group_task(
		Callable(self, "_prepare_terrain_dict_worker").bind(
			_terrain_chunk_dict.keys()
		), _terrain_chunk_dict.size()
	)
	WorkerThreadPool.wait_for_group_task_completion(group_id)


func _prepare_terrain_dict() -> void:
	for x in range(0, Constants.MAP_SIZE / Constants.CHUNK_SIZE):
		for y in range(0, Constants.MAP_SIZE / Constants.CHUNK_SIZE):
			var chunk_pos := Vector2i(x, y)
			var chunk := TerrainChunk.new(chunk_pos)
			chunk.height_data = _get_chunk_data(chunk_pos * Constants.CHUNK_SIZE)
			chunk.create_instance_and_body(_scenario, _space)
			chunk.generate_collision_shape()
			_terrain_chunk_dict[chunk_pos] = chunk


func _should_thread_break() -> bool:
	return _current_player_chunk_coord != _old_player_chunk_coord


func _get_chunk_coords_simple_distance(
	a: Vector2i, b: Vector2i
) -> int:
	var vec := b - a
	return max(
		abs(vec[vec.max_axis_index()]),
		abs(vec[vec.min_axis_index()])
	)


func _get_sorted_chunks_list(current_chunk_pos: Vector2i) -> Array[Vector2i]:
	var chunks_list := _terrain_chunk_dict.keys()
	chunks_list.sort_custom(
		func(a, b):
			return _get_chunk_coords_simple_distance(
				a, current_chunk_pos
			) < _get_chunk_coords_simple_distance(
				b, current_chunk_pos
			)
	)
	return chunks_list


func _update_chunk_lod_in_thread(
	idx: int,
	current_chunk_pos: Vector2i,
	chunks_list: Array[Vector2i],
	to_update_list: Array
) -> void:
	var current_chunk := chunks_list[idx]
	var simple_distance: int = clamp((
		_get_chunk_coords_simple_distance(
			current_chunk, current_chunk_pos
		) - 1) * 2,
		0,
		Constants.MAX_LOD
	)
	var chunk_obj := _terrain_chunk_dict[current_chunk] as TerrainChunk
	var old_lod = chunk_obj.lod
	
	if old_lod != simple_distance:
		chunk_obj.lod = simple_distance
		to_update_list[idx] = current_chunk
	else:
		to_update_list[idx] = null


func _update_chunk_mesh_in_thread(
	idx: int,
	to_update_list: Array
) -> void:
	var chunk_pos := to_update_list[idx] as Vector2i
	var chunk_obj := _terrain_chunk_dict[chunk_pos] as TerrainChunk
	
	chunk_obj.update_chunk()


func _update_chunks(current_chunk_pos: Vector2i) -> void:
	var chunks_list := _get_sorted_chunks_list(current_chunk_pos)
	var to_update_list: Array = []
	to_update_list.resize(chunks_list.size())
	
	var group_id: int = WorkerThreadPool.add_group_task(
		Callable(self, "_update_chunk_lod_in_thread").bind(
			current_chunk_pos, chunks_list, to_update_list
		), to_update_list.size()
	)
	while not WorkerThreadPool.is_group_task_completed(group_id):
#		if _should_thread_break() or _main_should_end:
		if _main_should_end:
			_workers_should_end = true
	WorkerThreadPool.wait_for_group_task_completion(group_id)
	_workers_should_end = false
	
	to_update_list = to_update_list.filter(
		func(chunk_pos):
			return chunk_pos != null
	)
	
	if to_update_list.size() == 0:
		return
	
	group_id = WorkerThreadPool.add_group_task(
		Callable(self, "_update_chunk_mesh_in_thread").bind(
			to_update_list
		), to_update_list.size()
	)
	while not WorkerThreadPool.is_group_task_completed(group_id):
#		if _should_thread_break() or _main_should_end:
		if _main_should_end:
			_workers_should_end = true
	WorkerThreadPool.wait_for_group_task_completion(group_id)
	_workers_should_end = false


func _clean_up():
	print("CLEAN_UP")
	for key in _terrain_chunk_dict:
		var chunk := _terrain_chunk_dict[key] as TerrainChunk
		chunk.remove_from_world()
		_terrain_chunk_dict[key] = null
	_terrain_chunk_dict = {}
	print("CLEAN_UP_DONE")


func _master_thread_loop() -> void:
#	_prepare_terrain_dict()
#
	while not _main_should_end:
		if _old_player_chunk_coord != _current_player_chunk_coord:
			_old_player_chunk_coord = _current_player_chunk_coord
			_update_chunks(_current_player_chunk_coord)
			OS.delay_msec(500)
	
	_clean_up()


func start_master_thread_loop() -> void:
	var res := _main_thread.start(
		Callable(self, "_master_thread_loop")
	)
	if res != OK:
		push_error("Failed to start thread: " + str(res))


func _get_chunk_data(world_pos: Vector2i) -> PackedFloat32Array:
	var result := PackedFloat32Array()
	result.resize(Constants.CHUNK_SIZE_P_1 * Constants.CHUNK_SIZE_P_1)
	
	for y in range(0, Constants.CHUNK_SIZE_P_1):
		for x in range(0, Constants.CHUNK_SIZE_P_1):
			var noise := _noise.get_noise_2d(
				x + world_pos.x, y + world_pos.y
			)
			result[y * Constants.CHUNK_SIZE_P_1 + x] = noise
	
	return result
	
