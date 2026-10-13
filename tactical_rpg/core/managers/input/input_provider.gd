class_name InputProvider
extends Node


## Base class for player input providers.
## 
## This class provides an interface to recieve input from player.


## Emits when [InputProvider] requests cursor's shape to be changed
@warning_ignore("unused_signal")
signal cursor_shape_requested(cursor_shape: CursorManager.GameCursorShape)


## Disables input provider
func disable() -> void:
	process_mode = Node.PROCESS_MODE_DISABLED


## Enables input provider
func enable() -> void:
	process_mode = Node.PROCESS_MODE_INHERIT


## Returns movement vector:
## [LEFT-RIGHT, FORWARD-BACKWARD, DOWN-UP]
func get_movement_vector() -> Vector3:
	assert(false, "Method not implemented")
	return Vector3.ZERO


## Returns rotation vector:
## [PITCH, YAW, ROLL]
func get_rotation_vector() -> Vector3:
	assert(false, "Method not implemented")
	return Vector3.ZERO


## Returns vertical input
func get_vertical_value() -> float:
	assert(false, "Method not implemented")
	return 0.0
