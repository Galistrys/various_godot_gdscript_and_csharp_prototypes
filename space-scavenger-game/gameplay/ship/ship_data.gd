class_name ShipData
extends Resource


## Data container for [ShipController]


## Ship's external scene
@export var external_scene: PackedScene

## Ship's internal scene
@export var internal_scene: PackedScene


func _init(
	p_external_scene: PackedScene = null,
	p_internal_scene: PackedScene = null
) -> void:
	external_scene = p_external_scene
	internal_scene = p_internal_scene
