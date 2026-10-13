extends CharacterBody3D

const MOUSE_CLAMP = 500.0
const VIEW_CHANGE_SMOOTHING = 0.4

const CAMERA_NORMAL_FOV = 70.0
const CAMERA_SPRINT_FOV = 80.0

const MAX_ANGLE = 85.0 * PI / 180.0
const MIN_ANGLE = -80.0 * PI / 180.0

const WALK_SPEED = 2.0
const SPRINT_SPEED = 6.0
const CROUCH_SPEED = 0.75
const ACCELERATION = 5.0
const DEACCELERATION = 10.0
const JUMP_STRENGTH = 5.0
const JUMP_ACCELERATION = 1.0

const HEIGHT_CHANGE_SPEED = 10.0
const HEAD_STAND_Y = 1.6
const HEAD_CROUCH_Y = 0.8

const FLOOR_MAX_ANGLE = 45.0

const GRAVITY = 9.8

var view_change: Vector2 = Vector2.ZERO
var current_vel: Vector3 = Vector3.ZERO

@onready var head_node: Node3D = $Head
@onready var camera: Camera3D = $Head/PlayerCamera

@onready var crouch_ray_cast: RayCast3D = $CrouchRayCast

@onready var stand_col_shape: CollisionShape3D = $StandingCol
@onready var crouch_col_shape: CollisionShape3D = $CrouchingCol

@export var mouse_sensitivity: Vector2 = Vector2(0.1, 0.1)

var _joypad_mode: bool = false

var _can_sprint: bool = true
var _crouching: bool = false
var _gravity: float = GRAVITY

func _ready() -> void:
	camera.fov = CAMERA_NORMAL_FOV
	
	Input.mouse_mode = Input.MOUSE_MODE_CAPTURED

func _input(event) -> void:
	if event is InputEventMouseMotion:
		_joypad_mode = false
		view_change += event.relative * mouse_sensitivity
	elif event is InputEventKey:
		_joypad_mode = false
		
		if event.pressed:
			if event.keycode == KEY_ESCAPE:
				if Input.mouse_mode == Input.MOUSE_MODE_CAPTURED:
					Input.mouse_mode = Input.MOUSE_MODE_VISIBLE
				else:
					Input.mouse_mode = Input.MOUSE_MODE_CAPTURED
	elif event is InputEventJoypadButton:
		_joypad_mode = true
	elif event is InputEventJoypadMotion:
		_joypad_mode = true
	
	# State: input

func _process(_delta) -> void:
	if _joypad_mode:
		_process_joypad_movement()
	
	# State: process
	
	view_change = Vector2.ZERO

func _physics_process(delta) -> void:
	# State: physics_process
	
	_camera_rotation(delta)
	_walk(delta)
	
	if _crouching:
		head_node.position.y = lerp(head_node.position.y, HEAD_CROUCH_Y, delta * HEIGHT_CHANGE_SPEED)
	else:
		head_node.position.y = lerp(head_node.position.y, HEAD_STAND_Y, delta * HEIGHT_CHANGE_SPEED)
	
	_apply_impulses_to_rigid_bodies()

func get_input_direction() -> Vector3:
	var res: Vector3 = transform.basis * Vector3(
		Input.get_action_strength("move_right") - Input.get_action_strength("move_left"),
		0,
		Input.get_action_strength("move_backward") - Input.get_action_strength("move_forward")
	)
	
	if res.length_squared() > 1:
		res = res.normalized()
	return res

func enable_stand_col_shape() -> void:
	stand_col_shape.disabled = false
	crouch_col_shape.disabled = true

func enable_crouch_col_shape() -> void:
	stand_col_shape.disabled = true
	crouch_col_shape.disabled = false

func _camera_rotation(delta: float) -> void:
	if Input.mouse_mode != Input.MOUSE_MODE_CAPTURED:
		return
	
	if view_change.length_squared() > 0:
		rotate_y(-view_change.x * delta)
		head_node.rotate_x(-view_change.y * delta)
		head_node.rotation.x = clamp(head_node.rotation.x, MIN_ANGLE, MAX_ANGLE)

func _walk(delta: float) -> void:
	var target_dir: Vector3 = get_input_direction()
	_can_sprint = not (Input.is_action_pressed("move_backward") or _crouching)
	
	var speed: float = WALK_SPEED
	var accel: float = JUMP_ACCELERATION
	
	if is_on_floor():
		if Input.is_action_just_pressed("jump"):
			_gravity = -JUMP_STRENGTH
			_crouching = false
		else:
			_gravity = 0
		
		if Input.is_action_pressed("crouch") or crouch_ray_cast.get_collider() != null:
			speed = CROUCH_SPEED
			_crouching = true
			enable_crouch_col_shape()
		elif not _crouching and Input.is_action_pressed("sprint") and _can_sprint:
			speed = SPRINT_SPEED
			enable_stand_col_shape()
		else:
			speed = WALK_SPEED
			_crouching = false
			enable_stand_col_shape()
		
		if target_dir.length_squared() > 0:
			accel = ACCELERATION
		else:
			accel = DEACCELERATION
	else:
		_gravity = GRAVITY * delta
	
	velocity.y -= _gravity
	
	current_vel = current_vel.lerp(target_dir * speed, delta * accel)
	velocity.x = current_vel.x
	velocity.z = current_vel.z
	move_and_slide()

func _process_joypad_movement() -> void:
	view_change = Vector2(
		Input.get_action_strength("look_right") - Input.get_action_strength("look_left"),
		Input.get_action_strength("look_down") - Input.get_action_strength("look_up")
	)
	
	if view_change.length_squared() > 1:
		view_change = view_change.normalized()
	view_change *= 50

func _apply_impulses_to_rigid_bodies() -> void:
	for i in range(get_slide_collision_count()):
		var collision: KinematicCollision3D = get_slide_collision(i)
		var collider = collision.get_collider()
		if collider is RigidBody3D:
			(collider as RigidBody3D).apply_central_impulse(
				-collision.get_normal() * velocity.length()
			)
