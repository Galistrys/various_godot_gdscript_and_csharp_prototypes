class_name FlyCamera
extends CharacterBody3D


const MIN_ANGLE = deg_to_rad(-90)
const MAX_ANGLE = deg_to_rad(90)

@onready var _camera := $Camera3D as Camera3D

var _view_change := Vector2.ZERO
var _camera_sensitivity := Vector2(0.1, 0.1)

var _speed = 20.0
var _sprint_speed = 40.0


func _ready() -> void:
	Input.mouse_mode = Input.MOUSE_MODE_CAPTURED


func _input(event: InputEvent) -> void:
	if event is InputEventMouseMotion:
		_view_change = event.relative
	
	elif event is InputEventMouseButton:
		if event.button_index == MOUSE_BUTTON_WHEEL_UP and event.pressed:
			_speed += 1.0
			_sprint_speed = _speed * 2.0
			print("Camera speed: ", _speed, " - ", _sprint_speed)
		elif event.button_index == MOUSE_BUTTON_WHEEL_DOWN and event.pressed:
			_speed -= 1.0
			_sprint_speed = _speed * 2.0
			print("Camera speed: ", _speed, " - ", _sprint_speed)
	
	elif event is InputEventKey:
		if event.pressed:
			if event.keycode == KEY_ESCAPE:
				if Input.mouse_mode == Input.MOUSE_MODE_CAPTURED:
					Input.mouse_mode = Input.MOUSE_MODE_VISIBLE
				else:
					Input.mouse_mode = Input.MOUSE_MODE_CAPTURED
			elif event.keycode == KEY_I:
				get_tree().root.debug_draw = Viewport.DEBUG_DRAW_DISABLED
			elif event.keycode == KEY_O:
				get_tree().root.debug_draw = Viewport.DEBUG_DRAW_WIREFRAME
			elif event.keycode == KEY_P:
				get_tree().root.debug_draw = Viewport.DEBUG_DRAW_OVERDRAW
		


func _process(delta: float) -> void:
	if _view_change.length_squared() > 0:
		rotate_y(-_view_change.x * _camera_sensitivity.x * delta)
		_camera.rotation.x = clamp(
			_camera.rotation.x - _view_change.y * _camera_sensitivity.y * delta,
			MIN_ANGLE,
			MAX_ANGLE
		)
	
	_view_change = Vector2.ZERO


func _physics_process(_delta: float) -> void:
	var input_dir = Input.get_vector(
		"debug_move_left", "debug_move_right", "debug_move_forward", "debug_move_backward"
	)
	
	var direction = (
		_camera.global_transform.basis * Vector3(
			input_dir.x,
			0,
			input_dir.y
		)
	).normalized()
	
	var up_down := Input.get_action_strength("debug_move_up") - Input.get_action_strength("debug_move_down")
	if up_down != 0:
		direction.y = up_down
	
	if direction:
		velocity = direction * (_sprint_speed if Input.is_action_pressed("debug_move_sprint") else _speed)
	else:
		velocity = Vector3.ZERO
	
	move_and_slide()
