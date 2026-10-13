class_name PlayerController
extends CharacterBody3D

const HEAD_STANDING_POS = Vector3(0, 1.55, 0)
const HEAD_CROUCHING_POS = Vector3(0, 0.65, 0)
const HEAD_CHANGING_SPEED = 5.0

@onready var _head := $Head as Node3D
#@onready var _camera := $Head/PlayerCamera as Camera3D
@onready var _standing_col := $StandCol as CollisionShape3D
@onready var _crouching_col := $CrouchCol as CollisionShape3D
@onready var _crouch_ray_cast := $CrouchRayCast as RayCast3D
@onready var _interaction_ray_cast := $Head/PlayerCamera/InteractionRayCast as RayCast3D

#@export_node_path(Node3D) var _test_node_path = NodePath("")
#@onready var _test_node: Node3D = get_node_or_null(_test_node_path)
var _mouse_sensitivity = Vector2(0.5, 0.5)
var _view_change: Vector2 = Vector2()

var _state_machine: PlayerStateMachine

func _ready() -> void:
	Input.mouse_mode = Input.MOUSE_MODE_CAPTURED
	_state_machine = PlayerStateMachine.new(
		self, _head, _interaction_ray_cast
	)


func _input(event: InputEvent) -> void:
	if event is InputEventMouseMotion:
		_view_change = event.relative * _mouse_sensitivity * 0.5
	elif event is InputEventKey:
		if event.pressed && event.keycode == KEY_M:
			if Input.mouse_mode == Input.MOUSE_MODE_CAPTURED:
				Input.mouse_mode = Input.MOUSE_MODE_VISIBLE
			else:
				Input.mouse_mode = Input.MOUSE_MODE_CAPTURED


func _get_player_movement_input() -> Vector3:
	var result = Vector3(
		Input.get_action_strength("move_right") - Input.get_action_strength("move_left"),
		Input.get_action_strength("move_up") - Input.get_action_strength("move_down"),
		Input.get_action_strength("move_backward") - Input.get_action_strength("move_forward")
	)
	if result.length_squared() > 1:
		result = result.normalized()
	return result


func _get_player_rotation_input() -> Vector3:
	var result = Vector3(
		-_view_change.y,
		-_view_change.x,
		0
	)
	
	_view_change = Vector2.ZERO
	return result


func _process(delta: float) -> void:
	_state_machine.rotation_input = _get_player_rotation_input() if Input.mouse_mode == Input.MOUSE_MODE_CAPTURED else Vector3.ZERO
	_state_machine.movement_input = _get_player_movement_input()
	
	_state_machine.process(delta)


func _physics_process(delta: float) -> void:
	_state_machine.physics_process(delta)
	
	if is_crouching():
		_head.position = _head.position.move_toward(
			HEAD_CROUCHING_POS, delta * HEAD_CHANGING_SPEED
		)
	else:
		_head.position = _head.position.move_toward(
			HEAD_STANDING_POS, delta * HEAD_CHANGING_SPEED
		)


func _apply_impulses_to_rigid_bodies() -> void:
	for i in range(get_slide_collision_count()):
		var collision = get_slide_collision(i)
		for j in range(collision.get_collision_count()):
			var collider = collision.get_collider(j)
			if collider is RigidBody3D:
				collider.apply_central_impulse(-collision.get_normal(j) * velocity.length())


func can_stand() -> bool:
	return not _crouch_ray_cast.is_colliding()


func stand() -> void:
	_standing_col.disabled = false
	_crouching_col.disabled = true
#	_head.position = HEAD_STANDING_POS


func crouch() -> void:
	_standing_col.disabled = true
	_crouching_col.disabled = false
#	_head.position = HEAD_CROUCHING_POS


func is_crouching() -> bool:
	return _standing_col.disabled
