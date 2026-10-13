extends Node

const WIND_CHANGE_SPEED = 0.5

var _wind_direction: Vector2 = Vector2.RIGHT
var _wind_strength: float = 0.5

var _target_wind_direction: Vector2 = Vector2.RIGHT
var _target_wind_strength: float = 0.5

func _process(delta):
	_wind_direction = Vector2.RIGHT.rotated(
		lerp_angle(
			_wind_direction.angle(),
			_target_wind_direction.angle(),
			delta * WIND_CHANGE_SPEED
		)
	)
	_wind_strength = lerp(_wind_strength, _target_wind_strength, WIND_CHANGE_SPEED * delta)

func _input(event):
	if event is InputEventKey:
		if event.pressed:
			if event.scancode == KEY_SPACE:
				_target_wind_direction = Vector2.RIGHT.rotated(randf() * 2 * PI)
				_target_wind_strength = randf()

func get_wind_direction() -> Vector2:
	return _wind_direction

func get_wind_strength() -> float:
	return _wind_strength
