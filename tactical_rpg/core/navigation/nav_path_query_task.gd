class_name NavPathQueryTask
extends NavTask


var _nav_map: NavMap
var _start_pos: Vector3
var _target_pos: Vector3
var _callback: Callable


func _init(
		p_nav_map: NavMap,
		p_start_pos: Vector3,
		p_target_pos: Vector3,
		p_callback: Callable
) -> void:
	_nav_map = p_nav_map
	_start_pos = p_start_pos
	_target_pos = p_target_pos
	_callback = p_callback


func execute() -> void:
	print("[NavPathQueryTask]: Execute %s -> %s" % [_start_pos, _target_pos])
	var path_query := _nav_map.get_path_query()
	var path_result := NavigationPathQueryResult3D.new()

	path_query.start_position = _start_pos
	path_query.target_position = _target_pos
	path_query.navigation_layers = 1

	NavigationServer3D.query_path(path_query, path_result)
	_callback.call_deferred(path_result.path)
	print("[NavPathQueryTask]: Finished %s -> %s" % [_start_pos, _target_pos])
