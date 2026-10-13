extends CanvasLayer

const COM_ARG_KEY = "arguments"
const COM_ARG_NAME_KEY = "name"
const COM_ARG_DESC_KEY = "desc"
const COM_ARG_REQUIRED_KEY = "required"
const COM_ARG_TYPE_KEY = "type"
const COM_DESC_KEY = "desc"
const COM_RETURN_KEY = "returns"

const LAST_COMMAND_RESULT = "$0"

const PWN_POSTFIX = " >"

const ARGS_SEPARATOR = " "

var _commands: Dictionary = {
	"cn": {
		COM_ARG_KEY: [
			{
				COM_ARG_NAME_KEY: "node_path",
				COM_ARG_DESC_KEY: "Absolute or relative path to node.",
				COM_ARG_REQUIRED_KEY: true,
				COM_ARG_TYPE_KEY: TYPE_STRING
			}
		],
		COM_DESC_KEY: "Change current node to another one."
	},
	"ls": {
		COM_ARG_KEY: [],
		COM_ARG_DESC_KEY: "Show all methods available for this node."
	},
	"clear": {
		COM_ARG_KEY: [],
		COM_DESC_KEY: "Clear console."
	},
	"help": {
		COM_ARG_KEY: [
			{
				COM_ARG_NAME_KEY: "command",
				COM_ARG_DESC_KEY: "Command you want to know more about.",
				COM_ARG_REQUIRED_KEY: false
			}
		],
		COM_DESC_KEY: "Clear console."
	},
	"fullscreen": {
		COM_ARG_KEY: [],
		COM_DESC_KEY: "Toggle fullscreen mode."
	}
}

@onready var _input_line_edit: LineEdit = $Container/MC/VBoxContainer/HBoxContainer/CommandLineEdit
@onready var _pwd: Label = $Container/MC/VBoxContainer/HBoxContainer/Label
@onready var _text_buffor: RichTextLabel = $Container/MC/VBoxContainer/ScrollContainer/TextBuffor

var _text_regex = RegEx.new()
var _numeric_regex = RegEx.new()
var _number_regex = RegEx.new()

var _last_command_result = null

var _current_node: Node

func _ready():
	_text_regex.compile("[a-zA-Z_ ]")
	_numeric_regex.compile("[0-9\\.-]")
	_number_regex.compile("[0-9]")
	_current_node = get_tree().root
	
	visible = false
	set_process_input(false)

func _get_root_node() -> Viewport:
	return get_tree().root

func _get_node_path(node: Node) -> NodePath:
	return node.get_path()

func _input(event):
	if event is InputEventKey:
		if event.pressed:
			if event.keycode == KEY_ENTER:
				_process_command()

func _process_command():
	var command: String = _input_line_edit.text.strip_edges()
	if command.is_empty():
		return
	
	_input_line_edit.clear()
	_text_buffor.add_text(command)
	
	var splitted = command.split(ARGS_SEPARATOR)
	var main = splitted[0]
	var args = null
	if splitted.size() > 1:
		args = splitted.slice(1)
	
	var force_method: bool = main[0] == "."
	if force_method:
		main = main.substr(1)
	
	if not force_method and _commands.has(main):
		_process_com(main, args)
	elif _current_node.get_method_list().has(splitted[0]):
		_process_mtd(main, args)

func _process_com(command: String, args: Array):
	var command_data: Dictionary = _commands[command]
	var com_args_data: Array = command_data[COM_ARG_KEY]
	
	pass
	

func _process_mtd(command: String, args: Array):
	pass



























