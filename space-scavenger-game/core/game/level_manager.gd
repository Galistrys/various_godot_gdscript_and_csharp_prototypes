class_name LevelManager
extends Node3D


## This class manages loading and unloading of levels and their data.


## Emits progress on a level loading.
## The [param value] is between [code]0.0[/code] and [code]1.0[/code]
signal load_level_progress(value: float)


## [LevelManager] status indicator
enum LevelManagerStatus {
	INVALID,
	IDLE,
	LOADING_LEVEL
}


const LEVEL_DIR = "res://levels/"


var _status := LevelManagerStatus.INVALID

var _current_level: Level


func _ready() -> void:
	pass


## Initialize [LevelManager]. 
## This method is executed on a separate thread by [GameManager]
func initialize_threaded() -> void:
	print("[LevelManager]: Initialization started")
	
	
	
	_status = LevelManagerStatus.IDLE
	print("[LevelManager]: Initialization finished")


## Changes level asynchronizly to [param p_level_id].
## Level scene must be in a path:
## [constant LevelManager.LEVEL_DIR]/[param p_level_id]/[param p_level_id].tscn
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
	
	print("[LevelManager]: Change level to \"%s\" finished" % p_level_id)
	_current_level = level_instance
	_status = LevelManagerStatus.IDLE
	return true


func _get_scene_path(p_level_id: String) -> String:
	return LEVEL_DIR + p_level_id + "/" + p_level_id + ".tscn"
