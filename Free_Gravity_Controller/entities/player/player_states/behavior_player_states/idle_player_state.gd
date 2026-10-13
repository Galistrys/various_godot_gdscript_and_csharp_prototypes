class_name IdlePlayerState
extends BehaviorPlayerState


var _interaction_ray: RayCast3D
var _current_interaction_object: Object = null


func _init(state_machine, interaction_ray: RayCast3D):
	super(state_machine)
	_interaction_ray = interaction_ray


func on_enter(_args: Array) -> void:
	_current_interaction_object = null


func physics_process(_delta: float) -> void:
	_update_current_interaction_object()


func _update_current_interaction_object() -> void:
	var curr := _interaction_ray.get_collider()
	if curr != _current_interaction_object:
		_current_interaction_object = curr
		# Update UI


func on_exit() -> void:
	_current_interaction_object = null
