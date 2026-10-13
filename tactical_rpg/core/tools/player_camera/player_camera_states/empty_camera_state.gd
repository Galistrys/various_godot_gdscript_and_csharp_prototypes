class_name EmptyCameraState
extends PlayerCameraState


## Disabled state for [PlayerCamera]


## Executed when [State] becomes a current state for [StateMachine]
func on_enter(_args: Array) -> void:
	pass


## Executed when [State] stopps being a current state for [StateMachine]
func on_exit() -> void:
	pass


## Update frame
func process(_delta: float) -> void:
	pass


## Fixed frame
func physics_process(_delta: float) -> void:
	pass
