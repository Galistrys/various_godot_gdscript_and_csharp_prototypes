extends Node3D


const PATH_E = 1
const PATH_NE = 2
const PATH_NW = 4
const PATH_W = 8
const PATH_SW = 16
const PATH_SE = 32

func _ready():
	var indicators_to_variant = []
	
	indicators_to_variant.resize(
		(PATH_E | PATH_NE | PATH_NW | PATH_W | PATH_SW | PATH_SE) + 1
	)
	
	indicators_to_variant[
		0
	] = { "variant": 0, "rotation": 0 }
	
	indicators_to_variant[
		PATH_E
	] = { "variant": 1, "rotation": 180 }
	indicators_to_variant[
		PATH_NE
	] = { "variant": 1, "rotation": 240 }
	indicators_to_variant[
		PATH_NW
	] = { "variant": 1, "rotation": 300 }
	indicators_to_variant[
		PATH_W
	] = { "variant": 1, "rotation": 0 }
	indicators_to_variant[
		PATH_SW
	] = { "variant": 1, "rotation": 60 }
	indicators_to_variant[
		PATH_SE
	] = { "variant": 1, "rotation": 120 }
	
	indicators_to_variant[
		PATH_E | PATH_W
	] = { "variant": 2, "rotation": 0 }
	indicators_to_variant[
		PATH_NE | PATH_SW
	] = { "variant": 2, "rotation": 60 }
	indicators_to_variant[
		PATH_NW | PATH_SE
	] = { "variant": 2, "rotation": 120 }
	
	indicators_to_variant[
		PATH_W | PATH_NE
	] = { "variant": 3, "rotation": 0 }
	indicators_to_variant[
		PATH_NW | PATH_SW
	] = { "variant": 3, "rotation": 60 }
	indicators_to_variant[
		PATH_W | PATH_SE
	] = { "variant": 3, "rotation": 120 }
	indicators_to_variant[
		PATH_SW | PATH_E
	] = { "variant": 3, "rotation": 180 }
	indicators_to_variant[
		PATH_NE | PATH_SE
	] = { "variant": 3, "rotation": 240 }
	indicators_to_variant[
		PATH_NW | PATH_E
	] = { "variant": 3, "rotation": 300 }
	
	indicators_to_variant[
		PATH_NW | PATH_W
	] = { "variant": 4, "rotation": 0 }
	indicators_to_variant[
		PATH_W | PATH_SW
	] = { "variant": 4, "rotation": 60 }
	indicators_to_variant[
		PATH_SW | PATH_SE
	] = { "variant": 4, "rotation": 120 }
	indicators_to_variant[
		PATH_SE | PATH_E
	] = { "variant": 4, "rotation": 180 }
	indicators_to_variant[
		PATH_E | PATH_NE
	] = { "variant": 4, "rotation": 240 }
	indicators_to_variant[
		PATH_NE | PATH_NW
	] = { "variant": 4, "rotation": 300 }
	
	indicators_to_variant[
		PATH_W | PATH_NW | PATH_NE
	] = { "variant": 5, "rotation": 0 }
	indicators_to_variant[
		PATH_SW | PATH_W | PATH_NW
	] = { "variant": 5, "rotation": 60 }
	indicators_to_variant[
		PATH_SE | PATH_SW | PATH_W
	] = { "variant": 5, "rotation": 120 }
	indicators_to_variant[
		PATH_E | PATH_SE | PATH_SW
	] = { "variant": 5, "rotation": 180 }
	indicators_to_variant[
		PATH_NE | PATH_E | PATH_SE
	] = { "variant": 5, "rotation": 240 }
	indicators_to_variant[
		PATH_NW | PATH_NE | PATH_E
	] = { "variant": 5, "rotation": 300 }
	
	indicators_to_variant[
		PATH_W | PATH_E | PATH_NE
	] = { "variant": 6, "rotation": 0 }
	indicators_to_variant[
		PATH_SW | PATH_NE | PATH_NW
	] = { "variant": 6, "rotation": 60 }
	indicators_to_variant[
		PATH_SE | PATH_NW | PATH_W
	] = { "variant": 6, "rotation": 120 }
	indicators_to_variant[
		PATH_E | PATH_W | PATH_SW
	] = { "variant": 6, "rotation": 180 }
	indicators_to_variant[
		PATH_NE | PATH_SW | PATH_SE
	] = { "variant": 6, "rotation": 240 }
	indicators_to_variant[
		PATH_NW | PATH_SE | PATH_E
	] = { "variant": 6, "rotation": 300 }
	
	indicators_to_variant[
		PATH_W | PATH_E | PATH_SE
	] = { "variant": 7, "rotation": 0 }
	indicators_to_variant[
		PATH_SW | PATH_NE | PATH_E
	] = { "variant": 7, "rotation": 60 }
	indicators_to_variant[
		PATH_SE | PATH_NW | PATH_NE
	] = { "variant": 7, "rotation": 120 }
	indicators_to_variant[
		PATH_E | PATH_W | PATH_NW
	] = { "variant": 7, "rotation": 180 }
	indicators_to_variant[
		PATH_NE | PATH_SW | PATH_W
	] = { "variant": 7, "rotation": 240 }
	indicators_to_variant[
		PATH_NW | PATH_SE | PATH_SW
	] = { "variant": 7, "rotation": 300 }
	
	indicators_to_variant[
		PATH_W | PATH_E | PATH_NE | PATH_SE
	] = { "variant": 8, "rotation": 0 }
	indicators_to_variant[
		PATH_SW | PATH_NE | PATH_E | PATH_NW
	] = { "variant": 8, "rotation": 60 }
	indicators_to_variant[
		PATH_SE | PATH_NE | PATH_W | PATH_NW
	] = { "variant": 8, "rotation": 120 }
	indicators_to_variant[
		PATH_E | PATH_W | PATH_NW | PATH_SW
	] = { "variant": 8, "rotation": 180 }
	indicators_to_variant[
		PATH_NE | PATH_SW | PATH_W | PATH_SE
	] = { "variant": 8, "rotation": 240 }
	indicators_to_variant[
		PATH_NW | PATH_SE | PATH_E | PATH_SW
	] = { "variant": 8, "rotation": 300 }
	
	indicators_to_variant[
		PATH_W | PATH_E | PATH_NW | PATH_SE
	] = { "variant": 9, "rotation": 0 }
	indicators_to_variant[
		PATH_W | PATH_E | PATH_SW | PATH_NE
	] = { "variant": 9, "rotation": 60 }
	indicators_to_variant[
		PATH_SE | PATH_NE | PATH_SW | PATH_NW
	] = { "variant": 9, "rotation": 120 }
	
	indicators_to_variant[
		PATH_W | PATH_NE | PATH_SE
	] = { "variant": 10, "rotation": 0 }
	indicators_to_variant[
		PATH_SW | PATH_NW | PATH_E
	] = { "variant": 10, "rotation": 60 }
	indicators_to_variant[
		PATH_SE | PATH_W | PATH_NE
	] = { "variant": 10, "rotation": 120 }
	
	indicators_to_variant[
		PATH_W | PATH_NW | PATH_NE | PATH_E | PATH_SE
	] = { "variant": 11, "rotation": 0 }
	indicators_to_variant[
		PATH_SW | PATH_W | PATH_NW | PATH_NE | PATH_E
	] = { "variant": 11, "rotation": 60 }
	indicators_to_variant[
		PATH_SE | PATH_SW | PATH_W | PATH_NW | PATH_NE
	] = { "variant": 11, "rotation": 120 }
	indicators_to_variant[
		PATH_E | PATH_SE | PATH_SW | PATH_W | PATH_NW
	] = { "variant": 11, "rotation": 180 }
	indicators_to_variant[
		PATH_NE | PATH_E | PATH_SE | PATH_SW | PATH_W
	] = { "variant": 11, "rotation": 240 }
	indicators_to_variant[
		PATH_NW | PATH_NE | PATH_E | PATH_SE | PATH_SW
	] = { "variant": 11, "rotation": 300 }
	
	indicators_to_variant[
		PATH_W | PATH_NW | PATH_NE | PATH_E
	] = { "variant": 12, "rotation": 0 }
	indicators_to_variant[
		PATH_SW | PATH_W | PATH_NW | PATH_NE
	] = { "variant": 12, "rotation": 60 }
	indicators_to_variant[
		PATH_SE | PATH_SW | PATH_W | PATH_NW
	] = { "variant": 12, "rotation": 120 }
	indicators_to_variant[
		PATH_E | PATH_SE | PATH_SW | PATH_W
	] = { "variant": 12, "rotation": 180 }
	indicators_to_variant[
		PATH_NE | PATH_E | PATH_SE | PATH_SW
	] = { "variant": 12, "rotation": 240 }
	indicators_to_variant[
		PATH_NW | PATH_NE | PATH_E | PATH_SE
	] = { "variant": 12, "rotation": 300 }
	
	indicators_to_variant[
		PATH_E | PATH_NE | PATH_NW | PATH_W | PATH_SW | PATH_SE
	] = { "variant": 13, "rotation": 0 }
	
	print(indicators_to_variant)
