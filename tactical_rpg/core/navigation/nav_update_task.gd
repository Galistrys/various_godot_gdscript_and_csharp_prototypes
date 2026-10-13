class_name NavUpdateTask
extends NavTask


var _root_node: Node
var _nav_chunks: Dictionary[Vector2i, NavChunk]

var _to_update: Array[Vector2i]


func _init(
		p_root_node: Node,
		p_nav_chunks: Dictionary[Vector2i, NavChunk]
) -> void:
	_root_node = p_root_node
	_nav_chunks = p_nav_chunks
	
	_to_update = []


func execute() -> void:
	print("[NavUpdateTask]: Execute")
	var group_id := WorkerThreadPool.add_group_task(
			_process_update_chunk.bind(_to_update), _to_update.size())
	WorkerThreadPool.wait_for_group_task_completion(group_id)
	print("[NavUpdateTask]: Finished")


func add_chunk_to_update(p_coords: Vector2i) -> void:
	if not _to_update.has(p_coords):
		_to_update.append(p_coords)


func get_chunks_to_update_count() -> int:
	return _to_update.size()


func _process_update_chunk(element: int, to_update: Array[Vector2i]) -> void:
	var chunk_coords := to_update[element]
	var nav_chunk := _nav_chunks[chunk_coords]
	nav_chunk.request_chunk_bake_async(_root_node, Callable())
	OS.delay_msec(Global.NAV_TASK_DELAY)
	while nav_chunk.status != NavChunk.ChunkStatus.READY:
		OS.delay_msec(Global.NAV_TASK_DELAY)
