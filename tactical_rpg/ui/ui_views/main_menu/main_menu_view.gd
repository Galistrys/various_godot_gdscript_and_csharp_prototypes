class_name MainMenuView
extends View


## Main Menu view


@onready var _view_stack_manager := $ViewStackManager as ViewStackManager
@onready var _version_label := $VersionLabel as Label


func initialize() -> void:
	print("[MainMenuView]: Initialization started")
	
	_view_stack_manager.initialize()
	
	_version_label.text = "Version: %s" % ProjectSettings.get_setting(&"application/config/version")
	
	print("[MainMenuView]: Initialization finished")
