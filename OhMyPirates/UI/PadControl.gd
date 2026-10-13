extends Control

const LENGTH = 48.0

export(NodePath) var player_ship_path
onready var _player_ship: ShipController = get_node_or_null(player_ship_path)

onready var _padButton = $Pad
onready var _pointerTR = $Pointer

var _pointerOffset: Vector2 = Vector2.ZERO
var _centerOffset: Vector2 = Vector2.ZERO

var _currentDirection: Vector2 = Vector2.DOWN


func get_current_direction() -> Vector2:
	return _currentDirection


func _ready():
	_pointerOffset = - _pointerTR.rect_size / 2.0
	_centerOffset = - rect_size / 2.0
	
	_pointerTR.rect_position = _pointerOffset + _currentDirection * LENGTH - _centerOffset
	
	set_process(false)
	
	if _player_ship == null:
		set_physics_process(false)


func _process(_delta):
	var mouse_point: Vector2 = get_local_mouse_position() + _centerOffset
	_currentDirection = Vector2.ZERO.direction_to(mouse_point)
	
	_pointerTR.rect_position = _pointerOffset + _currentDirection * LENGTH - _centerOffset

func _physics_process(_delta):
	_player_ship.set_target_direction(_currentDirection)

func _on_Pad_pressed():
	set_process(true)


func _on_Pad_released():
	set_process(false)
