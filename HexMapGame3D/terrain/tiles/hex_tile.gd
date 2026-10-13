class_name HexTile
extends RefCounted


const HIGHLIGHT_OVERLAY_MAT = preload("res://terrain/hex_highlight_overlay_material.tres")

var _scenario: RID
var _world_pos: Vector3

var highlighted := false


func _init(scenario: RID, world_pos: Vector3):
	_scenario = scenario
	_world_pos = world_pos


func show_tile() -> void:
	pass


func update_tile(_radius: int, _data: Dictionary) -> void:
	pass


func hide_tile() -> void:
	pass


func set_scale(_scale: float) -> void:
	printerr("set_scale function not implemented.")


func highlight() -> void:
	printerr("highlight function not implemented.")


func dehighlight() -> void:
	printerr("dehighlight function not implemented.")
