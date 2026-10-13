class_name PlayerCamera
extends Node3D


const CAMERA_SPEED = 2.0
const CAMERA_SPRINT_SPEED = 10.0
const CAMERA_VERT_TIME = 0.15
const MOUSE_MOVE_SCREEN_MARGIN = 30.0

const CAMERA_MAX_HEIGHT := 8.0
const CAMERA_MIN_HEIGHT := 2.0
const CAMERA_DISTANCE := 1.0


@onready var _camera := $Camera3D as Camera3D
var _target_position := Vector3.ZERO


func _ready() -> void:
	Input.mouse_mode = Input.MOUSE_MODE_CONFINED
	
	_target_position = Vector3(0, CAMERA_MAX_HEIGHT, CAMERA_DISTANCE)
	_camera.translate(_target_position)
	_camera.transform = _camera.transform.looking_at(Vector3.ZERO)


func _input(event: InputEvent) -> void:
	if event is InputEventKey:
		if event.pressed and event.keycode == KEY_ESCAPE:
			if Input.mouse_mode == Input.MOUSE_MODE_CONFINED:
				Input.mouse_mode = Input.MOUSE_MODE_VISIBLE
			else:
				Input.mouse_mode = Input.MOUSE_MODE_CONFINED


func _get_key_input_dir() -> Vector2:
	return Input.get_vector(
		"camera_move_left", "camera_move_right",
		"camera_move_up", "camera_move_down"
	)


func _get_mouse_input_dir() -> Vector2:
	var result := Vector2.ZERO
	if Input.mouse_mode != Input.MOUSE_MODE_CONFINED:
		return result
	
	var mouse_pos := get_window().get_mouse_position()
	
	if mouse_pos.x < MOUSE_MOVE_SCREEN_MARGIN:
		result.x -= 1.0 - (mouse_pos.x / MOUSE_MOVE_SCREEN_MARGIN)
	elif mouse_pos.x > get_window().size.x - MOUSE_MOVE_SCREEN_MARGIN:
		result.x += 1.0 - ((get_window().size.x - mouse_pos.x) / MOUSE_MOVE_SCREEN_MARGIN)
	if mouse_pos.y < MOUSE_MOVE_SCREEN_MARGIN:
		result.y -= 1.0 - (mouse_pos.y / MOUSE_MOVE_SCREEN_MARGIN)
	elif mouse_pos.y > get_window().size.y - MOUSE_MOVE_SCREEN_MARGIN:
		result.y += 1.0 - ((get_window().size.y - mouse_pos.y) / MOUSE_MOVE_SCREEN_MARGIN)
	
	return result.clamp(-Vector2.ONE, Vector2.ONE)


func _camera_rise() -> void:
	_target_position.y = clampf(
		_camera.position.y + 1,
		CAMERA_MIN_HEIGHT, CAMERA_MAX_HEIGHT
	)

func _camera_lower() -> void:
	_target_position.y = clampf(
		_camera.position.y - 1,
		CAMERA_MIN_HEIGHT, CAMERA_MAX_HEIGHT
	)

func _process(delta: float) -> void:
	if Input.is_action_just_released("camera_rise"):
		_camera_rise()
	elif Input.is_action_just_released("camera_lower"):
		_camera_lower()
	
	if _camera.position != _target_position:
		_camera.position = _camera.position.move_toward(
			_target_position, delta * 7
		)
		_camera.transform = _camera.transform.looking_at(Vector3.ZERO)


func _physics_process(delta: float) -> void:
	var speed := CAMERA_SPRINT_SPEED if Input.is_action_pressed("sprint") else CAMERA_SPEED
	
	var input_dir := _get_mouse_input_dir()
	if input_dir.x == 0 and input_dir.y == 0:
		input_dir = _get_key_input_dir()
	else:
#		# Changing mouse cursor
#
#		if input_dir.x == 0:
#			if input_dir.y > 0:
#				pass
#			else:
#				pass
#		else:
#			if input_dir.x > 0:
#				pass
#			else:
#				pass
		
		speed = CAMERA_SPRINT_SPEED
	
	var direction := (
		transform.basis * Vector3(input_dir.x, 0, input_dir.y)
	)
	if direction.length_squared() > 1.0:
		direction = direction.normalized()
	
	translate(direction * delta * speed)
