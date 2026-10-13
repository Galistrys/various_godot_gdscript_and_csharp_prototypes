class_name IdlePlayerState
extends PlayerState

var _interaction_ray_cast: RayCast3D

func _init(
	state_machine,
	player_controller: CharacterBody3D,
	player_head: Node3D,
	interaction_ray_cast: RayCast3D
):
	super(state_machine, player_controller, player_head)
	_interaction_ray_cast = interaction_ray_cast
	
	
