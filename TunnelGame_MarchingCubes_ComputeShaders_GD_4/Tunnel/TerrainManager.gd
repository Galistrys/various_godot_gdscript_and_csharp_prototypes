extends Node3D
class_name TerrainManager

const CHUNK_SIZE = ChunkMeshGenerator.CHUNK_SIZE

const WORLD_DATA_DIR = "user://WorldData"
const MESH_EXT = ".mesh"
const EMPTY_EXT = ".empty"
var _world_name = "TestWorld"
var _path_to_world_dir = WORLD_DATA_DIR + "/" + _world_name

@export var render_distance: int = 2
@export var num_of_threads: int = 4

@export var node_to_observe_path: NodePath
var _node_to_observe: Node3D

var _old_chunk_pos: Vector3 = Vector3.ONE
var _current_chunk_pos: Vector3 = Vector3.ZERO

enum ChunkState { NONE, LOADED, TO_REMOVE }
var _chunks_state: Dictionary = {}
var _chunks_data: Dictionary = {}

var _master_should_end: bool = false
var _workers_should_end: bool = false

var _master_thread: Thread = Thread.new()

func _ready():
	if not DirAccess.dir_exists_absolute(_path_to_world_dir):
		DirAccess.make_dir_recursive_absolute(_path_to_world_dir)
	
	_node_to_observe = get_node_or_null(node_to_observe_path)
	if _node_to_observe == null:
		_node_to_observe = self
	
	if not Engine.is_editor_hint():
		var res = _master_thread.start(Callable(self, "_master_thread_process"))
		if res != OK:
			push_error("Failed to start thread: " + str(res))

func _process(_delta):
	_current_chunk_pos = world_to_chunk_pos(_node_to_observe.position)
	print(_current_chunk_pos)

func _exit_tree():
	_master_should_end = true
	_master_thread.wait_to_finish()

func world_to_chunk_pos(world_pos: Vector3) -> Vector3:
	return Vector3((world_pos / ChunkMeshGenerator.CHUNK_SIZE).floor())

func vector_to_key(vec: Vector3) -> String:
	return str(vec.x, "_", vec.y, "_", vec.z)

func pos_to_key(x: int, y: int, z: int) -> String:
	return str(x, "_", y, "_", z)

func key_to_path(key: String, ext: String) -> String:
	return _path_to_world_dir + "/" + key + ext

func _should_thread_break() -> bool:
	return _current_chunk_pos != _old_chunk_pos

#func _get_chunk_priority(
#	chunk_world_pos: Vector3, x1: Vector3, x2: Vector3,
#	x2_x1: Vector3, x2_x1_l: float
#) -> float:
#	var distance_from_player = x2.distance_squared_to(_current_chunk_pos)
#	var distance_from_screen_center = x2_x1.cross(x1 - chunk_world_pos).length() / x2_x1_l
#
#	return -(distance_from_player + distance_from_screen_center)

func _worker_check_chunk(
	id: int,
	to_check_list: Array, to_add_list: Array,
	current_chunk_pos: Vector3
) -> void:
	
	var chunk_position: Vector3 = to_check_list[id]
	var key = vector_to_key(chunk_position)
	
	if not _chunks_state.has(key):
		var priority = chunk_position.distance_squared_to(current_chunk_pos)
		to_add_list[id] = [key, chunk_position * CHUNK_SIZE, priority]
	else:
		to_add_list[id] = null
		_chunks_state[key] = ChunkState.LOADED

func _load_chunk_from_file(_key: String) -> Chunk:
	return null

func _generate_chunk(key: String, world_pos: Vector3) -> Chunk:
	var chunk = Chunk.new(key)
	chunk.generate(world_pos, Chunk.DetailLevel.LEVEL_4)
#
#	if key == "0_0_0":
#		ResourceSaver.save(chunk.mesh, "res://test_mesh.mesh")
	
	return chunk

