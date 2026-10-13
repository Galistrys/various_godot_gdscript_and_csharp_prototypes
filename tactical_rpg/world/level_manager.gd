class_name LevelManager
extends Node3D


## This class manages loading and unloading of levels and their data.


## Emits load progress. Value is between [code]0.0[/code] and [code]1.0[/code]
## inclusive
signal load_level_progress(value: float)


## [LevelManager] status enum
enum LevelManagerStatus {
	INVALID,
	IDLE,
	LOADING_LEVEL
}


const _LEVEL_DIR = "res://world/levels/"


var _status := LevelManagerStatus.INVALID


var _current_level: Level


static func _get_scene_path(p_level_id: String) -> String:
	return _LEVEL_DIR + p_level_id + "/" + p_level_id + ".tscn"


func _ready() -> void:
	pass


## Initialize level manager async.
func initialize_threaded() -> void:
	print("[LevelManager]: Initialization started")
	
	_status = LevelManagerStatus.IDLE
	
	print("[LevelManager]: Initialization finished")


## Add terrain and objects on the map to correct groups
func set_up_nav_data_async(p_chunks_grid_size: Vector2i) -> void:
	print("[LevelManager]: Nav data setup started")
	if _status != LevelManagerStatus.IDLE:
		push_error("[LevelManager]: Tried to change level when status is not IDLE")
		return
	_current_level.set_up_nav_data_async(p_chunks_grid_size)
	await get_tree().physics_frame
	print("[LevelManager]: Nav data setup finished")


## Retuns currently loaded [Level]'s name
func get_current_level_name() -> String:
	return _current_level.level_id


## Changes current level (or just loads a new one, if there is no currently
## loaded). It removes current level from [SceneTree], loads new one in the
## background, instatiates it, adds it to the [SceneTree] and initializes it.
func change_level_async(p_level_id: String) -> bool:
	if _status != LevelManagerStatus.IDLE:
		push_error("[LevelManager]: Tried to change level when status is not IDLE")
		return false
	
	print("[LevelManager]: Change level to \"%s\" requested" % p_level_id)
	
	if _current_level != null:
		print("[LevelManager]: Removing current level")
		_current_level.queue_free()
		await _current_level.tree_exited
		print("[LevelManager]: Current level removed")
	_current_level = null
	
	_status = LevelManagerStatus.LOADING_LEVEL
	var level_path := _get_scene_path(p_level_id)
	var error := ResourceLoader.load_threaded_request(
			_get_scene_path(p_level_id),
			"", true)
	if error != OK:
		push_error("[LevelManager]: Failed to load level \"%s\" because of an error -> %s" \
				% [p_level_id, error_string(error)])
		_status = LevelManagerStatus.IDLE
		return false
		
	print("[LevelManager]: Requested load of level resource")
	
	var end_flag := false
	var progress := []
	while not end_flag:
		var result := ResourceLoader.load_threaded_get_status(level_path, progress)
		
		match result:
			ResourceLoader.THREAD_LOAD_IN_PROGRESS:
				load_level_progress.emit(progress[0])
			ResourceLoader.THREAD_LOAD_LOADED:
				load_level_progress.emit(1.0)
				end_flag = true
			ResourceLoader.THREAD_LOAD_FAILED:
				push_error("[LevelManager]: Thread load failed")
				_status = LevelManagerStatus.IDLE
				return false
			ResourceLoader.THREAD_LOAD_INVALID_RESOURCE:
				push_error("[LevelManager]: Thread load invalid resource")
				_status = LevelManagerStatus.IDLE
				return false
		
		await get_tree().physics_frame
	
	print("[LevelManager]: Level resource loaded")
	print("[LevelManager]: Instantiate level")
	
	var level_scene := ResourceLoader.load_threaded_get(level_path) as PackedScene
	var level_instance := level_scene.instantiate() as Level
	
	print("[LevelManager]: Level instantiated")
	
	await get_tree().physics_frame
	
	print("[LevelManager]: Add level to SceneTree")
	
	add_child(level_instance)
	
	print("[LevelManager]: Level added to SceneTree")
	
	await get_tree().physics_frame
	
	print("[LevelManager]: Initialize level")
	await level_instance.initialize_async(p_level_id)
	print("[LevelManager]: Level initialized")
	
	_current_level = level_instance
	
	print("[LevelManager]: Change level to \"%s\" finished" % p_level_id)
	
	_status = LevelManagerStatus.IDLE
	
	return true
