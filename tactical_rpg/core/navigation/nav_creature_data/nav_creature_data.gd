class_name NavCreatureData
extends Resource


## Container for navigation related data for different creatures types
##
## Contains information specific for creature type that are needed for navigation system, such as it's size


## Creature's radius
@export var creature_radius: float
## Creature's height
@export var creature_height: float


func _init(p_creature_radius: float = 0.0, p_creature_height: float = 0.0) -> void:
	creature_radius = p_creature_radius
	creature_height = p_creature_height
