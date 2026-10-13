class_name PlayerBehaviorStateMachine
extends StateMachine


## A [StateMachine] for [PlayerBehaviorState] states


@export var _player: Player
@export var _interaction_ray_cast: RayCast3D

@export var _movement_state_machine: PlayerMovementStateMachine


func _ready() -> void:
	register_state(
			PlayerIdleState.ID,
			PlayerIdleState.new(_player, _interaction_ray_cast))
	register_state(
			PlayerGrabbingState.ID,
			PlayerGrabbingState.new(_player, _interaction_ray_cast))
	
	await owner.ready
	transition(PlayerIdleState.ID, [])


@warning_ignore("unused_parameter")
func _process(delta: float) -> void:
	super._process(delta)
	_movement_state_machine.set_should_rotate(
			(_current_state as PlayerBehaviorState).player_should_rotate())
	_movement_state_machine.set_should_move(
			(_current_state as PlayerBehaviorState).player_should_move())
