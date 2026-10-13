class_name FlyingPlayerState
extends MovementPlayerState


const ACCELERATION = 1
const ROTATION_ACCELERATION = 0.25
const MOUSE_CAP = 10.0

var _current_rotation_speed: Vector3 = Vector3.ZERO

var _character: CharacterBody3D
var _head: Node3D
var _standing_col: CollisionShape3D
var _fly_col: CollisionShape3D


func _init(
	character: CharacterBody3D,
	head: Node3D,
	standing_col: CollisionShape3D,
	fly_col: CollisionShape3D
):
	_character = character
	_head = head
	_standing_col = standing_col
	_fly_col = fly_col


func on_enter(_args: Array) -> void:
	_fly_col.set_deferred("disabled", false)
	_standing_col.set_deferred("disabled", true)
	
	_character.global_transform = _head.global_transform
	_head.rotation = Vector3.ZERO


func rotate(
	delta: float,
	input: Vector3
) -> void:
	input.x *= 0.5
	input.y *= 0.5
	input.z *= 2
	_current_rotation_speed = _current_rotation_speed.lerp(
		input, delta * ROTATION_ACCELERATION
	)
	
	_character.rotate_object_local(
		Vector3.RIGHT, _current_rotation_speed.x * delta
	)
	_character.rotate_object_local(
		Vector3.UP, _current_rotation_speed.y * delta
	)
	_character.rotate_object_local(
		Vector3.BACK, _current_rotation_speed.z * delta
	)
	_character.up_direction = _character.transform.basis.y

func move(
	delta: float,
	input: Vector3
) -> void:
	var vel = _character.transform.basis * input * ACCELERATION * delta
	_character.velocity += vel
	
	@warning_ignore(return_value_discarded)
	_character.move_and_slide()
