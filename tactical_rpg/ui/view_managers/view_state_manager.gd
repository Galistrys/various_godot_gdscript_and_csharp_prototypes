class_name ViewStateManager
extends Node


## Base class for all sorts of menues
## 
## This class provides management mechanisms for [View]s handling and changing.


## Emmited when view is changed
signal view_changed(new_view_id: String)


@export var _id: String

@export var _start_view_name: String
@export var _start_view_data: Dictionary = {}

@export var _views_container: Control


var _current_view_name: String


func initialize() -> void:
	print("[ViewStateManager] <%s>: Initialization started" % _id)
	
	for view: View in _views_container.get_children():
		view.visible = false
		view.view_put.connect(_on_view_put)
		view.process_mode = Node.PROCESS_MODE_DISABLED
		view.initialize()
		print("[ViewStateManager] <%s>: View added -> %s" % [_id, view.name])
	
	if not _start_view_name.is_empty():
		assert(_is_view_name_valid(_start_view_name))
		_show_view(_start_view_name, _start_view_data)
	
	view_changed.emit(_start_view_name)
	
	print("[ViewStateManager] <%s>: Initialization finished" % _id)


func change_to_view(view_name: String, data: Dictionary) -> void:
	_show_view(view_name, data)


func _show_view(view_name: String, data: Dictionary) -> void:
	print("[ViewStateManager] <%s>: Showing view \"%s\" with data %s" % [_id, view_name, data])
	
	if not _is_view_name_valid(view_name):
		return
	
	if not _current_view_name.is_empty():
		var current_view := _views_container.get_node(_current_view_name) as View
		current_view.visible = false
		current_view.exit_view()
		current_view.process_mode = Node.PROCESS_MODE_DISABLED
	
	var new_view := _views_container.get_node(view_name) as View
	new_view.process_mode = Node.PROCESS_MODE_INHERIT
	new_view.enter_view(data)
	new_view.visible = true
	
	_current_view_name = new_view.name
	
	view_changed.emit(view_name)


func _is_view_name_valid(view_name: String) -> bool:
	if view_name == null:
		push_error("[ViewStateManager] <%s>: View is null -> %s" % [_id, view_name])
		return false
	if view_name.is_empty():
		push_error("[ViewStateManager] <%s>: View is empty -> %s" % [_id, view_name])
		return false
	if not _views_container.has_node(view_name):
		push_error("[ViewStateManager] <%s>: View not found -> %s" % [_id, view_name])
		return false
	
	return true


func _on_view_put(view_name: String, data: Dictionary) -> void:
	_show_view(view_name, data)
