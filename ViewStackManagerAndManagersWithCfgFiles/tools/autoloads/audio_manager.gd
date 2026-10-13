extends BaseManager


## Manager for the Audio buses
## 
## This autoload provides mechanisms for controling audio


## Emited whenever master volume is changed
signal master_volume_changed(value: int)
## Emited whenever music volume is changed
signal music_volume_changed(value: int)
## Emited whenever sound volume is changed
signal sound_volume_changed(value: int)


## Maximal audio volume setting
const MAX_VOLUME = 10
## Minimal audio volume setting
const MIN_VOLUME = 0

const _MASTER_BUS_NAME = "Master"
const _MUSIC_BUS_NAME = "Music"
const _SOUNDS_BUS_NAME = "Sounds"

const _CFG_SECTION = "audio"
const _MASTER_VOLUME_KEY = "master_volume"
const _MUSIC_VOLUME_KEY = "music_volume"
const _SOUNDS_VOLUME_KEY = "sounds_volume"

const _DEFAULT_DATA_OBJECT: Dictionary[String, Variant] = {
	_MASTER_VOLUME_KEY: 10,
	_MUSIC_VOLUME_KEY: 10,
	_SOUNDS_VOLUME_KEY: 10
}


var _master_volume_idx: int
var _music_volume_idx: int
var _sounds_volume_idx: int


func _init() -> void:
	_DATA_OBJECT.assign(_DEFAULT_DATA_OBJECT)
	
	_master_volume_idx = AudioServer.get_bus_index(_MASTER_BUS_NAME)
	assert(_master_volume_idx != -1,
			ErrorMsg.AUDIO_BUS_NOT_FOUND % [_MASTER_BUS_NAME])
	_music_volume_idx = AudioServer.get_bus_index(_MUSIC_BUS_NAME)
	assert(_music_volume_idx != -1,
			ErrorMsg.AUDIO_BUS_NOT_FOUND % [_MUSIC_BUS_NAME])
	_sounds_volume_idx = AudioServer.get_bus_index(_SOUNDS_BUS_NAME)
	assert(_sounds_volume_idx != -1,
			ErrorMsg.AUDIO_BUS_NOT_FOUND % [_SOUNDS_BUS_NAME])
	
	super._init()


## Set master volume. The value will be automatically clamped
func set_master_volume(volume: int) -> void:
	volume = clampi(volume, MIN_VOLUME, MAX_VOLUME)
	_DATA_OBJECT[_MASTER_VOLUME_KEY] = volume
	AudioServer.set_bus_volume_db(_master_volume_idx, _linear_to_db(volume))
	master_volume_changed.emit(volume)


## Set music volume. The value will be automatically clamped
func set_music_volume(volume: int) -> void:
	volume = clampi(volume, MIN_VOLUME, MAX_VOLUME)
	_DATA_OBJECT[_MUSIC_VOLUME_KEY] = volume
	AudioServer.set_bus_volume_db(_music_volume_idx, _linear_to_db(volume))
	music_volume_changed.emit(volume)


## Set sounds volume. The value will be automatically clamped
func set_sounds_volume(volume: int) -> void:
	volume = clampi(volume, MIN_VOLUME, MAX_VOLUME)
	_DATA_OBJECT[_SOUNDS_VOLUME_KEY] = volume
	AudioServer.set_bus_volume_db(_sounds_volume_idx, _linear_to_db(volume))
	sound_volume_changed.emit(volume)


func _apply_settings() -> void:
	set_master_volume(_DATA_OBJECT[_MASTER_VOLUME_KEY])
	set_music_volume(_DATA_OBJECT[_MUSIC_VOLUME_KEY])
	set_sounds_volume(_DATA_OBJECT[_SOUNDS_VOLUME_KEY])


func _get_section() -> String:
	return _CFG_SECTION


static func _linear_to_db(volume: int) -> float:
	return linear_to_db(volume / float(MAX_VOLUME))
