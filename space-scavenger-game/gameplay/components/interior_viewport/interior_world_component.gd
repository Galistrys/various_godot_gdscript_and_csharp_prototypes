class_name InteriorWorldComponent
extends Node3D


@export var _interior_scene: PackedScene


@onready var _exterior_camera := $ExteriorCamera as ExteriorCamera


@onready var _interior_viewport := $InteriorViewport as InteriorViewport

@onready var _sun := $InteriorViewport/DirectionalLight3D as DirectionalLight3D
@warning_ignore("unused_private_class_variable")
@onready var _env := $InteriorViewport/WorldEnvironment as WorldEnvironment


func _ready() -> void:
	_on_viewport_size_changed()
	get_viewport().size_changed.connect(_on_viewport_size_changed)
	
	var interior_instance := _interior_scene.instantiate()
	_interior_viewport.add_child(interior_instance)


@warning_ignore("unused_parameter")
func _process(delta: float) -> void:
	_update_relative_sun_rotation()


func transfer_node_to_interior(node: Node3D, target_transform: Transform3D) -> void:
	node.reparent(_interior_viewport)
	node.global_transform = target_transform


func follow_camera(camera: Camera3D) -> void:
	_exterior_camera.set_tracked_camera(camera)


func _update_relative_sun_rotation() -> void:
	_sun.basis = global_basis.transposed() * EnvironmentManager.instance.get_sun_basis()


func _on_viewport_size_changed():
	var screen_size := get_viewport().get_visible_rect().size
	_interior_viewport.size = screen_size
