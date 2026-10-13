class_name UiViews
extends Control


@onready var main_view_state_manager := $MainViewStateManager as ViewStateManager


func initialize_all() -> void:
	main_view_state_manager.initialize()
