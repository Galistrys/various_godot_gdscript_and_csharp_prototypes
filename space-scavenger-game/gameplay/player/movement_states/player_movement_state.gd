@abstract
class_name PlayerMovementState
extends PlayerState


## Abstract class to be used by [PlayerMovementStateMachine]


var _should_rotate := true
var _should_move := true


## Set if [Player] should be allowed to rotate
func set_should_rotate(value: bool) -> void:
	_should_rotate = value


## Set if [Player] should be allowed to move
func set_should_move(value: bool) -> void:
	_should_move = value



func update(delta: float) -> void:
	if _should_rotate:
		_rotate(delta)


func physics_update(delta: float) -> void:
	if _should_move:
		_move(delta)


@abstract func _rotate(delta: float) -> void
@abstract func _move(delta: float) -> void
