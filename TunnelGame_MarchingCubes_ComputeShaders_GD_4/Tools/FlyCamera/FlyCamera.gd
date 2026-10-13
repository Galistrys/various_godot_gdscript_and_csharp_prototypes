extends Node3D

const FLY_SPEED = 8
const FAST_FLY_SPEED = 32

const MAX_ANGLE = PI / 2.0
const MIN_ANGLE = -PI / 2.0
const MOUSE_SENSITIVITY = 0.1

@onready var _camera = $Camera3D

func _ready():
	Input.mouse_mode = Input.MOUSE_MODE_CAPTURED

func _input(event):
	if event is InputEventMouseMotion and Input.mouse_mode == Input.MOUSE_MODE_CAPTURED:
		rotate_y(deg_to_rad(-event.relative.x) * MOUSE_SENSITIVITY)
		_camera.rotate_x(deg_to_rad(-event.relative.y) * MOUSE_SENSITIVITY)
		
		_camera.rotation.x = clamp(_camera.rotation.x, MIN_ANGLE, MAX_ANGLE)
	elif event is InputEventKey:
		if event.pressed:
			if event.keycode == KEY_ESCAPE:
				if Input.mouse_mode == Input.MOUSE_MODE_CAPTURED:
					Input.mouse_mode = Input.MOUSE_MODE_VISIBLE
				else:
					Input.mouse_mode = Input.MOUSE_MODE_CAPTURED

func _physics_process(delta):
	print(position)
	var input = _get_input()
	var direction = _camera.global_transform.basis * input
	var speed = FAST_FLY_SPEED if Input.is_action_pressed("sprint") else FLY_SPEED
	position += direction * delta * speed

func _get_input() -> Vector3:
	var input := Vector3(
		Input.get_action_strength("move_right") - Input.get_action_strength("move_left"),
		Input.get_action_strength("move_up") - Input.get_action_strength("move_down"),
		Input.get_action_strength("move_backward") - Input.get_action_strength("move_forward")
	)
	
	if input.length_squared() > 1:
		input = input.normalized()
	return input
