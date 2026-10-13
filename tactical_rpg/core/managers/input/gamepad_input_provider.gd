class_name GamepadInputProvider
extends InputProvider


## Gamepad input provider
## 
## This input provider returns input from any kind of Gamepad


const _ROTATION_MOD := 0.01


func get_movement_vector() -> Vector3:
	var hor_val := Input.get_vector(&"move_left", &"move_right",
			&"move_forward", &"move_backward")
	return Vector3(hor_val.x, 0.0, hor_val.y)


func get_rotation_vector() -> Vector3:
	var look_rot := -Input.get_vector(&"look_left", &"look_right",
			&"look_up", &"look_down") * _ROTATION_MOD
	return Vector3(look_rot.x, look_rot.y, 0.0)


func get_vertical_value() -> float:
	var result := Input.get_axis(&"move_down", &"move_up")
	return result
