class_name Player
extends CharacterBody3D


## Player's movement state, when in nonzero-gravity environment


## Player's walking speed.
const WALK_SPEED = 1.5
## Player's run speed.
const RUN_SPEED = 4.0
## Player's crouching speed.
const CROUCH_SPEED = 0.75
## Player's jumping power
const JUMP_POWER = 4.5

## Player's acceleration.
const ACCELERATION = 5.0
## Player's deacceleration.
const DEACCELERATION = 10.0
## Player's acceleration mid air.
const MID_AIR_ACCELERATION = 0.5

## Camera diff for floor ray.
const CAMERA_DIF = 1.55
## Minimal angle for the camera in X axis in radians.
const VIEW_MIN_ANGLE = deg_to_rad(-85.0)
## Maximal angle for the camera in X axis in radians.
const VIEW_MAX_ANGLE = deg_to_rad(85.0)


var _target_head_position: Vector3
var _posture_tween: Tween


## Head [Node3D] node
@onready var head := $Head as Node3D
## Eyes [Node3D] node
@onready var eyes := $Head/Eyes as Node3D
## Camera [Camera3D] node
@onready var camera := $Head/Eyes/PlayerCamera as Camera3D

## Standing [CollisionShape3D] node
@onready var standing_col := $StandingCol as CollisionShape3D
## Crouching [CollisionShape3D] node
@onready var crouching_col := $CrouchingCol as CollisionShape3D
## Flying [CollisionShape3D] node
@onready var flying_col := $FlyingCol as CollisionShape3D

## Standing [RayCast3D] node
@onready var stand_ray_cast := $StandRayCast as RayCast3D
## Floor [RayCast3D] node
@onready var floor_ray_cast := $FloorRayCast as RayCast3D


func _ready() -> void:
	Input.mouse_mode = Input.MOUSE_MODE_CAPTURED


@warning_ignore("unused_parameter")
func _process(delta: float) -> void:
	head.position = _target_head_position
	camera.global_transform = eyes.get_global_transform_interpolated()


## Enable standing [CollisionShape3D]
func enable_standing_posture() -> void:
	if not standing_col.disabled:
		return
	if _posture_tween:
		_posture_tween.kill()
	_posture_tween = create_tween()
	_posture_tween.tween_property(self, ^"_target_head_position", Vector3.ZERO, 0.1)
	standing_col.disabled = false
	crouching_col.disabled = true
	flying_col.disabled = true


## Enable crouching [CollisionShape3D]
func enable_crouching_posture() -> void:
	if not crouching_col.disabled:
		return
	if _posture_tween:
		_posture_tween.kill()
	_posture_tween = create_tween()
	_posture_tween.tween_property(self, ^"_target_head_position", Vector3(0.0, -0.6, 0.0), 0.1)
	standing_col.disabled = true
	crouching_col.disabled = false
	flying_col.disabled = true


## Enable flying [CollisionShape3D]
func enable_flying_posture() -> void:
	if not flying_col.disabled:
		return
	if _posture_tween:
		_posture_tween.kill()
	_target_head_position = Vector3.ZERO
	standing_col.disabled = true
	crouching_col.disabled = true
	flying_col.disabled = false
