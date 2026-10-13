class_name GrabbableData
extends Resource


## Data container for [GrabbableObject]


## Object's model
@export var model: Mesh

## Object's collision shape
@export var shape: Shape3D

## Object's interactable data
@export var interactable_data: InteractableData


func _init(
	p_model: Mesh = null,
	p_shape: Shape3D = null,
	p_interactable_data: InteractableData = null
) -> void:
	model = p_model
	shape = p_shape
	interactable_data = p_interactable_data
