class_name PlayerCamera
extends Node3D


const HEIGHT_OFFSET = 0.9
const MAX_DISTANCE_FROM_LAST_TARGET = 32.0

const _MIN_DISTANCE = 4.0
const _MAX_DISTANCE = 16.0
const _MIN_ANGLE = 0.175
const _MAX_ANGLE = 1.483

const _EMPTY_STATE = &"empty_camera_state"
const _FOLLOW_TARGET_STATE = &"follow_target_camera_state"
const _FREE_ROAM_STATE = &"free_roam_camera_state"
const _TACTICAL_STATE = &"tactical_camera_state"


var _state_machine: StateMachine


var _target_position := Vector3()
# [distance, horizontal_angle, vertical_angle]
var _camera_position := Vector3(1.0, 0.0, PI / 4.0)


@onready var _camera_handle := $CameraHandle as Node3D
@onready var _camera := $CameraHandle/Camera3D as Camera3D


func _init() -> void:
	PlayerCameraState.initialize_threaded(
		get_current_position, set_target_position,
		get_current_camera_position, set_target_camera_position
	)


func _ready() -> void:
	set_process(false)
	set_physics_process(false)


func _process(delta: float) -> void:
	_state_machine.process(delta)
	
	_update_position()
	_update_camera_position()


func _exit_tree() -> void:
	_state_machine.stop_and_clean()
	PlayerCameraState.clear()


## Initializes [PlayerCamera]
func initialize_threaded() -> void:
	print("[PlayerCamera]: Initialization started")
	
	physics_interpolation_mode = Node.PHYSICS_INTERPOLATION_MODE_OFF
	
	_state_machine = StateMachine.new("PlayerCamera")
	
	var _empty_state := EmptyCameraState.new()
	var _follow_state := FollowTargetCameraState.new()
	var _free_roam_state := FreeRoamCameraState.new()
	
	_state_machine.register_state(_EMPTY_STATE, _empty_state)
	_state_machine.register_state(_FOLLOW_TARGET_STATE, _follow_state)
	_state_machine.register_state(_FREE_ROAM_STATE, _free_roam_state)
	
	_state_machine.transition_state(_EMPTY_STATE, [])
	
	set_process.call_deferred(false)
	set_physics_process.call_deferred(false)
	
	print("[PlayerCamera]: Initialization finished")


## Returns target position
func get_current_position() -> Vector3:
	return global_position


## Set target position
func set_target_position(p_vec: Vector3) -> void:
	_target_position = p_vec


## Returns target camera position in a [Vector3]:
## [code][distance, horizontal_angle, vertical_angle][/code]
func get_current_camera_position() -> Vector3:
	return _camera_position


## Set target camera position in a [Vector3]:
## [code][distance, horizontal_angle, vertical_angle][/code]
func set_target_camera_position(p_vec: Vector3) -> void:
	_camera_position = p_vec
	_camera_position[PlayerCameraState.CAM_VER_ANGLE_IDX] = clampf(
			_camera_position[PlayerCameraState.CAM_VER_ANGLE_IDX],
			_MIN_ANGLE,
			_MAX_ANGLE)


func _update_position() -> void:
	global_position = _target_position


func _update_camera_position() -> void:
	rotation.y = _camera_position[PlayerCameraState.CAM_HOR_ANGLE_IDX]
	_camera_handle.rotation.x = _camera_position[PlayerCameraState.CAM_VER_ANGLE_IDX]
	var dis := lerpf(
			_MIN_DISTANCE,
			_MAX_DISTANCE,
			_camera_position[PlayerCameraState.CAM_POS_DIST_IDX])
	var rot := _camera_handle.rotation.x
	_camera_handle.position = Vector3(
		0.0,
		HEIGHT_OFFSET + dis * sin(rot),
		dis * cos(rot)
	)
	
	_camera.look_at(global_position + Vector3(0.0, HEIGHT_OFFSET, 0.0))
