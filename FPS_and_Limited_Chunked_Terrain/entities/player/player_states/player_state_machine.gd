class_name PlayerStateMachine
extends RefCounted

var movement_input := Vector3.ZERO
var rotation_input := Vector3.ZERO

var _states_dict := {}
var _current_state: PlayerState

func _init(
	player_controller: CharacterBody3D,
	player_head: Node3D,
	interaction_ray_cast: RayCast3D
):
	_states_dict[Constants.PLAYER_STATE_IDLE] = IdlePlayerState.new(
		self, player_controller, player_head, interaction_ray_cast
	)
	transition_state_to(Constants.PLAYER_STATE_IDLE, [])


func process(delta: float) -> void:
	_current_state.movement_input = movement_input
	_current_state.rotation_input = rotation_input
	_current_state._process(delta)


func physics_process(delta: float) -> void:
	_current_state._physics_process(delta)
	if _current_state._can_movement_recieve_input():
		_current_state._rotate(delta)
		_current_state._move(delta)
	else:
		_current_state.movement_input = Vector3.ZERO
		_current_state.rotation_input = Vector3.ZERO
		_current_state._rotate(delta)
		_current_state._move(delta)


func transition_state_to(state_name: String, args: Array) -> void:
	if _states_dict.has(state_name) == null:
		printerr("State %s not found in behavior states dict." % state_name)
		return
	
	
	if _current_state != null:
		print_debug(
			"Transition from %s to %s" % [
				_current_state.get_state_type(),
				state_name
			]
		)
		_current_state._on_exit()
	
	_current_state = _states_dict[state_name]
	_current_state.movement_input = movement_input
	_current_state.rotation_input = rotation_input
	_current_state._on_enter(args)
