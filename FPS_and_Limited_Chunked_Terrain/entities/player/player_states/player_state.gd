class_name PlayerState
extends RefCounted

const VIEW_MIN_ANGLE = deg_to_rad(-85.0)
const VIEW_MAX_ANGLE = deg_to_rad(85.0)

const WALK_SPEED = 2.0
const SPRINT_SPEED = 5.0
const CROUCH_SPEED = 1.0
const JUMP_VELOCITY = 4.5
const GRAVITY = 9.8

const ACCELERATION = 5.0
const DEACCELERATION = 10.0
const JUMP_ACCELERATION = 1.0

## Base class for every PlayerState.

## Input provided by player for current frame
var movement_input := Vector3.ZERO
## Input provided by player for current frame
var rotation_input := Vector3.ZERO

# State's state machine
var _state_machine = null
var _player_controller: CharacterBody3D
var _player_head: Node3D


func _init(
	state_machine,
	player_controller: CharacterBody3D,
	player_head: Node3D
):
	_state_machine = state_machine
	_player_controller = player_controller
	_player_head = player_head


## Executed when state becomes a current one.
func _on_enter(_args: Array) -> void:
	pass


## Executed every process frame.
func _process(_delta: float) -> void:
	pass


## Executed every physics process frame.
func _physics_process(_delta: float) -> void:
	pass


## Executed when state stops being the current one.
func _on_exit() -> void:
	pass


## Hint to PlayerStateMachine, if it should pass player's input to movement and rotation functions
func _can_movement_recieve_input() -> bool:
	return true


func _rotate(delta: float) -> void:
	_player_controller.rotate_object_local(Vector3.UP, rotation_input.y * delta)
	_player_head.rotation.x = clamp(
		_player_head.rotation.x + rotation_input.x * delta,
		VIEW_MIN_ANGLE, VIEW_MAX_ANGLE
	)


func _move(delta: float) -> void:
	var direction := Vector3(movement_input.x, 0, movement_input.z)
	if direction.length_squared() > 1:
		direction = direction.normalized()
	var vel_y := _player_controller.velocity.y
	
	var speed := WALK_SPEED
	var accel := JUMP_ACCELERATION
	
	if _player_controller.is_on_floor():
		if Input.is_action_pressed("crouch") or not _player_controller.can_stand():
			if not _player_controller.is_crouching():
				_player_controller.crouch()
			speed = CROUCH_SPEED
			accel = ACCELERATION
			vel_y = 0
		else:
			if _player_controller.is_crouching():
				_player_controller.stand()
			if Input.is_action_just_pressed("jump"):
				vel_y = JUMP_VELOCITY
			else:
				vel_y = 0
				accel = ACCELERATION if direction else DEACCELERATION
			
			if Input.is_action_pressed("sprint"):
				speed = SPRINT_SPEED
	else:
		if _player_controller.is_crouching():
			_player_controller.stand()
		vel_y -= GRAVITY * delta
	
	var target_vel := _player_controller.transform.basis * direction * speed
	_player_controller.velocity.x = lerpf(
		_player_controller.velocity.x, target_vel.x, accel * delta
	)
	_player_controller.velocity.z = lerpf(
		_player_controller.velocity.z, target_vel.z, accel * delta
	)
	_player_controller.velocity.y = vel_y
	
	_player_controller.move_and_slide()


## Returns state name.
func get_state_type() -> String:
	return get_script().resource_path.get_file()
