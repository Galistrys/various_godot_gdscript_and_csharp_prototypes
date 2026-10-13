class_name NavLevel
extends Object


## Navigation level layer.
## 
## Handles creation and management over [NavChunk]s for currently loaded level.


signal _updates_processed_thread
signal _source_geometry_parsed


## Status enum
enum NavLevelStatus {
	## [NavLevel] is not initialized yet
	EMPTY,
	## [NavLevel] is processing tasks
	IS_PROCESSING,
	## [NavLevel] is initialized and ready for work
	READY,
	## [NavLevel] is deinitialized and cannot do any work
	CLEARED
}


## [NavLevel]'s id
var id: StringName:
	get: return _id

## [NavLevel]'s status
var status: NavLevelStatus:
	get: return _status


var _id: StringName
var _status: NavLevelStatus = NavLevelStatus.EMPTY

var _nav_maps: Dictionary[Global.CreatureSize, NavMap]
var _map_bounds: AABB

var _chunks_x: int
var _chunks_z: int

var _nav_chunks: Dictionary[Vector2i, NavChunk]
var _root_node: Node

var _update_task: NavUpdateTask
var _tasks: Array[NavTask]


## Initializes [NavLevel]
func initialize_threaded(
	p_id: StringName,
	p_root_node: Node,
	p_creature_sizes: Dictionary[Global.CreatureSize, NavCreatureData]
) -> void:
	_id = p_id
	
	_nav_maps = {}
	_nav_chunks = {}
	
	_root_node = p_root_node
	_update_task = NavUpdateTask.new(p_root_node, _nav_chunks)
	
	for creature_size in p_creature_sizes:
		var nav_map := NavMap.new(
				"%s_MAP_%s" % [_id, Global.creature_size_to_str(creature_size)])
		_nav_maps[creature_size] = nav_map
	
	var source_geometry := _parse_source_geometry_async()
	_map_bounds = source_geometry.get_bounds()
	
	@warning_ignore("integer_division")
	_chunks_x = int(_map_bounds.size.x) / Global.NAV_CHUNK_SIZE
	@warning_ignore("integer_division")
	_chunks_z = int(_map_bounds.size.z) / Global.NAV_CHUNK_SIZE
	
	for x in range(_chunks_x):
		for z in range(_chunks_z):
			var chunk_coords := Vector2i(x, z)
			var creature_sizes_regions: Dictionary[Global.CreatureSize, RID] = {}
			
			for creature_size in _nav_maps:
				var nav_map := _nav_maps[creature_size]
				creature_sizes_regions[creature_size] = nav_map.create_region_rid()
			
			var nav_chunk := NavChunk.new(
					Global.get_nav_group_name(_id, chunk_coords),
					creature_sizes_regions,
					p_creature_sizes,
					_get_chunk_aabb(chunk_coords)
			)
			_nav_chunks[chunk_coords] = nav_chunk
			_update_task.add_chunk_to_update(chunk_coords)
	
	_tasks = []
	
	_status = NavLevelStatus.READY


## Process all queued tasks in background thread
func process_tasks_async() -> bool:
	if _status != NavLevelStatus.READY or not has_pending_updates():
		return false
		
	print("[NavLevel] <%s>: Process tasks start" % _id)
	var _TIME_START := Time.get_ticks_usec()
	_status = NavLevelStatus.IS_PROCESSING
	
	var tasks_list := _tasks
	_tasks = []
	var update_task := _update_task
	_update_task = NavUpdateTask.new(_root_node, _nav_chunks)
	
	var thread := Thread.new()
	var err := thread.start(_process_updates_thread.bind(update_task, tasks_list))
	if err != Error.OK:
		push_error("[NavLevel] <%s>: Failed to start thread")
		return false
	
	await _updates_processed_thread
	thread.wait_to_finish()
	
	_status = NavLevelStatus.READY
	var _TIME_END := Time.get_ticks_usec()
	print("[NavLevel] <%s>: Process tasks finished in %.3f ms" \
			% [_id, float(_TIME_END - _TIME_START) / 1000.0])
	return true


## Add chunk to update
func add_chunk_to_update(p_coords: Vector2i) -> void:
	_update_task.add_chunk_to_update(p_coords)


