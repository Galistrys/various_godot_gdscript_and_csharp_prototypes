class_name BehaviorPlayerState
extends Object

var _player_state_machine

func _init(state_machine):
	_player_state_machine = state_machine

func can_movement_recieve_input() -> bool:
	return true

func process(_delta: float) -> void:
	pass

func physics_process(_delta: float) -> void:
	pass
