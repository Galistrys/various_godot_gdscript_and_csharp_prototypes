class_name FlyCamera
extends CharacterBody3D

const SPEED = 2.0
const SPRINT_SPEED = 20.0
const MIN_ANGLE = deg_to_rad(-90)
const MAX_ANGLE = deg_to_rad(90)

@onready var _camera := $Camera3D as Camera3D

var _view_change := Vector2.ZERO
var _camera_sensitivity := Vector2(0.1, 0.1)


func _ready() -> void:
	Input.mouse_mode = Input.MOUSE_MODE_CAPTURED


func _input(event: InputEvent) -> void:
	if event is InputEventMouseMotion and Input.mouse_mode == Input.MOUSE_MODE_CAPTURED:
		_view_change = event.relative
	
	if event is InputEventKey:
		if event.pressed and event.keycode == KEY_ESCAPE:
			if Input.mouse_mode == Input.MOUSE_MODE_CAPTURED:
				Input.mouse_mode = Input.MOUSE_MODE_VISIBLE
			else:
				Input.mouse_mode = Input.MOUSE_MODE_CAPTURED
		if event.pressed and event.keycode == KEY_I:
			if get_tree().root.debug_draw == Viewport.DEBUG_DRAW_DISABLED:
				get_tree().root.debug_draw = Viewport.DEBUG_DRAW_WIREFRAME
			else:
				get_tree().root.debug_draw = Viewport.DEBUG_DRAW_DISABLED
		


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
		"move_left", "move_right", "move_forward", "move_backward"
	)
	
	var direction = (
		_camera.global_transform.basis * Vector3(
			input_dir.x,
			0,
			input_dir.y
		)
	).normalized()
	
	var up_down := Input.get_action_strength("move_up") - Input.get_action_strength("move_down")
	if up_down != 0:
		direction.y = up_down
	
	if direction:
		velocity = direction * (SPRINT_SPEED if Input.is_action_pressed("sprint") else SPEED)
	else:
		velocity = velocity.move_toward(Vector3.ZERO, SPEED)

	move_and_slide()
