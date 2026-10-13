class_name NavChunk
extends Object


## Navigation chunk
## 
## Stores and handles [NavRegion]s for this [NavChunk]


signal _regions_baked
signal _source_geometry_parsed


## Region status enum
enum ChunkStatus {
	## Chunk has not yet been initialized
	EMPTY,
	## Regions are being updated
	IS_BAKING,
	## Chunk is ready
	READY,
	## Chunk is cleared and ready to be destroyed
	CLEARED
}


## Unique [NavChunk]'s id
var id: StringName:
	get: return _id


## [NavChunk]'s status
var status: ChunkStatus:
	get:
		_status_mutex.lock()
		var result := _status
		_status_mutex.unlock()
		return result
	set(value):
		_status_mutex.lock()
		_status = value
		_status_mutex.unlock()


var _status_mutex := Mutex.new()


var _id: StringName
var _status := ChunkStatus.EMPTY

var _regions: Dictionary[Global.CreatureSize, NavRegion]
var _aabb: AABB

var _to_bake: int


func _init(
		p_id: StringName,
		p_creature_sizes_regions: Dictionary[Global.CreatureSize, RID],
		p_nav_creature_datas: Dictionary[Global.CreatureSize, NavCreatureData],
		p_chunk_aabb: AABB
) -> void:
	_id = p_id
	
	_aabb = p_chunk_aabb
	
	_regions = {}
	for creature_size in p_creature_sizes_regions:
		var nav_region_id := "%s_REGION_%s" % [_id, Global.creature_size_to_str(creature_size)]
		var nav_region_rid := p_creature_sizes_regions[creature_size]
		var nav_region := NavRegion.new(
				nav_region_id,
				p_nav_creature_datas[creature_size],
				nav_region_rid)
		_regions[creature_size] = nav_region
	
	status = ChunkStatus.READY


## Request chunk bake async
func request_chunk_bake_async(
		p_root_node: Node,
		p_callback: Callable
) -> void:
	if status != ChunkStatus.READY:
		push_error("[NavChunk] <%s>: Chunk is in illegal state for this operation -> %s" % \
				[_id, ChunkStatus.keys()[status]])
		return
	status = ChunkStatus.IS_BAKING
	print("[NavChunk] <%s>: Chunk bake started" % _id)
	
	var source_geometry := await _parse_source_geometry_async(p_root_node)
	
	_to_bake = _regions.size()
	for creature_size in _regions:
		_regions[creature_size].request_bake.call_deferred(
				source_geometry,
				Global.create_chunk_nav_mesh(_id, _aabb),
				_region_finished_baking)
	
	await _regions_baked
	
	print("[NavChunk] <%s>: Chunk bake finished" % _id)
	status = ChunkStatus.READY
	p_callback.call_deferred()


## Clears [NavChunk] and frees all connected [NavRegion]s
func clear() -> void:
	status = ChunkStatus.CLEARED
	for creature_size in _regions:
		_regions[creature_size].clear()
		_regions[creature_size].free()
	_regions.clear()
	print("[NavChunk] <%s>: Cleared" % _id)


func _region_finished_baking() -> void:
	_to_bake -= 1
	if _to_bake == 0:
		_regions_baked.emit()


func _parse_source_geometry_async(p_root_node: Node) -> NavigationMeshSourceGeometryData3D:
	print("[NavChunk] <%s>: Parse source geometry started" % _id)
	var source_geometry := NavigationMeshSourceGeometryData3D.new()
	var callback: Callable = func() -> void:
			_source_geometry_parsed.emit()
	
	NavigationServer3D.call_deferred(
		NavigationServer3D.parse_source_geometry_data.get_method(),
		Global.create_chunk_nav_mesh(_id, _aabb),
		source_geometry,
		p_root_node,
		callback
	)
	
	await _source_geometry_parsed
	
	print("[NavChunk] <%s>: Parse source geometry finished" % _id)
	return source_geometry
