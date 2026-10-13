class_name GamepadInputProvider
extends InputProvider


## Gamepad input provider
## 
## This input provider returns input from any kind of Gamepad


const ROTATION_MOD := 0.1


func get_movement_vector() -> Vector3:
	var hor_val := Input.get_vector(&"move_left", &"move_right",
			&"move_forward", &"move_backward")
	var ver_val := int(Input.is_action_pressed(&"move_up")) \
			- int(Input.is_action_pressed(&"move_down"))
	return Vector3(hor_val.x, ver_val, hor_val.y)


func get_rotation_vector() -> Vector3:
	var look_rot := -Input.get_vector(&"look_left", &"look_right",
			&"look_up", &"look_down") * ROTATION_MOD
	var for_rot := int(Input.is_action_pressed(&"rotate_left")) \
			- int(Input.is_action_pressed(&"rotate_right"))
	return Vector3(look_rot.x, look_rot.y, for_rot)


func get_distance_direction() -> float:
	return Input.get_axis(&"move_closer", &"move_further")
