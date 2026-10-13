class_name InteractableComponent
extends Area3D


## Enables interaction interface for an entity


@export var _interaction_object: Node3D
@export var _interaction_col_shape: CollisionShape3D


func _init() -> void:
	collision_layer = Constants.CollisionLayer.INTERACTABLE


## Initializes [InteractableComponent]
func initialize(interactable_data: InteractableData) -> void:
	assert(
			is_node_ready(),
			"[InteractableComponent]: Initialize can only be called after ready")
	
	_interaction_col_shape.shape = interactable_data.shape
	_interaction_col_shape.position = interactable_data.position


## Returns paired entity
func get_interacation_object() -> Node3D:
	return _interaction_object
