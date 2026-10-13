class_name GameManager
extends Node


## This class orchestrates the game flow, combining World with UI


@onready var _world_manager := $WorldManager as WorldManager


func _ready() -> void:
	ready.connect(_initialize_async, ConnectFlags.CONNECT_ONE_SHOT)


func _initialize_threaded() -> void:
	_world_manager.initialize_threaded()


func _initialize_async() -> void:
	print("[GameManager]: Initialization started")
	
	var _thread := Thread.new()
	var error := _thread.start(_initialize_threaded)
	if error != OK:
		push_error("[GameManager]: Failed to start initialization thread")
		OS.crash("Failed to start initialization thread")
	
	while _thread.is_alive():
		await get_tree().physics_frame
	_thread.wait_to_finish()
	
	print("[GameManager]: Initialization finished")
	
	await _world_manager.change_level_async("test_level")
	
	_world_manager.spawn_player(Vector3(0.0, 2.0, 0.0))
	
	_world_manager.spawn_test_cube(Vector3(2.0, 4.0, 2.0))
