class_name FollowTargetCameraState
extends PlayerCameraState


## [PlayerCamera]'s behavior state for following a target


var _target: Node3D
var _distance: float


## Executed when [State] becomes a current state for [StateMachine]
func on_enter(args: Array) -> void:
	_target = args[0] as Node3D
	
	var current_position: Vector3 = _get_position.call()
	_distance = current_position.distance_to(_target.global_position)


## Executed when [State] stopps being a current state for [StateMachine]
func on_exit() -> void:
	_target = null


## Update frame
func process(delta: float) -> void:
	_process_default_rotation(delta)
	_process_movement(delta)


## Fixed frame
func physics_process(_delta: float) -> void:
	pass


func _process_movement(delta: float) -> void:
	var target_position := _target.get_global_transform_interpolated().origin
	if _distance > 0.001:
		_distance = lerpf(_distance, 0.0, delta * SPEED)
		var current_position := _get_position.call() as Vector3
		var dir := target_position.direction_to(current_position)
		_set_position.call(target_position + dir * _distance)
	else:
		_set_position.call(target_position)
