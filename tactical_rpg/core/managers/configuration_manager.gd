class_name ConfigurationManager
extends Node


## Manager for handling settings configuration file
##
## It handles settings configuration file's saving and loading


const _CFG_FILE_PATH = &"user://settings.cfg"

const _APP_SECTION = &"app"
const _VERSION_KEY = &"version"


var _cfg_file: ConfigFile


static func _file_exists(path: StringName) -> bool:
	return FileAccess.file_exists(path)


## Initializes [ConfigurationManager]
func initialize_threaded() -> void:
	print("[ConfigurationManager]: Initialization started")
	_initialize_settings_file()
	
	_load_settings()
	_save_settings()
	
	EventBus.configuration_save_requested.connect.call_deferred(_save_settings)
	EventBus.configuration_load_requested.connect.call_deferred(_load_settings)
	
	print("[ConfigurationManager]: Initialization finished")


## Sets section's data for given [param p_section_name]
func set_section_data(
	p_section_name: StringName,
	p_section_data: Dictionary[StringName, Variant]
) -> void:
	for key in p_section_data:
		_cfg_file.set_value(p_section_name, key, p_section_data[key])


## Gets section's data for given [param p_section_name]
func get_section_data(
	p_section_name: StringName,
	p_section_data: Dictionary[StringName, Variant]
) -> void:
	if not _cfg_file.has_section(p_section_name):
		print("[ConfigurationManager]: Section not present -> %s" \
				% p_section_name)
		return
	for key in p_section_data:
		if _cfg_file.has_section_key(p_section_name, key):
			p_section_data[key] = _cfg_file.get_value(p_section_name, key)
		else:
			print("[ConfigurationManager]: Section key not present -> %s - %s" \
				% [p_section_name, key])


func _initialize_settings_file() -> void:
	print("[ConfigurationManager]: Initialize settings file")
	_cfg_file = ConfigFile.new()
	if FileAccess.file_exists(_CFG_FILE_PATH):
		var err := _cfg_file.load(_CFG_FILE_PATH)
		if err != OK:
			push_error("[ConfigurationManager]: Failed to load a file at \"%s\" -> %s" \
				% [_CFG_FILE_PATH, error_string(err)])
			OS.crash("Crash")
	_cfg_file.set_value(
			_APP_SECTION,
			_VERSION_KEY,
			ProjectSettings.get_setting(&"application/config/version"))
	var error := _cfg_file.save(_CFG_FILE_PATH)
	if error:
		push_error("[ConfigurationManager]: Failed to save a file at \"%s\" -> %s" \
				% [_CFG_FILE_PATH, error_string(error)])
		get_tree().quit(1)
		OS.crash("Crash")


func _save_data_to_cfg_file() -> bool:
	print("[ConfigurationManager]: Save data to cfg file at: %s" \
			% _CFG_FILE_PATH)
	
	var error := _cfg_file.save(_CFG_FILE_PATH)
	if error:
		push_error(
				"[ConfigurationManager]: Failed to save cfg file at \"%s\" -> %s" \
				% [_CFG_FILE_PATH, error_string(error)])
		return false
	
	return true


func _load_data_from_cfg_file() -> bool:
	print("[ConfigurationManager]: Load data from cfg file at: %s" \
			% _CFG_FILE_PATH)
	
	var error := _cfg_file.load(_CFG_FILE_PATH)
	if error:
		push_error(
				"[ConfigurationManager]: Failed to load cfg file at \"%s\" -> %s" \
				% [_CFG_FILE_PATH, error_string(error)])
		return false
	
	return true


func _save_settings() -> void:
	if not _save_data_to_cfg_file():
		get_tree().quit(1)


func _load_settings() -> void:
	if not _load_data_from_cfg_file():
		get_tree().quit(1)
