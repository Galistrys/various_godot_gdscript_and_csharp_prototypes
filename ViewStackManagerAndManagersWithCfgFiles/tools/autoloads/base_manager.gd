class_name BaseManager
extends Node


var _DATA_OBJECT: Dictionary[String, Variant] = {}


func _init() -> void:
	_load_settings()
	_save_settings()
	
	AppManager.settings_apply_requested.connect(_apply_and_save)
	AppManager.settings_reload_requested.connect(_reload_settings)


func _apply_settings() -> void:
	OS.alert("The \"apply_settings\" method is not implemented. Fix it!",
			"METHOD NOT IMPLEMENTED")
	assert(false)


func _apply_and_save() -> void:
	_apply_settings()
	_save_settings()


func _reload_settings() -> void:
	_load_settings()
	_apply_settings()


func _get_section() -> String:
	OS.alert("The \"_get_section\" method is not implemented. Fix it!",
			"METHOD NOT IMPLEMENTED")
	assert(false)
	return ""


func _save_settings() -> void:
	if not IOUtils.save_data_to_cfg_file(
			AppManager.SETTINGS_CFG_PATH, _get_section(), _DATA_OBJECT):
		get_tree().quit(1)


func _load_settings() -> void:
	if not IOUtils.load_data_from_cfg_file(
			AppManager.SETTINGS_CFG_PATH, _get_section(), _DATA_OBJECT):
		get_tree().quit(1)
