class_name DockingPort
extends Area3D

@onready var _camera: Camera3D = $DockingCamera

func get_camera() -> Camera3D:
	return _camera

#func move_reference_object(
#	input: Vector3
#) -> void:
#	var direction = global_transform.basis * input
#	_reference_object_node.apply_thrust(direction)
#
#func rotate_reference_object(
#	input: Vector3
#) -> void:
#	input.x *= 0.5
#	input.y *= 0.5
#	input.z *= 2
#
#	var dir = transform.basis * input * 0.25
#	_reference_object_node.apply_angular_thrust(dir)

