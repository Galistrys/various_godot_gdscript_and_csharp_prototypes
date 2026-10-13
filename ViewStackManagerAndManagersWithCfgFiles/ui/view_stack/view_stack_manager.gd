class_name ViewStackManager
extends Node


## Base class for all sorts of menues
## 
## This class provides management mechanisms for [View]s handling and changing.


@export var _start_view_name: String
@export var _start_view_data: Dictionary = {}

@export var _views_container: Control


var _view_stack: Array[String] = []


func _init() -> void:
	pass 


func _ready() -> void:
	for view: View in _views_container.get_children():
		view.visible = false
		view.view_pop.connect(_on_view_pop)
		view.view_put.connect(_on_view_put)
		view.process_mode = Node.PROCESS_MODE_DISABLED
	
	assert(_is_view_name_valid(_start_view_name))
	
	print("Showing start view \"%s\" with data \"%s\"" % \
			[_start_view_name, _start_view_data])
	
	_view_stack.push_back(_start_view_name)
	var start_view := _views_container.get_node(_start_view_name) as View
	start_view.process_mode = Node.PROCESS_MODE_INHERIT
	start_view.enter_view(_start_view_data)
	start_view.visible = true


func _is_view_name_valid(view_name: String) -> bool:
	if view_name == null:
		ErrorMsg.print_error(ErrorMsg.VIEW_IS_NULL)
		return false
	if view_name.is_empty():
		ErrorMsg.print_error(ErrorMsg.VIEW_IS_EMPTY)
		return false
	if not _views_container.has_node(view_name):
		ErrorMsg.print_error(ErrorMsg.VIEW_NOT_FOUND, [view_name])
		return false
	
	return true


func _show_view(view_name: String, data: Dictionary) -> void:
	print("Showing view \"%s\" with data %s" % [view_name, data])
	
	if not _is_view_name_valid(view_name):
		return
	
	var current_view := _views_container.get_node(_view_stack.back()) as View
	var new_view := _views_container.get_node(view_name) as View
	
	#assert(current_view != new_view)
	
	_view_stack.push_back(view_name)
	
	current_view.visible = false
	current_view.pause_view()
	current_view.process_mode = Node.PROCESS_MODE_DISABLED
	new_view.process_mode = Node.PROCESS_MODE_INHERIT
	new_view.enter_view(data)
	new_view.visible = true


func _hide_view(data: Dictionary) -> void:
	if _view_stack.size() < 2:
		ErrorMsg.print_error(ErrorMsg.VIEW_TRIED_TO_REMOVE_LAST_VIEW)
		return
	
	var current_view_name: String = _view_stack.pop_back()
	var new_view_name: String = _view_stack.back()
	print("Hiding view \"%s\" with data \"%s\". Return to \"%s\"" % \
			[current_view_name, data, new_view_name])
	
	var current_view: View = _views_container.get_node(
			current_view_name) as View
	var old_view: View = _views_container.get_node(
			new_view_name) as View
	
	current_view.visible = false
	current_view.exit_view()
	current_view.process_mode = Node.PROCESS_MODE_DISABLED
	old_view.process_mode = Node.PROCESS_MODE_INHERIT
	old_view.resume_view(data)
	old_view.visible = true


func _on_view_put(view_name: String, data: Dictionary) -> void:
	_show_view(view_name, data)


func _on_view_pop(data: Dictionary) -> void:
	_hide_view(data)
