class_name ErrorMsg
extends Object


## Parameters: ["class_name"]
const INITIALIZING_SINGLETON_CLASS = "Trying to initialize singleton class: %s"

## Parameters: ["regex_expression", Error]
const REGEX_COMPILATION_FAILED = "RegEx compilation failed: \"%s\". Error: %s"

## Parameters: ["path_to_directory", Error]
const OPEN_DIRECTORY_FAILED = "Failed to open directory at: \"%s\". Error: %s"

## Parameters: ["path_to_directory", Error]
const LIST_FILES_FAILED = "Failed to list files at: \"%s\". Error: %s"

## Parameters: ["path_to_resource", Error]
const RESOURCE_LOAD_FAILED = "Failed to load a resource at: \"%s\". Error: %s"

## Parameters: ["location"]
const NOT_IMPLEMENTED_YET = "Not implemented yet: %s"

## Parameters: none
const VIEW_IS_NULL = "View name is [null]."

## Parameters: none
const VIEW_IS_EMPTY = "View name is empty."

## Parameters: ["view_name"]
const VIEW_NOT_FOUND = "View with name [%s] not found."

## Parameters: none
const VIEW_TRIED_TO_REMOVE_LAST_VIEW = "Tried to remove last view from the stack."

## Parameters: ["path_to_file", Error]
const FAILED_TO_SAVE_FILE = "Failed to save the file at: \"%s\". Error: %s"

## Parameters: ["path_to_file", Error]
const FAILED_TO_LOAD_FILE = "Failed to load the file at: \"%s\". Error: %s"

## Parameters: ["audio_bus_name"]
const AUDIO_BUS_NOT_FOUND = "Failed to find an audio bus with name: \"%s\""


func _init() -> void:
	@warning_ignore("assert_always_true")
	assert(true, INITIALIZING_SINGLETON_CLASS % ["ErrorMsg"])


static func print_error(msg: String, args: Array = []) -> void:
	printerr(msg % args)
	push_error(msg % args)
