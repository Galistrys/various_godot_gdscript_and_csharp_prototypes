class_name GuiManager
extends Control


## Manages GUI for the game


const UI_VIEWS_UID = &"uid://b0sw4t4i6i28q"


var _ui_views: UiViews


@onready var _loading_screen := $LoadingScreen as LoadingScreen


func initialize_threaded() -> void:
	print("[GuiManager]: Initialization started")
	
	_loading_screen.initialize_threaded()
	
	print("[GuiManager]: Initialization finished")


func first_main_menu_load() -> void:
	_load_ui_views()
	
	_ui_views.initialize_all()
	
	await _loading_screen.play_fade_out()


func get_loading_screen() -> LoadingScreen:
	return _loading_screen


func _load_ui_views() -> void:
	var ui_view_packed_scene := ResourceLoader.load(
			UI_VIEWS_UID, "PackedScene") as PackedScene
	_ui_views = ui_view_packed_scene.instantiate() as UiViews
	add_child(_ui_views)
