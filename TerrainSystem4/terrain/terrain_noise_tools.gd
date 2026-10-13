class_name TerrainNoiseTools
extends RefCounted

var _noise := preload("res://terrain/default_noise.res") as FastNoiseLite


func get_noise_for_coord(x: int, y: int) -> float:
	return remap(_noise.get_noise_2d(x, y), -1, 1, 0, 1)
