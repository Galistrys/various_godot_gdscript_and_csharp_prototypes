class_name PlayerFlyingState
extends PlayerMovementState


## State for [PlayerMovementStateMachine] when [Player] is in
## an environment without gravity


## State's ID
const ID = &"PlayerFlyingState"

## Speed acceleration
const FLY_ACCELERATION = 1.0
## Rotation acceleration
const ROTATION_ACCELERATION = 1.0

## Rotation speed input modifier
const PITCH_YAW_INPUT_MOD = 10.0
## sad
const ROLL_INPUT_MOD = 0.5


var _current_rotation_speed: Vector3 = Vector3.ZERO


@warning_ignore("unused_parameter")
func on_enter(data: Array) -> void:
	set_should_rotate(true)
	set_should_move(true)
	
	#_player.motion_mode = CharacterBody3D.MOTION_MODE_FLOATING
	
	# Workaround - currently, in MOTION_MODE_FLOATING, velocity is not updating
	# after calling move_and_slide
	_player.motion_mode = CharacterBody3D.MOTION_MODE_GROUNDED
	
	_player.enable_flying_posture()


@warning_ignore("unused_parameter")
func _rotate(delta: float) -> void:
	var input := InputService.get_rotation_vector()
	
	_current_rotation_speed = _current_rotation_speed.lerp(
			Vector3(
					input.y * PITCH_YAW_INPUT_MOD,
					input.x * PITCH_YAW_INPUT_MOD,
					input.z * ROLL_INPUT_MOD
			), delta * ROTATION_ACCELERATION
	)
	
	_player.rotate_object_local(
			Vector3.RIGHT, _current_rotation_speed.x * delta
	)
	_player.rotate_object_local(
			Vector3.UP, _current_rotation_speed.y * delta
	)
	_player.rotate_object_local(
			Vector3.BACK, _current_rotation_speed.z * delta
	)
	
	# Workaround - look above
	_player.up_direction = _player.global_basis.y


@warning_ignore("unused_parameter")
func _move(delta: float) -> void:
	var input := InputService.get_movement_vector()
	var vel := _player.global_basis * input * FLY_ACCELERATION * delta
	_player.velocity += vel
	
	_player.move_and_slide()