func _worker_load_chunk(
	id: int,
	to_add_list: Array,
	chunks_to_add: Array[Chunk]
) -> void:
	if _workers_should_end:
		return
	
	var data = to_add_list[id]
	
	var key = data[0]
	var world_pos = data[1]
	
#	var file: File = File.new()
	var chunk: Chunk = Chunk.new(key)
	
	if FileAccess.file_exists(key_to_path(key, EMPTY_EXT)):
		chunks_to_add[id] = chunk
		return
	elif FileAccess.file_exists(key_to_path(key, MESH_EXT)):
		chunk = _load_chunk_from_file(key)
	else:
		chunk = _generate_chunk(key, world_pos)
	
#	if chunk.is_empty():
#		if file.open(key_to_path(key, EMPTY_EXT), File.WRITE) == OK:
#			file.store_line("EMPTY")
#			file.close()
	
	chunks_to_add[id] = chunk

func _get_to_check_list(current_chunk_pos: Vector3) -> Array:
	var to_check_list: Array = []
	for x in range(current_chunk_pos.x - render_distance, current_chunk_pos.x + render_distance + 1):
		for y in range(current_chunk_pos.y - render_distance, current_chunk_pos.y + render_distance + 1):
			for z in range(current_chunk_pos.z - render_distance, current_chunk_pos.z + render_distance + 1):
				to_check_list.append(Vector3(x, y, z))
	return to_check_list

func _get_to_add_list(to_check_list: Array, current_chunk_pos: Vector3) -> Array:
	var to_add_list: Array = []
	to_add_list.resize(to_check_list.size())
	
	var group_id: int = WorkerThreadPool.add_group_task(
		Callable(self, "_worker_check_chunk").bind(to_check_list, to_add_list, current_chunk_pos),
		to_check_list.size(), num_of_threads
	)
	WorkerThreadPool.wait_for_group_task_completion(group_id)
	
	for i in range(to_add_list.size() - 1, -1, -1):
		if to_add_list[i] == null:
			to_add_list.remove_at(i)
	
	to_add_list.sort_custom(func(a, b): return a[2] < b[2])
	
	return to_add_list

func _update_chunks(current_chunk_pos: Vector3) -> void:
	var to_check_list: Array = _get_to_check_list(current_chunk_pos)
	var to_add_list: Array = _get_to_add_list(to_check_list, current_chunk_pos)
	
	if to_add_list.is_empty():
		return
	
	print("TO ADD: ", to_add_list.size())
	
	var chunks_to_add: Array[Chunk] = []
	chunks_to_add.resize(to_add_list.size())
	
	
	var group_id = WorkerThreadPool.add_group_task(
		Callable(self, "_worker_load_chunk").bind(to_add_list, chunks_to_add),
		to_add_list.size(), num_of_threads
	)
	
	while not WorkerThreadPool.is_group_task_completed(group_id):
		if _should_thread_break() or _master_should_end:
			_workers_should_end = true
	WorkerThreadPool.wait_for_group_task_completion(group_id)
	
	_workers_should_end = false
	
	for cta in chunks_to_add:
		if cta != null:
			_chunks_state[cta.key] = ChunkState.LOADED
			_chunks_data[cta.key] = cta
	
#	for c_k in _chunks_data:
#		var c_d = _chunks_data[c_k] as Chunk
#		printt(c_k, c_d.is_empty())

func _remove_process() -> void:
	var keys = _chunks_state.keys()
	for chunk_key in keys:
		var chunk_state = _chunks_state[chunk_key]
		if chunk_state == ChunkState.TO_REMOVE:
			_chunks_state.erase(chunk_key)
			_chunks_data[chunk_key].remove_from_world()
			_chunks_data.erase(chunk_key)
	
	for chunk_key in _chunks_state.keys():
		if _chunks_state[chunk_key] == ChunkState.LOADED:
			_chunks_state[chunk_key] = ChunkState.TO_REMOVE

func _master_thread_process():
	
	while not _master_should_end:
		_old_chunk_pos = _current_chunk_pos
		
		_update_chunks(_current_chunk_pos)
		_remove_process()
