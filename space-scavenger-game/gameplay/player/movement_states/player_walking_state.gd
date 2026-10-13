class_name PlayerWalkingState
extends PlayerMovementState


## State for [PlayerMovementStateMachine] when [Player] is in
## an environment with gravity


## State's ID
const ID = &"PlayerWalkingState"


@warning_ignore("unused_parameter")
func on_enter(data: Array) -> void:
	set_should_rotate(true)
	set_should_move(true)
	
	_player.motion_mode = CharacterBody3D.MOTION_MODE_GROUNDED
	
	_update_orientation()
	
	_player.floor_ray_cast.force_update_transform()
	_player.floor_ray_cast.force_raycast_update()
	
	if _player.floor_ray_cast.is_colliding():
		_player.global_position = _player.floor_ray_cast.get_collision_point() \
			+ _player.transform.basis.y * Player.CAMERA_DIF
	
	_player.stand_ray_cast.force_update_transform()
	_player.stand_ray_cast.force_raycast_update()
	
	if _player.stand_ray_cast.is_colliding():
		_player.enable_crouching_posture()
	else:
		_player.enable_standing_posture()


@warning_ignore("unused_parameter")
func _rotate(delta: float) -> void:
	var input := InputService.get_rotation_vector()
	var rot_x := clampf(
			_player.head.rotation.x + input.y,
			Player.VIEW_MIN_ANGLE, Player.VIEW_MAX_ANGLE)
	_player.head.rotation.x = rot_x
	_player.rotate_object_local(Vector3.UP, input.x)


func _move(delta: float) -> void:
	var input := InputService.get_movement_vector()
	var vel := Vector3.ZERO
	var on_floor := _player.is_on_floor()
	
	var direction := Vector3(input.x, 0.0, input.z)
	var accel := Player.MID_AIR_ACCELERATION
	if on_floor:
		if direction.length_squared() > 1.0:
			accel = Player.ACCELERATION
		else:
			accel = Player.DEACCELERATION
	
	# CHECK IF CROUCHING
	if (InputService.crouching or _player.stand_ray_cast.is_colliding()) \
			and on_floor:
		_player.enable_crouching_posture()
		vel = direction * Player.CROUCH_SPEED
	else:
		_player.enable_standing_posture()
		vel = direction * (
				Player.RUN_SPEED if InputService.sprinting else Player.WALK_SPEED)
	
	
	var target_vel := _player.velocity * _player.global_basis
	var vel_y := target_vel.y
	
	target_vel.y = 0.0
	target_vel = target_vel.lerp(vel, accel * delta)
	if not on_floor:
		vel_y -= _player.get_gravity().length() * delta
	elif InputService.is_jumping():
		vel_y = Player.JUMP_POWER
	else:
		vel_y = 0.0
	
	target_vel.y = vel_y
	_player.velocity = _player.global_basis * target_vel
	
	_player.move_and_slide()


func _update_orientation() -> void:
	_player.up_direction = Vector3.UP
	var direction := _player.global_basis.z
	var left_axis := _player.up_direction.cross(direction)
	var rotation_basis := Basis(left_axis, _player.up_direction, direction).orthonormalized()
	_player.global_basis = rotation_basis
