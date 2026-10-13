extends BaseManager


## An autoload to handle display related actions
## 
## This class handles changing display related actions, like:
## changing display, display mode, changing resolution, vsync, etc.


## Emitted whenever display mode is changed
signal display_mode_changed(value: int)
## Emitted whenever display idx is changed
signal display_screen_changed(value: int)
## Emitted whenever vsync mode is changed
signal display_vsync_mode_changed(value: bool)
## Emitted whenever resolution is changed
signal display_resolution_changed(value: Vector2i)


enum DisplayMode {
	WINDOWED = 0,
	BORDERLESS = 1,
	FULLSCREEN = 2,
	EXCLUSIVE_FULLSCREEN = 3
}


const MIN_X_RESOLUTION = 800
const MIN_Y_RESOLUTION = 480

const _CFG_SECTION = "display"

const _MODE_KEY = "mode"
const _SCREEN_KEY = "screen"
const _VSYNC_KEY = "vsync"
const _RESOLUTION_KEY = "resolution"

var _DEFAULT_DATA_OBJECT: Dictionary[String, Variant] = {
	_MODE_KEY: DisplayMode.EXCLUSIVE_FULLSCREEN,
	_SCREEN_KEY: DisplayServer.get_primary_screen(),
	_VSYNC_KEY: DisplayServer.VSYNC_ENABLED,
	_RESOLUTION_KEY: DisplayServer.screen_get_size(DisplayServer.get_primary_screen())
}


func _init() -> void:
	_DATA_OBJECT.assign(_DEFAULT_DATA_OBJECT)
	
	super._init()


## Moves the window to the specified screen.
func set_display_screen(screen_idx: int) -> void:
	if screen_idx not in range(0, DisplayServer.get_screen_count()):
		screen_idx = DisplayServer.get_primary_screen()
	
	_DATA_OBJECT[_SCREEN_KEY] = screen_idx
	DisplayServer.window_set_current_screen(screen_idx)
	
	display_screen_changed.emit(screen_idx)


## Sets vsync to be enabled or disabled
func set_display_vsync(vsync: bool) -> void:
	_DATA_OBJECT[_VSYNC_KEY] = vsync
	
	if vsync:
		DisplayServer.window_set_vsync_mode(DisplayServer.VSYNC_ENABLED)
	else:
		DisplayServer.window_set_vsync_mode(DisplayServer.VSYNC_DISABLED)
	
	display_vsync_mode_changed.emit(vsync)


## Sets display mode
func set_display_mode(mode: DisplayMode) -> void:
	if mode not in range(0, 4):
		mode = DisplayMode.EXCLUSIVE_FULLSCREEN
	
	_DATA_OBJECT[_MODE_KEY] = mode
	if mode != DisplayMode.WINDOWED:
		_DATA_OBJECT[_RESOLUTION_KEY] = get_window().size
	
	match mode:
		DisplayMode.WINDOWED:
			DisplayServer.window_set_flag(DisplayServer.WINDOW_FLAG_BORDERLESS, false)
			DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_WINDOWED)
			get_window().size = _DATA_OBJECT[_RESOLUTION_KEY]
		DisplayMode.BORDERLESS:
			DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_WINDOWED)
			DisplayServer.window_set_flag(DisplayServer.WINDOW_FLAG_BORDERLESS, true)
			set_display_resolution(DisplayServer.screen_get_size(_DATA_OBJECT[_SCREEN_KEY]))
			get_window().position = DisplayServer.screen_get_position(_DATA_OBJECT[_SCREEN_KEY])
		DisplayMode.FULLSCREEN:
			DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_FULLSCREEN)
		DisplayMode.EXCLUSIVE_FULLSCREEN:
			DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_EXCLUSIVE_FULLSCREEN)
	
	display_mode_changed.emit(mode)


## Sets display resolution
func set_display_resolution(resolution: Vector2i) -> void:
	resolution.x = maxi(resolution.x, MIN_X_RESOLUTION)
	resolution.y = maxi(resolution.y, MIN_Y_RESOLUTION)
	
	_DATA_OBJECT[_RESOLUTION_KEY] = resolution
	get_window().size = resolution
	
	display_resolution_changed.emit(resolution)


func _apply_settings() -> void:
	set_display_screen(_DATA_OBJECT[_SCREEN_KEY])
	set_display_vsync(_DATA_OBJECT[_VSYNC_KEY])
	set_display_mode(_DATA_OBJECT[_MODE_KEY])
	set_display_resolution(_DATA_OBJECT[_RESOLUTION_KEY])


func _get_section() -> String:
	return _CFG_SECTION
