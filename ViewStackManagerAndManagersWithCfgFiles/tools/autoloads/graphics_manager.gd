extends BaseManager

## An autoload to handle graphics related options
## 
## This class handles changing graphics related options.


enum AntiAliasingMode {
	NONE,
	FXAA,
	MSAAx2,
	MSAAx4,
	MSAAx8
}


const SUPERSAMPLING_SCALE_MIN = 0.5
const SUPERSAMPLING_SCALE_MAX = 2.0


## Emitted whenever antialiasting mode is changed
signal antialiasing_mode_changed(value: AntiAliasingMode)
## Emitted whenever supersampling value is changed
signal supersampling_value_changed(value: float)


const _CFG_SECTION = "graphics"

const _AA_KEY = "antialiasing"
const _SS_KEY = "supersampling"

var _DEFAULT_DATA_OBJECT: Dictionary[String, Variant] = {
	_AA_KEY: AntiAliasingMode.MSAAx2,
	_SS_KEY: 1.0
}


func _init() -> void:
	_DATA_OBJECT.assign(_DEFAULT_DATA_OBJECT)
	
	super._init()


func set_antialiasing_mode(mode: AntiAliasingMode) -> void:
	_DATA_OBJECT[_AA_KEY] = mode
	
	match mode:
		AntiAliasingMode.NONE:
			get_window().screen_space_aa = Viewport.SCREEN_SPACE_AA_DISABLED
			get_window().msaa_3d = Viewport.MSAA_DISABLED
		AntiAliasingMode.FXAA:
			get_window().screen_space_aa = Viewport.SCREEN_SPACE_AA_FXAA
			get_window().msaa_3d = Viewport.MSAA_DISABLED
		AntiAliasingMode.MSAAx2:
			get_window().screen_space_aa = Viewport.SCREEN_SPACE_AA_DISABLED
			get_window().msaa_3d = Viewport.MSAA_2X
		AntiAliasingMode.MSAAx4:
			get_window().screen_space_aa = Viewport.SCREEN_SPACE_AA_DISABLED
			get_window().msaa_3d = Viewport.MSAA_4X
		AntiAliasingMode.MSAAx8:
			get_window().screen_space_aa = Viewport.SCREEN_SPACE_AA_DISABLED
			get_window().msaa_3d = Viewport.MSAA_8X
		
	antialiasing_mode_changed.emit(mode)


func set_supersampling_scale(scale: float) -> void:
	if scale < SUPERSAMPLING_SCALE_MIN or scale > SUPERSAMPLING_SCALE_MAX:
		scale = 1.0
	_DATA_OBJECT[_SS_KEY] = scale
	
	get_window().scaling_3d_scale = scale
	
	supersampling_value_changed.emit(scale)


func _apply_settings() -> void:
	set_antialiasing_mode(_DATA_OBJECT[_AA_KEY])
	set_supersampling_scale(_DATA_OBJECT[_SS_KEY])


func _get_section() -> String:
	return _CFG_SECTION
