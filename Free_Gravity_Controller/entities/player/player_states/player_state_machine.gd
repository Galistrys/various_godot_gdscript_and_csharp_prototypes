class_name PlayerStateMachine
extends RefCounted


var movement_input := Vector3.ZERO
var rotation_input := Vector3.ZERO


var _reference_objects_list: Array[GravityReferenceObject] = []

var _behavior_states_dict := {}
var _current_behavior_state: BehaviorPlayerState

var _movement_states_dict := {}
var _current_movement_state: MovementPlayerState


func _init(
	character: CharacterBody3D,
	head: Node3D,
	_player_camera: Camera3D,
	floor_ray: RayCast3D,
	standing_col: CollisionShape3D,
	fly_col: CollisionShape3D,
	interaction_ray_cast: RayCast3D
):
	_behavior_states_dict[Constants.BEHAVIOR_STATE_IDLE] = IdlePlayerState.new(
		self,
		interaction_ray_cast
	)
	
	_current_behavior_state = _behavior_states_dict[Constants.BEHAVIOR_STATE_IDLE]
	
	_movement_states_dict[Constants.MOVEMENT_PLAYER_STATE_WALKING] = WalkingPlayerState.new(
		character,
		head,
		floor_ray,
		standing_col,
		fly_col
	)
	_movement_states_dict[Constants.MOVEMENT_PLAYER_STATE_FLYING] = FlyingPlayerState.new(
		character,
		head,
		standing_col,
		fly_col
	)
	
	_current_movement_state = _movement_states_dict[Constants.MOVEMENT_PLAYER_STATE_FLYING]
	_current_movement_state.on_enter([])


func process(delta: float) -> void:
	_current_behavior_state.process(delta)


func physics_process(delta: float) -> void:
	_current_behavior_state.physics_process(delta)
	if _current_behavior_state.can_movement_recieve_input():
		_current_movement_state.rotate(delta, rotation_input)
		_current_movement_state.move(delta, movement_input)
	else:
		_current_movement_state.rotate(delta, Vector3.ZERO)
		_current_movement_state.move(delta, Vector3.ZERO)


func transition_behavior_state_to(state_name: String, args: Array) -> void:
	if _behavior_states_dict.has(state_name) == null:
		printerr("State %s not found in behavior states dict." % state_name)
		return
	
	print_debug(
		"Transition from %s to %s" % [
			_current_behavior_state.get_state_type(),
			state_name
		]
	)
	
	if _current_behavior_state != null:
		_current_behavior_state.on_exit()
	
	_current_behavior_state = _behavior_states_dict[state_name] as BehaviorPlayerState
	_current_behavior_state.on_enter(args)


func transition_movement_state_to(state_name: String, args: Array) -> void:
	if _movement_states_dict.has(state_name) == null:
		printerr("State %s not found in movement states dict." % state_name)
		return
	
	print_debug(
		"Transition from %s to %s" % [
			_current_movement_state.get_state_type(),
			state_name
		]
	)
	
	if _current_movement_state != null:
		_current_movement_state.on_exit()
	
	_current_movement_state = _movement_states_dict[state_name] as MovementPlayerState
	_current_movement_state.on_enter(args)


func reset_movement_state() -> void:
	if _reference_objects_list.size() == 0:
		transition_movement_state_to(
			Constants.MOVEMENT_PLAYER_STATE_FLYING,
			[]
		)
	else:
		transition_movement_state_to(
			Constants.MOVEMENT_PLAYER_STATE_WALKING,
			[_reference_objects_list[0]]
		)


func add_gravity_reference_object(
	gravity_reference_object: GravityReferenceObject
) -> void:
	var pos = _reference_objects_list.find(gravity_reference_object)
	if pos != -1:
		printerr("Object ", gravity_reference_object.name, " already in array.")
		_reference_objects_list.remove_at(pos)
	_reference_objects_list.push_front(gravity_reference_object)
	
	if _current_movement_state is WalkingPlayerState:
		_current_movement_state.set_reference_object(gravity_reference_object)
	elif _current_movement_state is FlyingPlayerState:
		transition_movement_state_to(
			Constants.MOVEMENT_PLAYER_STATE_WALKING,
			[gravity_reference_object]
		)


func remove_gravity_reference_object(
	gravity_reference_object: GravityReferenceObject
) -> void:
	var pos = _reference_objects_list.find(gravity_reference_object)
	if pos == -1:
		printerr("Object ", gravity_reference_object.name, " not in array.")
	else:
		_reference_objects_list.erase(gravity_reference_object)
	
	if _current_movement_state is WalkingPlayerState:
		if _reference_objects_list.size() == 0:
			transition_movement_state_to(
				Constants.MOVEMENT_PLAYER_STATE_FLYING, []
			)
		else:
			_current_behavior_state.set_reference_object(
				_reference_objects_list[0]
			)

