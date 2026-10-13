class_name LoadingScreen
extends CanvasLayer


signal fade_in_finished
signal fade_out_finished
signal show_button_finished
signal continue_button_pressed


const _ANIM_SPEED_MOD = 1.0 / 3.0

const _FADE_IN_ANIM = &"fade_in"
const _FADE_OUT_ANIM = &"fade_out"

const _FADE_IN_CONT_ANIM = &"fade_in_continue"
const _FADE_OUT_CONT_ANIM = &"fade_out_continue"
const _SHOW_BUTTON = &"show_button"


var _with_continue := false


@onready var _animation_player := $AnimationPlayer as AnimationPlayer
@onready var _box_container := $GlobalContainer/ContentContainer/BoxContainer as HBoxContainer
@warning_ignore("unused_private_class_variable")
@onready var _tips_label := $GlobalContainer/ContentContainer/BoxContainer/TipsLabel as Label


func initialize_threaded() -> void:
	print("[LoadingScreen]: Initialization started")
	
	auto_translate_mode = Node.AUTO_TRANSLATE_MODE_DISABLED
	auto_translate_mode = Node.AUTO_TRANSLATE_MODE_ALWAYS
	
	print("[LoadingScreen]: Initialization finished")


func play_fade_in(p_with_continue: bool = false) -> void:
	_with_continue = p_with_continue
	
	if _with_continue:
		_box_container.visible = true
		_animation_player.play(_FADE_IN_CONT_ANIM, -1, _ANIM_SPEED_MOD)
	else:
		_box_container.visible = false
		_animation_player.play(_FADE_IN_ANIM, -1, _ANIM_SPEED_MOD)
	
	await _animation_player.animation_finished
	fade_in_finished.emit()


func play_fade_out() -> void:
	if _with_continue:
		_animation_player.play(_FADE_OUT_CONT_ANIM, -1, _ANIM_SPEED_MOD)
	else:
		_animation_player.play(_FADE_OUT_ANIM, -1, _ANIM_SPEED_MOD)
	
	await _animation_player.animation_finished
	
	fade_out_finished.emit()


func play_show_button() -> void:
	if _with_continue:
		_animation_player.play(_SHOW_BUTTON, -1, _ANIM_SPEED_MOD)
		
		await _animation_player.animation_finished
		show_button_finished.emit()


func _on_standard_text_button_pressed() -> void:
	continue_button_pressed.emit()
