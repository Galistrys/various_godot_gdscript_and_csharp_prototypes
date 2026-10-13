class_name CursorManager
extends Node


## Manages mouse cursor textures
##
## It loads cursors textures and generates size versions of them on
## window's size change. Allows for runtime changing of the cursor.


## Game Cursor Shape enum
enum GameCursorShape {
	DEFAULT, ## Default Cursor Shape
	INTERACT, ## Interactable objects shape
	MELEE_ATTACK, ## Melee attack shape
	RANGED_ATTACK, ## Range attack shape
	SPELL_ACTION, ## Spell action shape
	ENTER, ## Enter new region shape
	DIALOG, ## Start dialog shape
	FORBIDDEN, ## Forbidden shape
	HELP, ## Help shape
	ROTATE, ## Rotate camera shape
	WAIT ## Wait shape
}


const _TEXTURE_SIZE = 16

const _IMAGE_KEY = &"image"
const _HOTSPOT_KEY = &"hotspot"


const _GAME_CURSORS_DATA: Dictionary[GameCursorShape, Dictionary] = {
	GameCursorShape.INTERACT: {
		_IMAGE_KEY: preload("uid://cqw0vx232xqq8"),
		_HOTSPOT_KEY: Vector2i(2, 2)
	},
	GameCursorShape.MELEE_ATTACK: {
		_IMAGE_KEY: preload("uid://bc7j3tgywgwfe"),
		_HOTSPOT_KEY: Vector2i(2, 2)
	},
	GameCursorShape.RANGED_ATTACK: {
		_IMAGE_KEY: preload("uid://bv7s5yhh81b0t"),
		_HOTSPOT_KEY: Vector2i(2, 2)
	},
	GameCursorShape.SPELL_ACTION: {
		_IMAGE_KEY: preload("uid://66ud3c66ircd"),
		_HOTSPOT_KEY: Vector2i(7, 7)
	},
	GameCursorShape.ENTER: {
		_IMAGE_KEY: preload("uid://dxuoub6784ekm"),
		_HOTSPOT_KEY: Vector2i(7, 7)
	},
	GameCursorShape.DIALOG: {
		_IMAGE_KEY: preload("uid://nf2qvxgyo5xb"),
		_HOTSPOT_KEY: Vector2i(7, 7)
	},
	GameCursorShape.FORBIDDEN: {
		_IMAGE_KEY: preload("uid://cnuxoeopnmtlq"),
		_HOTSPOT_KEY: Vector2i(7, 7)
	},
	GameCursorShape.HELP: {
		_IMAGE_KEY: preload("uid://dr06c5g5qy374"),
		_HOTSPOT_KEY: Vector2i(7, 7)
	},
	GameCursorShape.ROTATE: {
		_IMAGE_KEY: preload("uid://wa140ekkkkcj"),
		_HOTSPOT_KEY: Vector2i(7, 7)
	},
	GameCursorShape.WAIT: {
		_IMAGE_KEY: preload("uid://dflh1qu5j4ert"),
		_HOTSPOT_KEY: Vector2i(7, 7)
	},
}

const _SYSTEM_CURSORS_DATA: Dictionary[Input.CursorShape, Dictionary] = {
	Input.CURSOR_ARROW: {
		_IMAGE_KEY: preload("uid://cbs443hhgs54x"),
		_HOTSPOT_KEY: Vector2i(2, 2)
	},
	Input.CURSOR_IBEAM: {
		_IMAGE_KEY: preload("uid://t3oqs1fabds3"),
		_HOTSPOT_KEY: Vector2i(7, 7)
	},
	Input.CURSOR_POINTING_HAND: {
		_IMAGE_KEY: preload("uid://cqw0vx232xqq8"),
		_HOTSPOT_KEY: Vector2i(2, 2)
	},
	Input.CURSOR_CROSS: {
		_IMAGE_KEY: preload("uid://c3ys6spwk7y4d"),
		_HOTSPOT_KEY: Vector2i(2, 2)
	},
	Input.CURSOR_WAIT: {
		_IMAGE_KEY: preload("uid://dflh1qu5j4ert"),
		_HOTSPOT_KEY: Vector2(7, 7)
	},
	Input.CURSOR_BUSY: {
		_IMAGE_KEY: preload("uid://dnvdyqpjhwey5"),
		_HOTSPOT_KEY: Vector2(7, 7)
	},
	Input.CURSOR_DRAG: {
		_IMAGE_KEY: preload("uid://bigu7e1xggk7q"),
		_HOTSPOT_KEY: Vector2(2, 2)
	},
	Input.CURSOR_CAN_DROP: {
		_IMAGE_KEY: preload("uid://di2afbsl3dn04"),
		_HOTSPOT_KEY: Vector2(2, 2)
	},
	Input.CURSOR_FORBIDDEN: {
		_IMAGE_KEY: preload("uid://cnuxoeopnmtlq"),
		_HOTSPOT_KEY: Vector2(2, 2)
	},
	Input.CURSOR_VSIZE: {
		_IMAGE_KEY: preload("uid://c2bmftycc4q3t"),
		_HOTSPOT_KEY: Vector2(7, 7)
	},
	Input.CURSOR_HSIZE: {
		_IMAGE_KEY: preload("uid://bxac17idtxf4q"),
		_HOTSPOT_KEY: Vector2(7, 7)
	},
	Input.CURSOR_BDIAGSIZE: {
		_IMAGE_KEY: preload("uid://0hib66by1to1"),
		_HOTSPOT_KEY: Vector2(7, 7)
	},
	Input.CURSOR_FDIAGSIZE: {
		_IMAGE_KEY: preload("uid://o780obogikxr"),
		_HOTSPOT_KEY: Vector2(7, 7)
	},
	Input.CURSOR_HELP: {
		_IMAGE_KEY: preload("uid://dr06c5g5qy374"),
		_HOTSPOT_KEY: Vector2(7, 7)
	}
}


