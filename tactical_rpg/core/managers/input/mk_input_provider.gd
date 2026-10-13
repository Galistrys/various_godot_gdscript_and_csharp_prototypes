class_name MkInputProvider
extends InputProvider


## Mouse and Keyboard input provider
## 
## This input provider returns input from both mouse and keyboard


var _mouse_input := Vector2.ZERO

var _mouse_sensitivity: float = 0.5


func _unhandled_input(event: InputEvent) -> void:
	if event is InputEventMouseMotion:
		var viewport_transform: Transform2D = get_tree().root.get_final_transform()
		var motion: Vector2 = event.xformed_by(viewport_transform).relative
		_mouse_input = -(motion * get_process_delta_time() * _mouse_sensitivity).clamp(
				-Vector2.ONE, Vector2.ONE)


func get_movement_vector() -> Vector3:
	var hor_val := Input.get_vector(
			&"move_left", &"move_right", &"move_forward", &"move_backward")
	return Vector3(hor_val.x, 0.0, hor_val.y)


func get_rotation_vector() -> Vector3:
	var result: Vector3 = Vector3.ZERO
	
	if Input.is_mouse_button_pressed(MOUSE_BUTTON_MIDDLE):
		cursor_shape_requested.emit(CursorManager.GameCursorShape.ROTATE)
		result = Vector3(_mouse_input.x, _mouse_input.y, 0.0)
	else:
		result = Vector3(
				Input.get_axis(&"look_left", &"look_right"),
				Input.get_axis(&"look_down", &"look_up") \
						if not Input.is_key_pressed(KEY_ALT) else 0.0,
				0.0)
		result *= get_process_delta_time()
		
		if result:
			cursor_shape_requested.emit(CursorManager.GameCursorShape.ROTATE)
		else:
			cursor_shape_requested.emit(CursorManager.GameCursorShape.DEFAULT)
	
	_mouse_input = Vector2.ZERO
	return result


func get_vertical_value() -> float:
	var result := 0.0
	if Input.is_key_pressed(KEY_ALT):
		result = float(Input.get_action_strength(&"move_down")) \
			- float(Input.get_action_strength(&"move_up"))
		result /= 2.0
	else:
		result = float(Input.is_action_just_released(&"mouse_move_up")) \
			- float(Input.is_action_just_released(&"mouse_move_down"))
	
	return result
