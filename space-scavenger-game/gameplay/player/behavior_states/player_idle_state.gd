class_name PlayerIdleState
extends PlayerBehaviorState


## Default [Player]'s behavior state


## State's ID
const ID = &"PlayerIdleState"


var _previous_collider: Object = null
var _current_ic: InteractableComponent


@warning_ignore("unused_parameter")
func physics_update(delta: float) -> void:
	var collider := _interaction_ray_cast.get_collider()
	
	if collider != _previous_collider:
		if collider and collider is InteractableComponent:
			_current_ic = collider as InteractableComponent
			print("[PlayerIdleState]: Interactable object detected -> %s" \
					% _current_ic.get_interacation_object().name)
		else:
			_current_ic = null
	
	_previous_collider = collider


@warning_ignore("unused_parameter")
func update(delta: float) -> void:
	if _current_ic and InputService.is_interaction_just_pressed():
		request_transition.emit(
				PlayerGrabbingState.ID, [_current_ic.get_interacation_object()])


func on_exit() -> void:
	_previous_collider = null
	_current_ic = null


func player_should_rotate() -> bool:
	return true


func player_should_move() -> bool:
	return true
