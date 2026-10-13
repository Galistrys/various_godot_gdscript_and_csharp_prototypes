class_name PlayerState
extends RefCounted


var _state_machine = null


@warning_ignore(unused_private_class_variable)
var movement_input := Vector3.ZERO
var rotation_input := Vector3.ZERO


func _init(state_machine) -> void:
	_state_machine = state_machine


func on_enter(_args: Array) -> void:
	pass


func process(_delta: float) -> void:
	pass


func physics_process(_delta: float) -> void:
	pass


func on_exit() -> void:
	pass

func get_state_type() -> String: return get_script().resource_path.get_file()
