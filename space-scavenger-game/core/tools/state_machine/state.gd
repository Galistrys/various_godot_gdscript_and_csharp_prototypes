@abstract
class_name State
extends RefCounted


## This is an abstract class to be used by [StateMachine]


## Emitted when a [State] requests a transition to the given [State]
@warning_ignore("unused_signal")
signal request_transition(state_name: StringName, data: Array)


## Initializes the [State]
func initialize() -> void:
	pass


## Called when [State] becames active
@warning_ignore("unused_parameter")
func on_enter(data: Array) -> void:
	pass


## Called every [method Node._process], when [State] is active
@warning_ignore("unused_parameter")
func update(delta: float) -> void:
	pass


## Called every [method Node._physics_process], when [State] is active
@warning_ignore("unused_parameter")
func physics_update(delta: float) -> void:
	pass


## Called when [State] becames inactive
@warning_ignore("unused_parameter")
func on_exit() -> void:
	pass