var _current_scale := 0

var _current_game_cursor_images: Dictionary[GameCursorShape, Image] = {}
var _current_system_cursor_images: Dictionary[Input.CursorShape, Image] = {}

var _current_default_shape := GameCursorShape.DEFAULT


func _ready() -> void:
	pass


## Initializes [CursorManagement]
func initialize_threaded() -> void:
	print("[CursorManager]: Initialization started")
	
	get_window().size_changed.connect(_on_viewport_size_changed)
	_on_viewport_size_changed.call_deferred()
	
	print("[CursorManager]: Initialization finished")


## Sets default cursor to a cursor shape [param p_cursor_shape]
func set_cursor_shape(
	p_cursor_shape: GameCursorShape = GameCursorShape.DEFAULT
) -> void:
	if p_cursor_shape == _current_default_shape:
		return
	
	print("[CursorManager]: Cursor changed to %s" % GameCursorShape.keys()[p_cursor_shape])
	_current_default_shape = p_cursor_shape
	if p_cursor_shape == GameCursorShape.DEFAULT:
		_set_system_cursor(
			Input.CURSOR_ARROW,
			_current_system_cursor_images[Input.CURSOR_ARROW],
			_SYSTEM_CURSORS_DATA[Input.CURSOR_ARROW][_HOTSPOT_KEY] * _current_scale)
		return
	
	_set_system_cursor(
		Input.CURSOR_ARROW,
		_current_game_cursor_images[p_cursor_shape],
		_GAME_CURSORS_DATA[p_cursor_shape][_HOTSPOT_KEY] * _current_scale
	)


func _on_viewport_size_changed() -> void:
	print("[CursorManager]: Viewport changed size")
	var window := get_window()
	var window_size := window.size
	var content_size := window.content_scale_size
	
	var scale := roundi(float(window_size.y) / float(content_size.y) * 0.75)
	scale = clampi(scale, 1, 4)
	if _current_scale == scale:
		print("[CursorManager]: Scale unchanged")
		return
	
	print("[CursorManager]: New cursor scale, regenerating images -> %d" % scale)
	
	_reload_game_cursor_textures(scale)
	_reload_system_cursor_textures(scale)
	
	if _current_default_shape != GameCursorShape.DEFAULT:
		_current_default_shape = GameCursorShape.DEFAULT
		set_cursor_shape(_current_default_shape)


func _reload_game_cursor_textures(p_scale: int) -> void:
	_current_scale = p_scale
	
	var cursor_images: Dictionary[GameCursorShape, Image] = {}
	
	for cursor_shape in _GAME_CURSORS_DATA:
		var cursor_data: Dictionary = _GAME_CURSORS_DATA[cursor_shape]
		var cursor_image: Image = cursor_data[_IMAGE_KEY]
		
		var new_cursor_image := _create_image(cursor_image, p_scale)
		
		cursor_images[cursor_shape] = new_cursor_image

	_current_game_cursor_images = cursor_images


func _reload_system_cursor_textures(p_scale: int) -> void:
	_current_scale = p_scale
	
	var cursor_images: Dictionary[Input.CursorShape, Image] = {}
	
	for cursor_shape in _SYSTEM_CURSORS_DATA:
		var cursor_data: Dictionary = _SYSTEM_CURSORS_DATA[cursor_shape]
		var cursor_image: Image = cursor_data[_IMAGE_KEY]
		var cursor_hotspot: Vector2i = cursor_data[_HOTSPOT_KEY]
		
		var new_cursor_hotspot := cursor_hotspot * p_scale
		var new_cursor_image := _create_image(cursor_image, p_scale)
		
		cursor_images[cursor_shape] = new_cursor_image
		_set_system_cursor(cursor_shape, new_cursor_image, new_cursor_hotspot)
		
		if cursor_shape == Input.CURSOR_DRAG:
			_set_system_cursor(Input.CURSOR_MOVE, new_cursor_image, new_cursor_hotspot)
		elif cursor_shape == Input.CURSOR_VSIZE:
			_set_system_cursor(Input.CURSOR_VSPLIT, new_cursor_image, new_cursor_hotspot)
		elif cursor_shape == Input.CURSOR_HSIZE:
			_set_system_cursor(Input.CURSOR_HSPLIT, new_cursor_image, new_cursor_hotspot)
	
	_current_system_cursor_images = cursor_images


func _create_image(p_image: Image, p_scale: int) -> Image:
	var new_image := Image.new()
	new_image.copy_from(p_image)
	
	new_image.resize(
			_TEXTURE_SIZE * p_scale,
			_TEXTURE_SIZE * p_scale,
			Image.INTERPOLATE_NEAREST)
	
	return new_image


func _set_system_cursor(
	p_cursor_shape: Input.CursorShape,
	p_image: Image,
	p_hotspot: Vector2i
) -> void:
	Input.set_custom_mouse_cursor(p_image, p_cursor_shape, p_hotspot)
