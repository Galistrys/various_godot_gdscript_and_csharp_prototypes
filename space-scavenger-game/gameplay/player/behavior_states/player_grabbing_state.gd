class_name PlayerGrabbingState
extends PlayerBehaviorState


## State for [PlayerBehaviorStateMachine] when [Player] is grabbing a
## [GrabbableObject]


## State's ID
const ID = &"PlayerGrabbingState"

## [GrabbableObject] linear velocity modifier
const MOVE_SPEED = 8.0

## Max distance a [GrabbableObject] can be from the [Player]
const MAX_DISTANCE = 2.5
## Min distance a [GrabbableObject] can be from the [Player]
const MIN_DISTANCE = 1.0
## Speed at which [GrabbableObject] should change distance from [Player]
const CLOSING_SPEED = 4.0

## [GrabbableObject] angular velocity modifier
const ROTATION_SPEED = 1.0
## [GrabbableObject] angular velocity dampening
const ANGULAR_DAMP = 4.0

## Throw force strength
const THROW_FORCE = 8.0


var _grabbable_object: GrabbableObject
var _rotate_mode: bool
var _distance: float

var _angular_damp: float


func on_enter(data: Array) -> void:
	assert(
			data[0] is GrabbableObject,
			"[PlayerGrabbingState]: Object provided is not a GrabbableObject")
	
	_grabbable_object = data[0] as GrabbableObject
	_grabbable_object.gravity_scale = 0.0
	
	_angular_damp = _grabbable_object.angular_damp
	_grabbable_object.angular_damp = ANGULAR_DAMP
	
	_rotate_mode = false
	
	_distance = inverse_lerp(
			MIN_DISTANCE, MAX_DISTANCE, clampf(
					_grabbable_object.global_position.distance_to(
							_interaction_ray_cast.global_position),
					MIN_DISTANCE, MAX_DISTANCE))
	
	print("[PlayerGrabbingState] <%s>: Object grabbed" % _grabbable_object.name)


func on_exit() -> void:
	print("[PlayerGrabbingState] <%s>: Object released" % _grabbable_object.name)
	_grabbable_object.gravity_scale = 1.0
	_grabbable_object.angular_damp = _angular_damp
	_grabbable_object = null


@warning_ignore("unused_parameter")
func update(delta: float) -> void:
	if not InputService.is_interaction_pressed():
		request_transition.emit(PlayerIdleState.ID, [])
	if InputService.is_mid_interaction_just_pressed():
		var direction := -_interaction_ray_cast.global_basis.z
		_grabbable_object.apply_central_impulse(direction * THROW_FORCE)
		request_transition.emit(PlayerIdleState.ID, [])
	
	_distance = clampf(
			_distance + InputService.get_distance_direction() \
					* delta * CLOSING_SPEED,
			0.0, 1.0)
	
	_rotate_mode = InputService.is_in_rotate_mode()


@warning_ignore("unused_parameter")
func physics_update(delta: float) -> void:
	var target_position := _interaction_ray_cast.global_position + (
			-_interaction_ray_cast.global_basis.z * lerpf(MIN_DISTANCE, MAX_DISTANCE, _distance))
	var target_move := target_position - _grabbable_object.global_position
	_grabbable_object.linear_velocity = target_move * MOVE_SPEED
	
	if _rotate_mode:
		var rot_vec := InputService.get_rotation_vector() * ROTATION_SPEED
		var res := Vector3(rot_vec.y, rot_vec.x, 0.0)
		_grabbable_object.angular_velocity -= _interaction_ray_cast.global_basis * res


func player_should_rotate() -> bool:
	return not _rotate_mode


func player_should_move() -> bool:
	return true
