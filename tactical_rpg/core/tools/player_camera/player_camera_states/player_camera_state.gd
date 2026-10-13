@abstract
class_name PlayerCameraState
extends State


## Base class for all [PlayerCamera] states


## Camera's horizontal movement speed
const SPEED = 12.0
## Camera's movement acceleration
const ACCELERATION = 5.0
## Camera's movement deacceleration
const DEACCELERATION = 10.0

## Camera's distance change speed
const DISTANCE_SPEED = 4.0
## Camera's distance change smoothing
const DISTANCE_SMOOTHING = 8.0

const MAX_DISTANCE_FROM_LAST_TARGET = 32.0
const MAX_DISTANCE_SQUARED_FROM_LAST_TARGET = pow(MAX_DISTANCE_FROM_LAST_TARGET, 2.0)

## Camera polar position vector index -> Distance
const CAM_POS_DIST_IDX = 0
## Camera polar position vector index -> Horizontal Angle
const CAM_HOR_ANGLE_IDX = 1
## Camera polar position vector index -> Vertical Angle
const CAM_VER_ANGLE_IDX = 2


static var _get_position: Callable
static var _set_position: Callable
static var _get_camera_position: Callable
static var _set_camera_position: Callable


var _target_distance: float


static func initialize_threaded(
	p_get_position: Callable,
	p_set_position: Callable,
	p_get_camera_position: Callable,
	p_set_camera_position: Callable
) -> void:
	_get_position = p_get_position
	_set_position = p_set_position
	_get_camera_position = p_get_camera_position
	_set_camera_position = p_set_camera_position


static func clear() -> void:
	_get_position = Callable()
	_set_position = Callable()
	_get_camera_position = Callable()
	_set_camera_position = Callable()


func _init() -> void:
	_target_distance = _get_camera_position.call()[CAM_POS_DIST_IDX] as float


func _process_default_rotation(delta: float) -> void:
	var camera_position := _get_camera_position.call() as Vector3
	var rotation_input := InputManager.get_rotation()
	
	var vertical_input := InputManager.get_vertical()
	
	camera_position[CAM_VER_ANGLE_IDX] -= rotation_input.y
	camera_position[CAM_HOR_ANGLE_IDX] += rotation_input.x
	
	_target_distance = clampf(
			_target_distance + vertical_input * DISTANCE_SPEED * delta,
			0.0, 1.0)
	camera_position[CAM_POS_DIST_IDX] = lerpf(
			camera_position[CAM_POS_DIST_IDX], _target_distance, delta * DISTANCE_SMOOTHING)
	
	_set_camera_position.call(camera_position)
