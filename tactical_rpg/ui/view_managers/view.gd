class_name View
extends Control

## Base class for all sorts of menu [View]s
## 
## A view must inherit from this class to be properly handeled by [ViewStackManager]

## Emitted when [View] is exiting
@warning_ignore("unused_signal")
signal view_pop(data: Dictionary)
## Emitted when view is requesting another view to be shown
@warning_ignore("unused_signal")
signal view_put(view_name: String, data: Dictionary)


func initialize() -> void:
	pass


## Called when [ViewStackManager] is entering the view
@warning_ignore("unused_parameter")
func enter_view(data: Dictionary) -> void:
	pass


## Called when [ViewStackManager] is resuming the view
@warning_ignore("unused_parameter")
func resume_view(data: Dictionary) -> void:
	pass


## Called when [ViewStackManager] is pausing the view
func pause_view() -> void:
	pass


## Called when [ViewStackManager] is exiting the view
func exit_view() -> void:
	pass
