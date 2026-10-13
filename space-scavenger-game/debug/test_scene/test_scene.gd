class_name TestScene
extends Node3D


var _space: RID

var _floor_body: RID
var _floor_shape: RID

var _ball_body: RID
var _ball_shape: RID

@onready var _floor_mi: MeshInstance3D = $Floor
@onready var _ball_mi: MeshInstance3D = $Ball


func _ready() -> void:
	_space = PhysicsServer3D.space_create()
	#PhysicsServer3D.area_set_param(_space, PhysicsServer3D.AREA_PARAM_GRAVITY, 0.0)
	#PhysicsServer3D.area_set_param(_space, PhysicsServer3D.AREA_PARAM_GRAVITY_VECTOR, Vector3.ZERO)
	PhysicsServer3D.space_set_active(_space, true)
	
	_floor_body = PhysicsServer3D.body_create()
	PhysicsServer3D.body_set_mode(_floor_body, PhysicsServer3D.BODY_MODE_STATIC)
	_floor_shape = PhysicsServer3D.box_shape_create()
	PhysicsServer3D.shape_set_data(_floor_shape, Vector3(8.0, 1.0, 8.0))
	PhysicsServer3D.body_add_shape(_floor_body, _floor_shape)
	PhysicsServer3D.body_set_space(_floor_body, _space)
	PhysicsServer3D.body_set_state(_floor_body, PhysicsServer3D.BODY_STATE_TRANSFORM, _floor_mi.global_transform)
	
	_ball_body = PhysicsServer3D.body_create()
	PhysicsServer3D.body_set_mode(_ball_body, PhysicsServer3D.BODY_MODE_RIGID)
	_ball_shape = PhysicsServer3D.sphere_shape_create()
	PhysicsServer3D.shape_set_data(_ball_shape, 0.5)
	PhysicsServer3D.body_add_shape(_ball_body, _ball_shape)
	PhysicsServer3D.body_set_space(_ball_body, _space)
	PhysicsServer3D.body_set_state(_ball_body, PhysicsServer3D.BODY_STATE_TRANSFORM, _ball_mi.global_transform)
	PhysicsServer3D.body_set_force_integration_callback(_ball_body, _body_moved)


func _body_moved(state: PhysicsDirectBodyState3D) -> void:
	_ball_mi.transform = state.transform


func _exit_tree() -> void:
	PhysicsServer3D.free_rid(_ball_shape)
	PhysicsServer3D.free_rid(_ball_body)
	PhysicsServer3D.free_rid(_floor_shape)
	PhysicsServer3D.free_rid(_floor_body)
	PhysicsServer3D.free_rid(_space)
