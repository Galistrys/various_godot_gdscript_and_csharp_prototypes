class_name MkInputProvider
extends InputProvider


## Mouse and Keyboard input provider
## 
## This input provider returns input from both mouse and keyboard


var _mouse_input := Vector2.ZERO

var _mouse_sensitivity: float = 0.5


func _input(event: InputEvent) -> void:
	if event is InputEventMouseMotion:
		var viewport_transform: Transform2D = get_tree().root.get_final_transform()
		var motion: Vector2 = event.xformed_by(viewport_transform).relative
		_mouse_input = -(motion * get_process_delta_time() * _mouse_sensitivity).clamp(
				-Vector2.ONE, Vector2.ONE)


func get_movement_vector() -> Vector3:
	var hor_val := Input.get_vector(
			&"move_left", &"move_right", &"move_forward", &"move_backward")
	var ver_val := int(Input.is_action_pressed(&"move_up")) \
			- int(Input.is_action_pressed(&"move_down"))
	return Vector3(hor_val.x, ver_val, hor_val.y)


func get_rotation_vector() -> Vector3:
	var for_rot := int(Input.is_action_pressed(&"rotate_left")) \
			- int(Input.is_action_pressed(&"rotate_right"))
	var result := Vector3(_mouse_input.x, _mouse_input.y, for_rot)
	_mouse_input = Vector2.ZERO
	return result


func get_distance_direction() -> float:
	var res := 0.0
	if Input.is_action_just_released(&"move_further"):
		res = 1.0
	if Input.is_action_just_released(&"move_closer"):
		res = -1.0
	return res
