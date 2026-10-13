class_name PlayerController
extends CharacterBody3D

@onready var _head := $Head as Node3D
@onready var _camera := $Head/PlayerCamera as Camera3D
@onready var _standing_col := $StandingCol as CollisionShape3D
@onready var _fly_col := $FlyCol as CollisionShape3D
@onready var _floor_ray := $FloorRayCast as RayCast3D
@onready var _interaction_ray_cast := $Head/PlayerCamera/InteractionRayCast as RayCast3D

#@export_node_path(Node3D) var _test_node_path = NodePath("")
#@onready var _test_node: Node3D = get_node_or_null(_test_node_path)
var _mouse_sensitivity = Vector2(0.5, 0.5)
var _view_change: Vector2 = Vector2()

var _state_machine: PlayerStateMachine

func _ready() -> void:
	Input.mouse_mode = Input.MOUSE_MODE_CAPTURED
	_state_machine = PlayerStateMachine.new(
		self, _head, _camera, _floor_ray, _standing_col, _fly_col, _interaction_ray_cast
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
#		elif event.pressed && event.keycode == KEY_P:
#			if _current_movement_state is EvaPlayerState:
#				_current_movement_state = WalkPlayerState.new(_test_node)
#			else:
#				_current_movement_state = EvaPlayerState.new()
#			_current_movement_state.on_enter(self, _head, _floor_ray, _standing_col, _fly_col)

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
		Input.get_action_strength("rotate_left") - Input.get_action_strength("rotate_right")
	)
	
	_view_change = Vector2.ZERO
	return result


func _process(delta: float) -> void:
	_state_machine.rotation_input = _get_player_rotation_input() if Input.mouse_mode == Input.MOUSE_MODE_CAPTURED else Vector3.ZERO
	_state_machine.movement_input = _get_player_movement_input()
	
	_state_machine.process(delta)


func _physics_process(delta: float) -> void:
	_state_machine.physics_process(delta)


func add_gravity_reference_object(gro: GravityReferenceObject) -> void:
	_state_machine.add_gravity_reference_object(gro)


func remove_gravity_reference_object(gro: GravityReferenceObject) -> void:
	_state_machine.remove_gravity_reference_object(gro)


func _apply_impulses_to_rigid_bodies() -> void:
	for i in range(get_slide_collision_count()):
		var collision = get_slide_collision(i)
		for j in range(collision.get_collision_count()):
			var collider = collision.get_collider(j)
			if collider is RigidBody3D:
				collider.apply_central_impulse(-collision.get_normal(j) * velocity.length())