## Request path calculation
func request_path(
		p_creature_size: Global.CreatureSize,
		p_start_pos: Vector3,
		p_target_pos: Vector3,
		p_callback: Callable
) -> void:
	_tasks.append(NavPathQueryTask.new(
		_nav_maps[p_creature_size],
		p_start_pos,
		p_target_pos,
		p_callback
	))


## Calculate path instantly. May be desynced with current state of a level
func get_path_now(
		p_creature_size: Global.CreatureSize,
		p_start_pos: Vector3,
		p_target_pos: Vector3
) -> PackedVector3Array:
	var path_query := _nav_maps[p_creature_size].get_path_query()
	var path_result := NavigationPathQueryResult3D.new()

	path_query.start_position = p_start_pos
	path_query.target_position = p_target_pos
	path_query.navigation_layers = 1

	NavigationServer3D.query_path(path_query, path_result)
	return path_result.path


## Returns [code]true[/code] if [NavLevel] is processing tasks
func is_processing() -> bool:
	return _status == NavLevelStatus.IS_PROCESSING


## Returns [code]true[/code] if [NavLevel] has tasks pending
func has_pending_updates() -> bool:
	return _tasks.size() > 0 or _update_task.get_chunks_to_update_count() > 0


## Returns number of chunks in X and Z directions
func get_chunks_grid_size() -> Vector2i:
	return Vector2i(_chunks_x, _chunks_z)


## Clears [NavLevel] and frees all connected [NavChunk]s
func clear() -> void:
	if _status != NavLevelStatus.READY:
		push_error("[NavLevel] <%s>: NavLevel is in illegal state for this operation -> %s" % \
				[_id, NavLevelStatus.keys()[_status]])
	
	_tasks.clear()
	_update_task = null
	
	for cc in _nav_chunks:
		var nav_chunk := _nav_chunks[cc]
		nav_chunk.clear()
		nav_chunk.free()
	_nav_chunks.clear()
	
	for cs in _nav_maps:
		var nav_map := _nav_maps[cs]
		nav_map.clear()
		nav_map.free()
	_nav_maps.clear()
	
	_root_node = null
	
	_status = NavLevelStatus.CLEARED
	print("[NavLevel] <%s>: Cleared" % _id)


func _process_updates_thread(p_update_task: NavUpdateTask, p_tasks: Array[NavTask]) -> void:
	print("[NavLevel] <%s>: Process updates thread" % _id)
	if p_update_task.get_chunks_to_update_count() > 0:
		p_update_task.execute()
		print("[NavLevel] <%s>: Wait for NavMaps to update" % _id)
		for cs in _nav_maps:
			_nav_maps[cs].wait_for_iteration_id_to_change()
		print("[NavLevel] <%s>: Wait for NavMaps finished" % _id)
	for tasks in p_tasks:
		tasks.execute()
	print("[NavLevel] <%s>: Process updates thread finished" % _id)
	self.call_deferred(emit_signal.get_method(), _updates_processed_thread.get_name())


func _parse_source_geometry_async() -> NavigationMeshSourceGeometryData3D:
	print("[NavLevel] <%s>: Parse source geometry started" % _id)
	var source_geometry := NavigationMeshSourceGeometryData3D.new()
	var callback: Callable = func() -> void:
			_source_geometry_parsed.emit()
	
	NavigationServer3D.parse_source_geometry_data(
			Global.create_dummy_nav_mesh(),
			source_geometry,
			_root_node,
			callback
	)
	
	#await _source_geometry_parsed
	print("[NavLevel] <%s>: Parse source geometry finished" % _id)
	return source_geometry


func _get_chunk_aabb(p_chunk_coords: Vector2i) -> AABB:
	return AABB(
			Vector3(
					p_chunk_coords.x * Global.NAV_CHUNK_SIZE,
					_map_bounds.position.y,
					p_chunk_coords.y * Global.NAV_CHUNK_SIZE),
			Vector3(
					Global.NAV_CHUNK_SIZE,
					_map_bounds.size.y,
					Global.NAV_CHUNK_SIZE)
	)
