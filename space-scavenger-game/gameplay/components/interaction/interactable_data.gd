class_name InteractableData
extends Resource


## Data container for interaction area shape and relative position


## Interaction area's shape
@export var shape: Shape3D

## Interaction area's position
@export var position: Vector3


func _init(
	p_shape: Shape3D = null,
	p_position: Vector3 = Vector3.ZERO
) -> void:
	shape = p_shape
	position = p_position
	
