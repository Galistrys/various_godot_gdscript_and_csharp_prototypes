@abstract
class_name InputProvider
extends Node


## Base class for player input providers.
## 
## This class provides an interface to recieve input from player.


## Disables input provider
func disable() -> void:
	process_mode = Node.PROCESS_MODE_DISABLED


## Enables input provider
func enable() -> void:
	process_mode = Node.PROCESS_MODE_INHERIT


## Returns movement vector [left-right, down-up, forward-backward]
@abstract func get_movement_vector() -> Vector3

## Returns rotation vector [left-right, down-up, roll_left-roll_right]
@abstract func get_rotation_vector() -> Vector3

## Returns distance change [closer-further]
@abstract func get_distance_direction() -> float
