class_name ValueComponent
extends Node


## Compoment that handles storing a value


## Emitted when max value changed
signal max_value_changed(new_value: float)
## Emitted when current value changed
signal value_changed(new_value: float)
## Emitted when value dropped to zero
signal value_zero


## Max value
@export var max_value: float:
	get:
		return max_value
	set(value):
		assert(value > 0.0, "Max value must be more than 0.0")
		var diff := value - max_value
		
		max_value = value
		max_value_changed.emit(value)
		
		if diff > 0:
			current_value += diff
		else:
			current_value = clampf(current_value, 0.0, value)


## Current value
var current_value: float:
	get:
		return current_value
	set(value):
		current_value = clampf(value, 0.0, max_value)
		value_changed.emit(current_value)
		if current_value == 0.0:
			value_zero.emit()


func _ready() -> void:
	current_value = max_value
