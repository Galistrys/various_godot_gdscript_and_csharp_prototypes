class_name InputManager
extends Node


## Player's input manager
## 
## This class unifies and manages player's input.
## The input is send over global event bus.


## Available providers
enum Provider {
	## Mouse and Keyboard
	MOUSE_AND_KEYBOARD,
	## Physical Joypad device
	GAMEPAD
}


const _CFG_SECTION = &"input"


static var _instance: InputManager

@warning_ignore("unused_private_class_variable")
var _data_object: Dictionary[StringName, Variant] = {} # Use when changing controls is being implemented

var _current_input_provider: InputProvider = null


@onready var _mk_input_provider := $MkInputProvider as MkInputProvider
@onready var _gamepad_input_provider := $GamepadInputProvider as GamepadInputProvider


## Get movement vector
static func get_movement() -> Vector3:
	return _instance.get_movement_vector()


## Get rotation vector
static func get_rotation() -> Vector3:
	return _instance.get_rotation_vector()


## Get vertical move value
static func get_vertical() -> float:
	return _instance.get_vertical_value()


static func set_mouse_visible(p_visible: bool) -> void:
	if p_visible:
		Input.mouse_mode = Input.MOUSE_MODE_VISIBLE
	else:
		Input.mouse_mode = Input.MOUSE_MODE_HIDDEN


func _ready() -> void:
	set_mouse_visible(true)


func _unhandled_key_input(event: InputEvent) -> void:
	if event is InputEventKey:
		if event.pressed and event.keycode == KEY_F10:
			set_mouse_visible(Input.mouse_mode == Input.MOUSE_MODE_HIDDEN)


func _unhandled_input(event: InputEvent) -> void:
	if event is InputEventKey or event is InputEventMouseMotion:
		_change_provider(Provider.MOUSE_AND_KEYBOARD)
	elif event is InputEventJoypadButton or event is InputEventJoypadMotion:
		_change_provider(Provider.GAMEPAD)


func _exit_tree() -> void:
	_instance = null


func initialize_threaded() -> void:
	print("[InputManager]: Initialization started")
	
	_instance = self
	_set_up_providers()
	
	print("[InputManager]: Initialization finished")


## Returns movement input vector
## [LEFT-RIGHT, FORWARD-BACKWARD, DOWN-UP]
func get_movement_vector() -> Vector3:
	return _current_input_provider.get_movement_vector()


## Returns rotation input vector
## [PITCH, YAW, ROLL]
func get_rotation_vector() -> Vector3:
	return _current_input_provider.get_rotation_vector()


## Returns vertical input value
func get_vertical_value() -> float:
	return _current_input_provider.get_vertical_value()


## Disables current [InputProvider] and enables [InputProvider]
## associated with [enum InputManager.Provider] value
func _change_provider(provider: Provider) -> void:
	if _current_input_provider != null:
		_current_input_provider.disable()
	
	match provider:
		Provider.MOUSE_AND_KEYBOARD:
			_current_input_provider = _mk_input_provider
		Provider.GAMEPAD:
			_current_input_provider = _gamepad_input_provider
	
	_current_input_provider.enable()


func _set_up_providers() -> void:
	_mk_input_provider.disable()
	_gamepad_input_provider.disable()
	
	_mk_input_provider.cursor_shape_requested.connect(
			CursorService.set_cursor_shape)
	_gamepad_input_provider.cursor_shape_requested.connect(
			CursorService.set_cursor_shape)
	
	_change_provider(Provider.MOUSE_AND_KEYBOARD)
