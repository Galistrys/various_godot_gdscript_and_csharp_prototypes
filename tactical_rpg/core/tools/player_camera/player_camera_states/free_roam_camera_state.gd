class_name FreeRoamCameraState
extends PlayerCameraState


## [PlayerCamera]'s behavior state for free roam


var _last_target: Node3D

var _current_speed := Vector3()


## Executed when [State] becomes a current state for [StateMachine]
func on_enter(_args: Array) -> void:
	_last_target = _args[0] as Node3D


## Executed when [State] stopps being a current state for [StateMachine]
func on_exit() -> void:
	_last_target = null


## Update frame
func process(delta: float) -> void:
	_process_default_rotation(delta)
	_process_movement(delta)


## Fixed frame
func physics_process(_delta: float) -> void:
	pass


func _process_movement(delta: float) -> void:
	var current_position: Vector3 = _get_position.call()
	var current_rotation: float = _get_camera_position.call()[CAM_HOR_ANGLE_IDX]
	var movement_input := InputManager.get_movement()
	movement_input.y = 0.0
	movement_input = movement_input.rotated(Vector3.UP, current_rotation)
	
	var target_speed := movement_input * SPEED
	var accel := 0.0
	if target_speed.length_squared() > 0:
		accel = ACCELERATION
	else:
		accel = DEACCELERATION
	_current_speed = _current_speed.lerp(target_speed, delta * accel)
	
	var target_position := current_position + _current_speed * delta
	
	var last_target_pos := _last_target.get_global_transform_interpolated().origin
	if target_position.distance_squared_to(last_target_pos) \
			> MAX_DISTANCE_SQUARED_FROM_LAST_TARGET:
		var dir := last_target_pos.direction_to(target_position)
		target_position = last_target_pos + dir * MAX_DISTANCE_FROM_LAST_TARGET
	
	_set_position.call(target_position)
