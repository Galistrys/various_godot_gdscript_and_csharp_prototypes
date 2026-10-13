@abstract
class_name PlayerState
extends State


## Abstract class for every [Player]'s state


var _player: Player


func _init(player: Player) -> void:
	_player = player
