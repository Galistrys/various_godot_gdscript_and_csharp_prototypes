class_name EntitiesManager
extends Node3D


## This class manages loading, unloading and management for entities


static var instance: EntitiesManager


@export var _player_packed_scene: PackedScene
@export var _grabbable_packed_scene: PackedScene


func _init() -> void:
	instance = self


func _exit_tree() -> void:
	instance = null


## Initialize [EntitiesManager]. 
## This method is executed on a separate thread by [GameManager]
func initialize_threaded() -> void:
	print("[EntitiesManager]: Initialization started")
	
	pass
	
	print("[EntitiesManager]: Initialization finished")


## @experimental
## Only for testing
func spawn_player(pos: Vector3) -> void:
	var player := _player_packed_scene.instantiate() as Player
	player.position = pos
	add_child(player)


## @experimental
## Only for testing
func spawn_test_cube(pos: Vector3) -> void:
	var grabbable_object := _grabbable_packed_scene.instantiate() as GrabbableObject
	var data := load("res://data/test_objects/test_cube/test_cube_grabbable_data.tres") as GrabbableData
	grabbable_object.position = pos
	add_child(grabbable_object)
	grabbable_object.initialize(data)
