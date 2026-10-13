class_name ShipController
extends AnimatableBody3D


func _ready() -> void:
	pass


func _physics_process(delta: float) -> void:
	var move_vector := Vector3.ZERO
	var rot_vector := Vector3.ZERO
	
	if Input.is_key_pressed(KEY_I):
		move_vector += Vector3.FORWARD
	if Input.is_key_pressed(KEY_J):
		move_vector += Vector3.LEFT
	if Input.is_key_pressed(KEY_K):
		move_vector += Vector3.BACK
	if Input.is_key_pressed(KEY_L):
		move_vector += Vector3.RIGHT
	if Input.is_key_pressed(KEY_U):
		move_vector += Vector3.UP
	if Input.is_key_pressed(KEY_O):
		move_vector += Vector3.DOWN
	if Input.is_key_pressed(KEY_N):
		rot_vector += Vector3.UP
	if Input.is_key_pressed(KEY_M):
		rot_vector += Vector3.DOWN
	
	move_and_collide(global_basis * move_vector * delta)
	if rot_vector.length_squared() != 0:
		rotate_object_local(rot_vector, delta * 1.0)
