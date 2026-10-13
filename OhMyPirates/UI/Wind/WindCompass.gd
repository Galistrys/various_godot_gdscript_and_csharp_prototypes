extends PanelContainer

export(Vector2) var _min_scale: Vector2 = Vector2(0.25, 0.25)
export(Vector2) var _max_scale: Vector2 = Vector2.ONE

onready var _arrow_texture_rect: TextureRect = $Background/Arrow

func _process(_delta):
	_arrow_texture_rect.rect_rotation = rad2deg(WindManager.get_wind_direction().angle())
	_arrow_texture_rect.rect_scale = (_max_scale - _min_scale) * WindManager.get_wind_strength() + _min_scale
