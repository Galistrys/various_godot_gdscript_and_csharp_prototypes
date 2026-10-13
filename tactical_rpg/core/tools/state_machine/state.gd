@abstract
class_name State
extends RefCounted


## A [State] object for [StateMachine].


## Emitted when [State] requests transition to a different [State]
@warning_ignore("unused_signal")
signal request_state_transition(state_name: String, data: Array)


## Executed when [State] becomes a current state for [StateMachine]
@abstract func on_enter(args: Array) -> void


## Executed when [State] stopps being a current state for [StateMachine]
@abstract func on_exit() -> void


## Update frame
@abstract func process(delta: float) -> void


## Fixed frame
@abstract func physics_process(delta: float) -> void


## Get state's script name
func get_class_name() -> String:
	return get_script().resource_path.get_file().get_basename()
