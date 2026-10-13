extends Node


signal settings_apply_requested
signal settings_reload_requested


const SETTINGS_CFG_PATH = "user://settings.cfg"
const GAME_VERSION = "PreAlpha"


const _CFG_SECTION = "app"
const _VERSION_KEY = "version"


func _init() -> void:
	_init_settings_file()


func _ready() -> void:
	apply_settings()


func apply_settings() -> void:
	settings_apply_requested.emit()


func reload_settings() -> void:
	settings_reload_requested.emit()


func _init_settings_file() -> void:
	var cfg := ConfigFile.new()
	if FileAccess.file_exists(SETTINGS_CFG_PATH):
		var err := cfg.load(SETTINGS_CFG_PATH)
		if err != OK:
			ErrorMsg.print_error(
				ErrorMsg.FAILED_TO_LOAD_FILE, [SETTINGS_CFG_PATH, error_string(err)])
			OS.crash("Crash")
	cfg.set_value(_CFG_SECTION, _VERSION_KEY, GAME_VERSION)
	var error := cfg.save(SETTINGS_CFG_PATH)
	if error:
		ErrorMsg.print_error(
			ErrorMsg.FAILED_TO_SAVE_FILE, [SETTINGS_CFG_PATH, error_string(error)])
		get_tree().quit(1)
		OS.crash("Crash")
