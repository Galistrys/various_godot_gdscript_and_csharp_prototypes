class_name GameManager
extends Node


## Entry point for the Game


signal _initialization_thread_finished


@onready var _world_manager := $WorldManager as WorldManager
@onready var _gui_manager := $GuiManager as GuiManager


func _ready() -> void:
	await ready
	await _initialize_async()


func _initialize_async() -> void:
	print("[GameManager]: Initialization started")
	
	var _thread := Thread.new()
	var error := _thread.start(_initialization_thread)
	if error != OK:
		push_error("[GameManager]: Failed to start initialization thread")
		OS.crash("Failed to start initialization thread")
	
	var loading_screen := _gui_manager.get_loading_screen()
	await loading_screen.play_fade_in(false)
	
	while _thread.is_alive():
		await get_tree().physics_frame
	_thread.wait_to_finish()
	
	_gui_manager.first_main_menu_load.call_deferred()
	
	print("[GameManager]: Initialization finished")
	
	await _world_manager.change_level_async("prototype_level")


func _initialization_thread() -> void:
	ConfigurationService.initialize_threaded()
	SettingsService.initialize_threaded()
	LocalizationService.initialize_threaded()
	CursorService.initialize_threaded()
	InputService.initialize_threaded()
	NavigationService.initialize_threaded()
	_world_manager.initialize_threaded()
	_gui_manager.initialize_threaded()
	
	_initialization_thread_finished.emit.call_deferred()
