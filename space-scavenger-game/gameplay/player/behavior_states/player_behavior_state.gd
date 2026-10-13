@abstract
class_name PlayerBehaviorState
extends PlayerState


## Abstract class to be used by [PlayerBehaviorStateMachine]


var _interaction_ray_cast: RayCast3D


func _init(player: Player, interaction_ray_cast: RayCast3D) -> void:
	super(player)
	_interaction_ray_cast = interaction_ray_cast


## Returns [code]true[/code] if [PlayerMovementStateMachine]
## should allow to rotate player
@abstract func player_should_rotate() -> bool

## Returns [code]true[/code] if [PlayerMovementStateMachine]
## should allow to move player
@abstract func player_should_move() -> bool
