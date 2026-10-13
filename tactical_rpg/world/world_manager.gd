class_name WorldManager
extends Node3D


## Manages 3D world for the game


const _SYNC_WAIT = 0.25


@onready var _level_manager := $LevelManager as LevelManager
@onready var _player_camera: PlayerCamera = $PlayerCamera


## Initializes [WorldManager]
func initialize_threaded() -> void:
	print("[WorldManager]: Initialization started")
	
	_level_manager.initialize_threaded()
	
	_player_camera.initialize_threaded()
	
	print("[WorldManager]: Initialization finished")


## Changes current level to [param p_level_id].
## It deactivates [NavigationManager], requests [LevelManager] to change level,
## activates [NavigationManager], sets up navigation groups in the level and
## waits for [NavigationManager] to finish baking navigation meshes.
func change_level_async(p_level_id: String) -> void:
	print("[WorldManager]: Change level requested -> %s" % p_level_id)
	
	await NavigationService.deactivate_async()
	
	await _wait_async()
	
	await _level_manager.change_level_async(p_level_id)
	
	await _wait_async()
	
	await get_tree().create_timer(1.0).timeout
	
	await NavigationService.activate_async(p_level_id, _level_manager)
	await _level_manager.set_up_nav_data_async(NavigationService.get_chunks_grid_size())
	
	await _wait_async()
	
	NavigationService.toggle_processing(true)
	await NavigationService.wait_for_nav_updates_to_finish_async()
	
	print("[WorldManager]: Change level finished -> %s" % p_level_id)


func _wait_async() -> void:
	var wait_count := ceili(_SYNC_WAIT / (1.0 / Engine.physics_ticks_per_second))
	for i in range(wait_count):
		await get_tree().physics_frame
