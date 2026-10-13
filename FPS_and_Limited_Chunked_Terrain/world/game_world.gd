class_name GameWorld
extends Node3D

@onready var _terrain := $Terrain as Terrain

var _load_thread := Thread.new()

func _ready():
	if not Engine.is_editor_hint():
		var res = _load_thread.start(
			Callable(self, "_load_in_thread")
		)
		if res != OK:
			push_error("Failed to start thread: " + str(res))


func _load_thread_finished() -> void:
	_terrain.start_master_thread_loop()


func _load_in_thread() -> void:
	_terrain.prepare_terrain_dict_threaded()
	call_deferred("_load_thread_finished")
