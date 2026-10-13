class_name WorldManager
extends Node3D


## This class manages World and orchestrates work for child managers

@onready var _environment_manager := $EnvironmentManager as EnvironmentManager
@onready var _level_manager := $LevelManager as LevelManager
@onready var _entities_manager := $EntitiesManager as EntitiesManager
@onready var _ships_manager := $ShipsManager as ShipsManager


func _ready() -> void:
	pass


## Initialize [WorldManager]. 
## This method is executed on a separate thread by [GameManager]
func initialize_threaded() -> void:
	print("[WorldManager]: Initialization started")
	
	_environment_manager.initialize_threaded()
	_level_manager.initialize_threaded()
	_entities_manager.initialize_threaded()
	_ships_manager.initialize_threaded()
	
	print("[WorldManager]: Initialization finished")


## Commands [LevelManager] to change level to [param p_level_id]
func change_level_async(p_level_id: String) -> bool:
	return await _level_manager.change_level_async(p_level_id)


## @experimental
func spawn_player(pos: Vector3) -> void:
	_entities_manager.spawn_player(pos)


## @experimental
func spawn_test_cube(pos: Vector3) -> void:
	_entities_manager.spawn_test_cube(pos)
