class_name NavMap
extends Object


## Navigation map data for creature type
## 
## Stores information and provides interface
## for manipulating creature navigation map


## [NavMap]'s status enum
enum MapStatus {
	## [NavMap] has not yet been initialized
	EMPTY,
	## [NavMap] is initialized
	READY,
	## [NavMap] is cleared and ready to be destroyed
	CLEARED
}

## [NavMap]'s id
var id: StringName:
	get:
		return _id


## [NavMap]'s status
var status: MapStatus:
	get:
		return _status


var _id: StringName
var _map_rid: RID
var _status: MapStatus

var _map_iteration_id: int = 0


func _init(
		p_id: StringName
) -> void:
	_id = p_id
	
	_map_rid = NavigationServer3D.map_create()
	NavigationServer3D.map_set_up(_map_rid, Vector3.UP)
	NavigationServer3D.map_set_active(_map_rid, true)
	NavigationServer3D.map_set_cell_size(_map_rid, Global.NAV_CELL_SIZE)
	NavigationServer3D.map_set_cell_height(_map_rid, Global.NAV_CELL_HEIGHT)
	
	_status = MapStatus.READY


## Create new region [RID] for this [NavMap]
func create_region_rid() -> RID:
	var region_rid := NavigationServer3D.region_create()
	NavigationServer3D.region_set_map(region_rid, _map_rid)
	
	return region_rid


## Returns preconfigured [NavigationPathQueryParameters3D] with this [NavMap]'s map [RID]
func get_path_query() -> NavigationPathQueryParameters3D:
	var result := NavigationPathQueryParameters3D.new()
	result.map = _map_rid
	return result


## Waits for map's iteration id to change.
## 
## [color=yellow]WARNING![/color] This method uses "sleep" on current thread.
## Use carefully!
func wait_for_iteration_id_to_change() -> void:
	print("[NavMap] <%s>: Wait for iteration id to change -> %d" % [_id, _map_iteration_id])
	var new_iteration_id := NavigationServer3D.map_get_iteration_id(_map_rid)
	var end_flag := new_iteration_id == _map_iteration_id
	while end_flag:
		OS.delay_msec(Global.NAV_TASK_DELAY)
		new_iteration_id = NavigationServer3D.map_get_iteration_id(_map_rid)
		end_flag = new_iteration_id == _map_iteration_id
	_map_iteration_id = new_iteration_id
	print("[NavMap] <%s>: Wait for iteration id to change finished -> %d" % [_id, _map_iteration_id])


## Prepares map to be destroyed. Map is unusable after executing this method
func clear() -> void:
	if _status != MapStatus.READY:
		push_error("[NavMap] <%s>: Map is in illegal state for baking -> %s" % \
				[_id, MapStatus.keys()[_status]])
	_status = MapStatus.CLEARED
	
	NavigationServer3D.free_rid(_map_rid)
	_map_rid = RID()
	
	print("[NavMap] <%s>: Cleared" % _id)
