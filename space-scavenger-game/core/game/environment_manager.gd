class_name EnvironmentManager
extends Node3D


## This class manages environment related stuff, such as sun position and rotation,
## background, environmental effects, etc.


static var instance: EnvironmentManager


@warning_ignore("unused_private_class_variable")
@onready var _world_environment := $WorldEnvironment as WorldEnvironment
@warning_ignore("unused_private_class_variable")
@onready var _sun := $DirectionalLight3D as DirectionalLight3D


func _init() -> void:
	instance = self


func _exit_tree() -> void:
	instance = null


## Initialize [EnvironmentManager]. 
## This method is executed on a separate thread by [GameManager]
func initialize_threaded() -> void:
	print("[EnvironmentManager]: Initialization started")
	
	
	
	print("[EnvironmentManager]: Initialization finished")


func get_sun_basis() -> Basis:
	return _sun.global_basis
