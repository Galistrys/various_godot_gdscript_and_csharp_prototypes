class_name StateMachine
extends Node


## It is an abstract machine that can be in exactly one
## of a finite number of states at any given time


@export var _id: StringName


var _current_state: State
var _states: Dictionary[StringName, State] = {}


func _process(delta: float) -> void:
	_current_state.update(delta)


func _physics_process(delta: float) -> void:
	_current_state.physics_update(delta)


## Register a [State] in this [StateMachine]
func register_state(state_name: StringName, state: State) -> bool:
	if _states.has(state_name):
		push_error(
				"[StateMachine] <%s>: Already has state with that name -> %s" \
				% [_id, state_name])
		return false
	
	_states[state_name] = state
	var err := state.request_transition.connect(transition)
	if err != OK:
		print("[StateMachine] <%s>: State failed to connect a signal, reason: %s -> %s" \
				% [_id, error_string(err), state_name])
	
	print("[StateMachine] <%s>: State registered -> %s" % [_id, state_name])
	return true


## Transition to a given [State]
func transition(state_name: StringName, data: Array) -> bool:
	if not _states.has(state_name):
		push_error(
				"[StateMachine] <%s>: Does not contain state -> %s" \
				% [_id, state_name])
		return false
	
	print("[StateMachine] <%s>: Transition -> %s" % [_id, state_name])
	if _current_state:
		_current_state.on_exit()
	_current_state = _states[state_name]
	_current_state.on_enter(data)
	
	return true
