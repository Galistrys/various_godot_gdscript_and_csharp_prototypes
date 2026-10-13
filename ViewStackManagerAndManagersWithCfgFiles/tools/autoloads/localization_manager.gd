extends BaseManager


## An autoload to handle localizations loading
## 
## This class handles loading translations files.
## In the future it will also handle localization changes.


## Emitted whenever localization is changed
signal localization_changed(lang: String)


const _CFG_SECTION = "localization"
const _LANGUAGE_KEY = "language"

const _TRANSLATIONS_DIR = "res://localization/"
const _TRANSLATION_FILE_REGEX = "^.*\\.translation$"

var _DEFAULT_DATA_OBJECT: Dictionary[String, Variant] = {
	_LANGUAGE_KEY: OS.get_locale_language()
}

var _supported_languages: PackedStringArray


func _init() -> void:
	_DATA_OBJECT.assign(_DEFAULT_DATA_OBJECT)
	
	print("Loading translation files...")
	var translation_files := _get_all_translation_files()
	for tr_file in translation_files:
		print("Adding translation file: ", tr_file)
		TranslationServer.add_translation(load(tr_file))
	print("Loading translation files finished")
	
	var source_array: Array[String] = Array(
			Array(TranslationServer.get_loaded_locales()),
			TYPE_STRING, &"", null)
	var result_array: Array[String] = []
	
	for locale in source_array:
		if result_array.find(locale) == -1:
			result_array.append(locale)
	
	_supported_languages = PackedStringArray(result_array)
	
	super._init()


## Get list of supported languages
func get_supported_languages() -> PackedStringArray:
	return _supported_languages


## Change current localization
func change_localization(language: String) -> void:
	if language not in _supported_languages:
		var local_language := OS.get_locale_language()
		print("Language \"%s\" not found. Fallback to locale language \"%s\"..." % [language, local_language])
		if local_language in _supported_languages:
			language = local_language
		else:
			print("Language \"%s\" not found. Fallback to \"en\"" % [language])
			language = "en"
	
	_DATA_OBJECT[_LANGUAGE_KEY] = language
	TranslationServer.set_locale(language)
	localization_changed.emit(language)


func _apply_settings() -> void:
	change_localization(_DATA_OBJECT[_LANGUAGE_KEY])


func _get_section() -> String:
	return _CFG_SECTION


func _get_all_translation_files() -> PackedStringArray:
	var regex := RegEx.new()
	var error := regex.compile(_TRANSLATION_FILE_REGEX)
	if error:
		ErrorMsg.print_error(
				ErrorMsg.REGEX_COMPILATION_FAILED,
				[error])
		return PackedStringArray()
	
	var dir = DirAccess.open(_TRANSLATIONS_DIR)
	error = DirAccess.get_open_error()
	if error:
		ErrorMsg.print_error(
				ErrorMsg.OPEN_DIRECTORY_FAILED,
				[_TRANSLATIONS_DIR, error])
		return PackedStringArray()
	
	error = dir.list_dir_begin()
	if error:
		ErrorMsg.print_error(
				ErrorMsg.LIST_FILES_FAILED,
				[_TRANSLATIONS_DIR, error])
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
