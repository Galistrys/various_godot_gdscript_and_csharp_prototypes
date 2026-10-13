class_name GravityZone
extends Area3D


@export_node_path(AnimatableBody3D) var _gravity_reference_object_path = NodePath("")
@onready var _gravity_reference_object: GravityReferenceObject = get_node(_gravity_reference_object_path)


func _physics_process(_delta):
	gravity_direction = _gravity_reference_object.global_transform.basis * Vector3.DOWN


func _on_body_entered(body):
	if body is PlayerController:
		body.add_gravity_reference_object(_gravity_reference_object)


func _on_body_exited(body):
	if body is PlayerController:
		body.remove_gravity_reference_object(_gravity_reference_object)
