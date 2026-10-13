class_name WalkingPlayerState
extends MovementPlayerState


const VIEW_MIN_ANGLE = deg_to_rad(-80.0)
const VIEW_MAX_ANGLE = deg_to_rad(80.0)

const CAMERA_DIF = 1.55

const RUN_SPEED = 3
const SPEED = 1
const ACCELERATION = 5

var _grav_object: GravityReferenceObject

var _character: CharacterBody3D
var _head: Node3D
var _floor_ray: RayCast3D
var _standing_col: CollisionShape3D
var _fly_col: CollisionShape3D


func _init(
	character: CharacterBody3D,
	head: Node3D,
	floor_ray: RayCast3D,
	standing_col: CollisionShape3D,
	fly_col: CollisionShape3D
):
	_character = character
	_head = head
	_floor_ray = floor_ray
	_standing_col = standing_col
	_fly_col = fly_col


func on_enter(args: Array) -> void:
	_grav_object = args[0] as GravityReferenceObject
	
	_standing_col.set_deferred("disabled", false)
	_fly_col.set_deferred("disabled", true)
	
	var point_to_look = _character.global_transform.origin \
		- _character.global_transform.basis.z
	_rotate_to_gravity()
	var plane = Plane(_grav_object.global_transform.basis.y)
	var point_on_plane = plane.project(point_to_look)
	_character.global_transform = _character.global_transform.looking_at(
		point_on_plane, _grav_object.global_transform.basis.y
	)
	
	_floor_ray.force_update_transform()
	_floor_ray.force_raycast_update()
	
	if _floor_ray.is_colliding():
		_character.global_position = _floor_ray.get_collision_point() \
			+ _character.transform.basis.y * CAMERA_DIF


func rotate(
	delta: float,
	input: Vector3
) -> void:
	_rotate_to_gravity()
	_character.rotate_object_local(Vector3.UP, input.y * delta)
	_head.rotation.x = clamp(
		_head.rotation.x + input.x * delta,
		VIEW_MIN_ANGLE, VIEW_MAX_ANGLE
	)


func move(
	delta: float,
	input: Vector3
) -> void:
	var vel = Vector3.ZERO
	var vel_y = (_character.velocity * _character.transform.basis).y
	
	if not _character.is_on_floor():
		vel_y -= 9.8 * delta
	elif Input.is_action_just_pressed("ui_accept"):
		vel_y = 4.5
	else:
		vel_y = 0
	
	var direction = Vector3(input.x, 0, input.z)
	if direction.length_squared() > 1:
		direction = direction.normalized()
	
	vel = direction * (
		RUN_SPEED if Input.is_action_pressed("sprint") else SPEED
	)
	vel.y = vel_y
	
	_character.velocity = _character.transform.basis * vel
	
	# Czy zastosować do Eva_Player_State? 
	# Chyba lepiej utworzyć nowy stan dla nieważkości w środku jakiegoś pola
	if not _character.is_on_floor(): 
		_character.velocity += _grav_object.velocity
	@warning_ignore(return_value_discarded)
	_character.move_and_slide()


func on_exit() -> void:
	_grav_object = null


func set_reference_object(grav_object: GravityReferenceObject) -> void:
	_grav_object = grav_object


func _rotate_to_gravity() -> void:
	var b = _character.transform.basis.x
	var a = _grav_object.transform.basis.y
	var look_point = a.cross(b)
	_character.up_direction = a
	_character.transform = _character.transform.looking_at(
		look_point + _character.transform.origin, a
	)
