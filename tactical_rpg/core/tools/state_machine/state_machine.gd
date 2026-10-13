class_name StateMachine
extends RefCounted


## Generic StateMachine class.
## 
## It holds multiple [State] objects and allows for transitions between them.


var _id: StringName

var _states_dict: Dictionary[String, State] = {}
var _current_state: State = null


func _init(p_state_machine_id: String) -> void:
	_id = p_state_machine_id


## Registers a [param p_state] in this [StateMachine] with a given [param p_state_name].
## It connects [State] object's signal [signal State.request_state_transition] to
## [method StateMachine.transition_state]
func register_state(p_state_name: String, p_state: State) -> void:
	if _states_dict.has(p_state_name):
		push_error("[StateMachine] <%s>: Already contains state with that name -> %s" \
				% [_id, p_state_name])
	print("[StateMachine] <%s>: Register state -> %s" % [_id, p_state_name])
	_states_dict[p_state_name] = p_state
	p_state.request_state_transition.connect(transition_state)


## Transitions to [param p_state_name] with a given [param p_data].
func transition_state(p_state_name: String, p_data: Array) -> void:
	if _states_dict.has(p_state_name) == null:
		push_error("[StateMachine] <%s>: State not found in states dict -> %s" \
				% [_id, p_state_name])
		return
	
	if _current_state != null:
		print("[StateMachine] <%s>: Transition from %s to %s" \
				% [_id, _current_state.get_class_name(), p_state_name])
		_current_state.on_exit()
	else:
		print("[StateMachine] <%s>: Transition to %s" % [_id, p_state_name])
	
	_current_state = _states_dict[p_state_name]
	_current_state.on_enter(p_data)


## Process update frame current [State]
func process(delta: float) -> void:
	_current_state.process(delta)


## Process fixed frame for current [State]
func physics_process(delta: float) -> void:
	_current_state.physics_process(delta)


## Stops current state and cleans all states from this [StateMachine]
func stop_and_clean() -> void:
	if _current_state:
		_current_state.on_exit()
	_states_dict.clear()
