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


## Player should be sprinting
var sprinting: bool:
	get:
		return _sprinting


## Player should be crouching
var crouching: bool:
	get:
		return _crouching


## If [code]true[/code], then sprinting is in toggle mode.
var sprinting_togglable: bool:
	get:
		return _sprinting_togglable
	set(value):
		_sprinting_togglable = value


## If [code]true[/code], then crouching is in toggle mode.
var crouching_togglable: bool:
	get:
		return _crouching_togglable
	set(value):
		_crouching_togglable = value


var _current_input_provider: InputProvider = null

var _sprinting_togglable := false
var _sprinting := false

var _crouching_togglable := false
var _crouching := false


@onready var _mk_input_provider := $MkInputProvider as MkInputProvider
@onready var _gamepad_input_provider := $GamepadInputProvider as GamepadInputProvider


func _ready() -> void:
	_set_up_providers()


func _unhandled_key_input(event: InputEvent) -> void:
	if event is InputEventKey:
		if event.pressed and event.keycode == KEY_F10:
			if Input.mouse_mode == Input.MOUSE_MODE_CAPTURED:
				Input.mouse_mode = Input.MOUSE_MODE_VISIBLE
			else:
				Input.mouse_mode = Input.MOUSE_MODE_CAPTURED


func _input(event: InputEvent) -> void:
	if event is InputEventKey or event is InputEventMouseMotion:
		change_provider(Provider.MOUSE_AND_KEYBOARD)
	elif event is InputEventJoypadButton or event is InputEventJoypadMotion:
		change_provider(Provider.GAMEPAD)


func _process(_delta: float) -> void:
	if _sprinting_togglable:
		if Input.is_action_just_pressed(&"sprint"):
			_sprinting = not _sprinting
	else:
		_sprinting = Input.is_action_pressed(&"sprint")
	
	if _crouching_togglable:
		if Input.is_action_just_pressed(&"crouch"):
			_crouching = not _crouching
	else:
		_crouching = Input.is_action_pressed(&"crouch")


## Get movement input vector
func get_movement_vector() -> Vector3:
	return _current_input_provider.get_movement_vector()


## Get rotation input vector
func get_rotation_vector() -> Vector3:
	if Input.mouse_mode != Input.MOUSE_MODE_CAPTURED:
		return Vector3.ZERO
	return _current_input_provider.get_rotation_vector()


## Returns [code]true[/code] if jump requested
func is_jumping() -> bool:
	return Input.is_action_pressed(&"jump")


## Returns [code]true[/code] if "interact" button was just pressed
func is_interaction_just_pressed() -> bool:
	return Input.is_action_just_pressed(&"interact")


## Returns [code]true[/code] if "interact" button is pressed
func is_interaction_pressed() -> bool:
	return Input.is_action_pressed(&"interact")


## Returns [code]true[/code] if "mid_interact" button was just pressed
func is_mid_interaction_just_pressed() -> bool:
	return Input.is_action_just_pressed(&"mid_interact")


## Returns [code]true[/code] if player should be in interaction rotation mode
func is_in_rotate_mode() -> bool:
	return Input.is_action_pressed(&"rotate_mode")


## Returns [closer-further] direction
func get_distance_direction() -> float:
	return _current_input_provider.get_distance_direction()


## Disables current [InputProvider] and enables [InputProvider]
## associated with [enum InputManager.Provider] value
func change_provider(p_provider: Provider) -> void:
	if _current_input_provider != null:
		_current_input_provider.disable()
	
	match p_provider:
		Provider.MOUSE_AND_KEYBOARD:
			_current_input_provider = _mk_input_provider
		Provider.GAMEPAD:
			_current_input_provider = _gamepad_input_provider
		_:
			push_error("[InputManager]: Input provider not defined -> %s" \
					% Provider.keys()[p_provider])
	
	_current_input_provider.enable()


func _set_up_providers() -> void:
	_mk_input_provider.disable()
	_gamepad_input_provider.disable()
	
	# Temporary - delete when SettingsManager is created
	change_provider(Provider.MOUSE_AND_KEYBOARD)
	#change_provider(Provider.GAMEPAD)
