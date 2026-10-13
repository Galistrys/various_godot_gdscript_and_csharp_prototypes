extends View


@onready var _conf_dialog: ConfirmationDialog = $ConfirmationDialog

@onready var _sp_game_button: Button = $VBoxContainer/HBoxContainer/Buttons/SpGameButton



func _ready() -> void:
	_conf_dialog.get_label().horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	_conf_dialog.get_ok_button().theme_type_variation = "DialogButton"
	_conf_dialog.get_cancel_button().theme_type_variation = "DialogButton"


@warning_ignore("unused_parameter")
func enter_view(data: Dictionary) -> void:
	_sp_game_button.grab_focus()


@warning_ignore("unused_parameter")
func resume_view(data: Dictionary) -> void:
	_sp_game_button.grab_focus()


func _on_sp_game_button_pressed() -> void:
	pass # Replace with function body.


func _on_mp_game_button_pressed() -> void:
	pass # Replace with function body.


func _on_records_button_pressed() -> void:
	pass # Replace with function body.


func _on_settings_button_pressed() -> void:
	pass # Replace with function body.


func _on_about_button_pressed() -> void:
	pass # Replace with function body.


func _on_exit_button_pressed() -> void:
	_conf_dialog.popup_centered()


func _on_confirmation_dialog_confirmed() -> void:
	get_tree().quit(0)
