class_name IOUtils
extends Node

## File I/O helper class.
## 
## Contains various util methods to handle files operations.


## Returns [code]true[/code] if file at [param path] exists, and [code]false[/code] otherwise.
static func file_exists(path: String) -> bool:
	return FileAccess.file_exists(path)


## Saves data from [param data] dictionary to cfg file's [param section] at [param path].
##
## Return [code]true[/code] if operation ended succesfully, and [code]false[/code] otherwise.
static func save_data_to_cfg_file(
	path: String, section: String, data: Dictionary[String, Variant]
) -> bool:
	print("Save \"%s\" data to cfg file at: %s" % [section, path])
	var cf := ConfigFile.new()
	
	var error := cf.load(path)
	if error:
		ErrorMsg.print_error(
				ErrorMsg.FAILED_TO_SAVE_FILE, [path, error_string(error)])
		return false
	
	for key in data:
		cf.set_value(section, key, data[key])
	
	error = cf.save(path)
	
	if error:
		ErrorMsg.print_error(
				ErrorMsg.FAILED_TO_SAVE_FILE, [path, error])
		return false
	
	return true


## Loads data from cfg file's [param section] at [param path] to [param target] dictionary.
##
## Return [code]true[/code] if operation ended succesfully, and [code]false[/code] otherwise.
static func load_data_from_cfg_file(
	path: String, section: String, target: Dictionary[String, Variant]
) -> bool:
	print("Load \"%s\" data from cfg file at: %s" % [section, path])
	var cf := ConfigFile.new()
	
	var error := cf.load(path)
	if error:
		ErrorMsg.print_error(
				ErrorMsg.FAILED_TO_LOAD_FILE, [path, error_string(error)])
		return false
	
	var result: Dictionary[String, Variant] = {}
	for key in target:
		result.set(key, cf.get_value(section, key, target[key]))
	
	target.assign(result)
	
	return true
