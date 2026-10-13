class_name GravityReferenceObject
extends AnimatableBody3D

const ACCELERATION = 1.0
const ANGULAR_ACCELERATION = 0.25

var linear_velocity: Vector3 = Vector3.ZERO
var angular_velocity: Vector3 = Vector3.ZERO

var _linear_thrust: Vector3 = Vector3.ZERO
var _angular_thrust: Vector3 = Vector3.ZERO

var velocity_damp: bool = false
var angular_velocity_damp: bool = false


func _ready() -> void:
	pass


## Applies thrust of [b]thrust[/b] to the body for one frame.
## [b]thrust[/b] vector should be normalized.
func apply_linear_thrust(thrust: Vector3) -> void:
	_linear_thrust = thrust


func apply_angular_thrust(thrust: Vector3) -> void:
	_angular_thrust = thrust


func _physics_process(delta: float) -> void:
#	printt(velocity, angular_velocity)
	if angular_velocity_damp and _angular_thrust.length_squared() == 0:
		angular_velocity = angular_velocity.move_toward(
			Vector3.ZERO, delta * ANGULAR_ACCELERATION
		)
	else:
		angular_velocity = angular_velocity.move_toward(
			angular_velocity + _angular_thrust, delta * ANGULAR_ACCELERATION
		)
	rotate_object_local(Vector3.RIGHT, delta * angular_velocity.x)
	rotate_object_local(Vector3.UP, delta * angular_velocity.y)
	rotate_object_local(Vector3.BACK, delta * angular_velocity.z)
	
	if velocity_damp and _linear_thrust.length_squared() == 0:
		linear_velocity = linear_velocity.move_toward(
			Vector3.ZERO, delta * ACCELERATION
		)
	else:
		linear_velocity = linear_velocity.move_toward(
			linear_velocity + _linear_thrust, delta * ACCELERATION
		)
	linear_velocity = linear_velocity.move_toward(
		linear_velocity + _linear_thrust, delta * ACCELERATION
	)
	@warning_ignore(return_value_discarded)
	move_and_collide(linear_velocity * delta)
	
	_linear_thrust = Vector3.ZERO
	_angular_thrust = Vector3.ZERO
