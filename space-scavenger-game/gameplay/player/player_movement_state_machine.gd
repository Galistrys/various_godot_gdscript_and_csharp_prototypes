class_name PlayerMovementStateMachine
extends StateMachine


## A [StateMachine] for [PlayerMovementState] states


@export var _player: Player


var _should_rotate := true
var _should_move := true

var _grav_zero := true


func _ready() -> void:
	register_state(PlayerWalkingState.ID, PlayerWalkingState.new(_player))
	register_state(PlayerFlyingState.ID, PlayerFlyingState.new(_player))
	
	await owner.ready
	transition(PlayerFlyingState.ID, [])


func _process(delta: float) -> void:
	var state := _current_state as PlayerMovementState
	state.set_should_rotate(_should_rotate)
	state.set_should_move(_should_move)
	super._process(delta)


func _physics_process(delta: float) -> void:
	super._physics_process(delta)
	
	var grav_zero := _player.get_gravity().length_squared() == 0
	if _grav_zero != grav_zero:
		_grav_zero = grav_zero
		
		if _current_state is PlayerFlyingState and not grav_zero:
			transition(PlayerWalkingState.ID, [])
		elif _current_state is PlayerWalkingState and grav_zero:
			transition(PlayerFlyingState.ID, [])


## Set if [Player] should be allowed to rotate
func set_should_rotate(value: bool) -> void:
	_should_rotate = value


## Set if [Player] should be allowed to move
func set_should_move(value: bool) -> void:
	_should_move = value
