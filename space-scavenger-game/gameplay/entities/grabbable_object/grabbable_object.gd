class_name GrabbableObject
extends RigidBody3D


## Generic grabbable object


@onready var _model: MeshInstance3D = $Model
@onready var _col_shape: CollisionShape3D = $ColShape
@onready var _interactable_component: InteractableComponent = $InteractableComponent


## Initializes [GrabbableObject] with given [param grabbable_data]
func initialize(grabbable_data: GrabbableData) -> void:
	assert(
			is_node_ready(),
			"[GrabbableObject]: Initialize can only be called after ready")
	
	_model.mesh = grabbable_data.model
	_col_shape.shape = grabbable_data.shape
	_interactable_component.initialize(grabbable_data.interactable_data)
