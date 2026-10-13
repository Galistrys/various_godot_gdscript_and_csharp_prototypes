class_name NavRegion
extends Object


## Navigation region data for creature type
## 
## Stores information and provides interface for manipulating creature's region


## Region status enum
enum RegionStatus {
	## Region has not yet been initialized
	EMPTY,
	## Navigation mesh is being baked
	IS_BAKING,
	## Region is ready
	READY,
	## Region is cleared and ready to be destroyed
	CLEARED
}


## Unique [NavRegion]'s id
var id: StringName:
	get: return _id


## [NavRegion]'s status
var status: RegionStatus:
	get: return _status



var _id: StringName
var _region_rid: RID
var _creature_data: NavCreatureData
var _nav_mesh: NavigationMesh
var _status := RegionStatus.EMPTY


func _init(
		p_id: StringName,
		p_creature_data: NavCreatureData,
		p_region_rid: RID
) -> void:
	_id = p_id
	_creature_data = p_creature_data
	_region_rid = p_region_rid
	
	NavigationServer3D.region_set_enabled(_region_rid, true)
	
	_status = RegionStatus.READY


## Requests region baking. If region is already baked, then rebakes it.
## After process is finished it calls [param callback].
func request_bake(
		p_source_geometry: NavigationMeshSourceGeometryData3D,
		p_nav_mesh: NavigationMesh,
		p_callback: Callable
) -> bool:
	#print("[NavRegion] <%s>: Requested bake" % _id)
	if _status != RegionStatus.READY:
		push_error("[NavRegion] <%s>: Region is in illegal state for baking -> %s" % \
				[_id, RegionStatus.keys()[_status]])
		return false
	
	_status = RegionStatus.IS_BAKING
	
	p_nav_mesh.agent_radius = _creature_data.creature_radius
	p_nav_mesh.agent_height = _creature_data.creature_height
	
	NavigationServer3D.bake_from_source_geometry_data_async(
			p_nav_mesh, p_source_geometry, _finished_baking.bind(p_nav_mesh, p_callback)
	)
	return true


## Prepares region to be destroyed. Region is unusable after executing this method
func clear() -> void:
	if _status != RegionStatus.READY:
		push_error("[NavRegion] <%s>: Region is in illegal state for baking -> %s" % \
				[_id, RegionStatus.keys()[_status]])
	NavigationServer3D.free_rid(_region_rid)
	_region_rid = RID()
	_nav_mesh = null
	_status = RegionStatus.CLEARED
	print("[NavRegion] <%s>: Cleared" % _id)


func _finished_baking(p_nav_mesh: NavigationMesh, p_callback: Callable) -> void:
	#print("[NavRegion] <%s>: Finished baking" % _id)
	NavigationServer3D.region_set_navigation_mesh(_region_rid, p_nav_mesh)
	_nav_mesh = p_nav_mesh
	
	#if _id.contains("MEDIUM") and _id.contains("_1_1_"):
		#ResourceSaver.save(_nav_mesh, str("res://debug/nav_mesh_debug/nvm_", _id ,".res"))
	
	_status = RegionStatus.READY
	if p_callback:
		p_callback.call()
