extends KinematicBody2D
class_name ShipController

const PI_HALF = PI / 2.0
const SHIP_MAIN_SPEED = 300.0
const SHIP_ACCELERATION = 1.0
const SHIP_ROTATION_SPEED = 1.0

const DOT_MAX = 1
const DOT_MIN = -1

const SPEED_CALC_MAX = 1
const SPEED_CALC_MIN = 0.1

const MAX_SAILS_ANGLE = deg2rad(70.0)
const SAIL_ROTATION_SPEED = 1.0

var _ship_target_direction := Vector2.DOWN
var _ship_current_direction := Vector2.DOWN
var _sails_current_local_rotation := 0.0

var _velocity := 0.0

onready var _large_sail_sprite: Sprite = $SailLarge
onready var _smail_sail_sprite: Sprite = $SailSmall

func _ready():
	pass


func _physics_process(delta):
	_rotate_ship(delta)
	_adjust_sail_rotation(delta)
	
	var target_speed: float = _calculate_target_speed()
	_velocity = lerp(_velocity, target_speed, SHIP_ACCELERATION * delta)
	var collision: KinematicCollision2D = move_and_collide(
		_ship_current_direction * _velocity * delta
	)
	if collision != null and collision.collider is TileMap:
		_ship_current_direction = collision.normal
		_ship_target_direction = collision.normal
		rotation = _ship_current_direction.angle()


func set_target_direction(target_direction: Vector2) -> void:
	_ship_target_direction = target_direction

func _rotate_ship(delta: float) -> void:
	var angle_to_make: float = _ship_current_direction \
		.angle_to(_ship_target_direction)
	_ship_current_direction = _ship_current_direction.rotated(
		sign(angle_to_make) \
		* min(SHIP_ROTATION_SPEED * delta, abs(angle_to_make))
	)
	self.rotation = _ship_current_direction.angle() - PI_HALF

func _adjust_sail_rotation(delta: float) -> void:
	var target_local: Vector2 = WindManager.get_wind_direction() \
		.rotated(-_ship_current_direction.angle())
	target_local = Vector2.RIGHT.rotated(
		clamp(target_local.angle(), -MAX_SAILS_ANGLE, MAX_SAILS_ANGLE)
	)
	
	var current_local: Vector2 = Vector2.RIGHT \
		.rotated(_sails_current_local_rotation)
	var local_angle_to_make: float = current_local.angle_to(target_local)
	
	_sails_current_local_rotation += sign(local_angle_to_make) \
		* min(SAIL_ROTATION_SPEED * delta, abs(local_angle_to_make))
	_large_sail_sprite.rotation = _sails_current_local_rotation
	_smail_sail_sprite.rotation = _sails_current_local_rotation

func _calculate_target_speed() -> float:
	var local_sails_direction: Vector2 = Vector2.RIGHT.rotated(_sails_current_local_rotation)
	var wind_local_direction: Vector2 = WindManager.get_wind_direction().rotated(-_ship_current_direction.angle())
	var dot: float = local_sails_direction.dot(wind_local_direction)
	var target_speed: float = range_lerp(dot, DOT_MIN, DOT_MAX, SPEED_CALC_MIN, SPEED_CALC_MAX)
	
	return SHIP_MAIN_SPEED * target_speed * WindManager.get_wind_strength()
