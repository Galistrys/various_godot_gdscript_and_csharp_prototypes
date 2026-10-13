class_name Palette
extends Object


const MAIN_COLOR = Color("#ff9900")
const DARKER_COLOR = Color("#cf6300")
const LIGHTER_COLOR = Color("#ffb888")
const DIFF_COLOR = Color("#00326f")
const DARK_DIFF_COLOR = Color("#33004d")


func _init() -> void:
	@warning_ignore("assert_always_true")
	assert(true, ErrorMsg.INITIALIZING_SINGLETON_CLASS % ["Palette"])
