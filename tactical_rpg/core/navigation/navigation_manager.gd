class_name NavigationManager
extends Node


## Manages navigation related processes.
##
## Handles creation, update and control of usage of navigation data.


## Emited when activation finishes
signal activated
## Emited when deactivation finishes
signal deactivated


## Navigation CreatureSize data dictionary
## @deprecated: It will be changed in the future to more dynamic mechanism
const _NAV_CREATURE_SIZES_DATA: Dictionary[Global.CreatureSize, NavCreatureData] = {
	Global.CreatureSize.CREATURE_TINY: preload("uid://buqbv36hprd88"),
	Global.CreatureSize.CREATURE_SMALL: preload("uid://xdlbmh5rwssl"),
	Global.CreatureSize.CREATURE_MEDIUM: preload("uid://dlcfgv4xa7gne"),
	Global.CreatureSize.CREATURE_LARGE: preload("uid://un38rjwxgjpa"),
	Global.CreatureSize.CREATURE_HUGE: preload("uid://i26iuu0i6irn")
}


var _nav_level: NavLevel

var _is_initialized := false
var _is_active := false
var _is_processing := false



func _unhandled_key_input(event: InputEvent) -> void:
	if event is InputEventKey:
		if event.pressed and event.keycode == KEY_K:
			request_chunk_update(Vector2i.ZERO)


func _physics_process(_delta: float) -> void:
	if not _is_active or not _is_processing:
		return
	if _nav_level.has_pending_updates() and not _nav_level.is_processing():
		_nav_level.process_tasks_async()


## Initializes [NavigationManager]
func initialize_threaded() -> void:
	print("[NavigationManager]: Initialization started")
	
	_is_initialized = true
	
	print("[NavigationManager]: Initialization finished")


## Activates [NavigationManager]
func activate_async(p_id: String, p_root_node: Node) -> void:
	print("[NavigationManager]: Activation started")
	if not _is_initialized or _is_active:
		return
	
	_nav_level = NavLevel.new()
	_nav_level.initialize_threaded(p_id, p_root_node, _NAV_CREATURE_SIZES_DATA)
	await get_tree().physics_frame
	
	_is_active = true
	
	activated.emit()
	
	print("[NavigationManager]: Activation finished")


## Deactivates [NavigationManager]
func deactivate_async() -> void:
	print("[NavigationManager]: Deactivation started")
	if not _is_initialized or not _is_active:
		return
	
	_is_active = false
	
	toggle_processing(false)
	
	await wait_for_nav_updates_to_finish_async()
	
	_nav_level.clear()
	_nav_level.free()
	_nav_level = null
	
	deactivated.emit()
	
	print("[NavigationManager]: Deactivation finished")


## Returns number of chunks in X and Z directions for the current level
func get_chunks_grid_size() -> Vector2i:
	return _nav_level.get_chunks_grid_size()


## Toggle updates processing
func toggle_processing(p_should_process: bool) -> void:
	_is_processing = p_should_process


## Waits async for all navigation updates and processes to finish.
## Returns [code]false[/code] if [NavigationManager] is not initialized,
## otherwise returns [code]true[/code].
func wait_for_nav_updates_to_finish_async() -> bool:
	if not _is_initialized:
		return false
	
	var end_flag := true
	while end_flag:
		await get_tree().physics_frame
		end_flag = _nav_level.has_pending_updates() or _nav_level.is_processing()
	
	return true


## Request an update for chunk at [param p_coords].
## Returns [code]false[/code] if [NavigationManager] is not initialized,
## otherwise returns [code]true[/code].
func request_chunk_update(p_coords: Vector2i) -> bool:
	if not _is_initialized:
		return false
	_nav_level.add_chunk_to_update(p_coords)
	
	return true


## Request path calculation for creature size of [param p_creature_size],
## from [param p_start_pos] to [param p_target_pos]. After path has been
## calculated, it calls [param p_callback] with a parameter of type
## [PackedVector3Array].
func request_path(
		p_creature_size: Global.CreatureSize,
		p_start_pos: Vector3,
		p_target_pos: Vector3,
		p_callback: Callable
) -> void:
	_nav_level.request_path(p_creature_size, p_start_pos, p_target_pos, p_callback)


## Returns path calculated for creature size of [param p_creature_size],
## from [param p_start_pos] to [param p_target_pos]. The path,
## is returned without waiting for updates to finish, so it may not be
## synchronized with current state of the level.
func get_path_now(
		p_creature_size: Global.CreatureSize,
		p_start_pos: Vector3,
		p_target_pos: Vector3
) -> PackedVector3Array:
	return _nav_level.get_path_now(p_creature_size, p_start_pos, p_target_pos)
