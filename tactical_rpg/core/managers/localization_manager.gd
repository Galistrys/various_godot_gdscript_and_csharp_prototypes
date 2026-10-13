class_name LocalizationManager
extends Node


## An autoload to handle localizations loading
## 
## This class handles loading translations files.
## In the future it will also handle localization changes.


## Emitted whenever localization is changed
signal localization_changed(lang: String)


const _CFG_SECTION = &"localization"
const _LANGUAGE_KEY = &"language"

const _TRANSLATIONS_DIR = &"res://ui/localization/"
const _TRANSLATION_FILE_REGEX = "^.*\\.translation$"

var _data_object: Dictionary[StringName, Variant] = {
	_LANGUAGE_KEY: OS.get_locale_language()
}

var _supported_languages: PackedStringArray


## Initializes [LocalizationManager]
func initialize_threaded() -> void:
	print("[LocalizationManager]: Initialization started")
	
	print("[LocalizationManager]: Loading translation files...")
	var translation_files := _get_all_translation_files()
	for tr_file in translation_files:
		print("[LocalizationManager]: Adding translation file: %s" % tr_file)
		TranslationServer.add_translation(load(tr_file))
	print("[LocalizationManager]: Loading translation files finished")
	
	var source_array: Array[String] = Array(
			Array(TranslationServer.get_loaded_locales()),
			TYPE_STRING, &"", null)
	var result_array: Array[String] = []
	
	for locale in source_array:
		if result_array.find(locale) == -1:
			result_array.append(locale)
	
	_supported_languages = PackedStringArray(result_array)
	
	print("[LocalizationManager]: Initialization finished")


## Get list of supported languages
func get_supported_languages() -> PackedStringArray:
	return _supported_languages


## Change current localization
func change_localization(language: StringName) -> void:
	if language not in _supported_languages:
		var local_language := OS.get_locale_language()
		print("[LocalizationManager]: Language \"%s\" not found. Fallback to locale language \"%s\"..." \
				% [language, local_language])
		if local_language in _supported_languages:
			language = local_language
		else:
			print("[LocalizationManager]: Language \"%s\" not found. Fallback to \"en\"" % language)
			language = "en"
	
	_data_object[_LANGUAGE_KEY] = language
	TranslationServer.set_locale(language)
	localization_changed.emit(language)


func _apply_settings() -> void:
	change_localization(_data_object[_LANGUAGE_KEY])


func _get_all_translation_files() -> PackedStringArray:
	var regex := RegEx.new()
	var error := regex.compile(_TRANSLATION_FILE_REGEX)
	if error:
		push_error("[LocalizationManager]: Regex compilation failed -> %s" \
				% error_string(error))
		return PackedStringArray()
	
	var dir := DirAccess.open(_TRANSLATIONS_DIR)
	error = DirAccess.get_open_error()
	if error:
		push_error("[LocalizationManager]: Open directory failed at \"%s\" -> %s" \
				% [_TRANSLATIONS_DIR, error_string(error)])
		return PackedStringArray()
	
	error = dir.list_dir_begin()
	if error:
		push_error("[LocalizationManager]: List files failed at \"%s\" -> %s" \
				% [_TRANSLATIONS_DIR, error_string(error)])
		return PackedStringArray()
	
	var files: Array[String] = []
	var file_name := dir.get_next()
	while file_name != "":
		if not dir.current_is_dir():
			var file_path := dir.get_current_dir().path_join(file_name)
			if regex.search(file_path):
				files.append(file_path)
		file_name = dir.get_next()
	dir.list_dir_end()
	
	return PackedStringArray(files)
