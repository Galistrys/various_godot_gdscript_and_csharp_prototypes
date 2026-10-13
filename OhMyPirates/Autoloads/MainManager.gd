extends Node

func _ready():
	pass

func _input(event):
	if event is InputEventKey:
		if event.pressed:
			if event.scancode == KEY_F11:
				OS.window_fullscreen = not OS.window_fullscreen
