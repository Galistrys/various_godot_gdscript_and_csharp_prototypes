class_name RiverMapTileData
extends MapTileData


const RIVER_E = 1
const RIVER_NE = 2
const RIVER_NW = 4
const RIVER_W = 8
const RIVER_SW = 16
const RIVER_SE = 32

const INDICATOR_TO_VARIANT: Array[Dictionary] = [
	{ "variant": 0, "rotation": 0 },
	{ "variant": 1, "rotation": deg_to_rad(180) },
	{ "variant": 1, "rotation": deg_to_rad(240) },
	{ "variant": 4, "rotation": deg_to_rad(240) },
	{ "variant": 1, "rotation": deg_to_rad(300) },
	{ "variant": 3, "rotation": deg_to_rad(300) },
	{ "variant": 4, "rotation": deg_to_rad(300) },
	{ "variant": 5, "rotation": deg_to_rad(300) },
	{ "variant": 1, "rotation": 0 },
	{ "variant": 2, "rotation": 0 },
	{ "variant": 3, "rotation": 0 },
	{ "variant": 6, "rotation": 0 },
	{ "variant": 4, "rotation": 0 },
	{ "variant": 7, "rotation": deg_to_rad(180) },
	{ "variant": 5, "rotation": 0 },
	{ "variant": 12, "rotation": 0 },
	{ "variant": 1, "rotation": deg_to_rad(60) },
	{ "variant": 3, "rotation": deg_to_rad(180) },
	{ "variant": 2, "rotation": deg_to_rad(60) },
	{ "variant": 7, "rotation": deg_to_rad(60) },
	{ "variant": 3, "rotation": deg_to_rad(60) },
	{ "variant": 10, "rotation": deg_to_rad(60) },
	{ "variant": 6, "rotation": deg_to_rad(60) },
	{ "variant": 8, "rotation": deg_to_rad(60) },
	{ "variant": 4, "rotation": deg_to_rad(60) },
	{ "variant": 6, "rotation": deg_to_rad(180) },
	{ "variant": 7, "rotation": deg_to_rad(240) },
	{ "variant": 9, "rotation": deg_to_rad(60) },
	{ "variant": 5, "rotation": deg_to_rad(60) },
	{ "variant": 8, "rotation": deg_to_rad(180) },
	{ "variant": 12, "rotation": deg_to_rad(60) },
	{ "variant": 11, "rotation": deg_to_rad(60) },
	{ "variant": 1, "rotation": deg_to_rad(120) },
	{ "variant": 4, "rotation": deg_to_rad(180) },
	{ "variant": 3, "rotation": deg_to_rad(240) },
	{ "variant": 5, "rotation": deg_to_rad(240) },
	{ "variant": 2, "rotation": deg_to_rad(120) },
	{ "variant": 6, "rotation": deg_to_rad(300) },
	{ "variant": 7, "rotation": deg_to_rad(120) },
	{ "variant": 12, "rotation": deg_to_rad(300) },
	{ "variant": 3, "rotation": deg_to_rad(120) },
	{ "variant": 7, "rotation": 0 },
	{ "variant": 10, "rotation": deg_to_rad(120) },
	{ "variant": 8, "rotation": 0 },
	{ "variant": 6, "rotation": deg_to_rad(120) },
	{ "variant": 9, "rotation": 0 },
	{ "variant": 8, "rotation": deg_to_rad(120) },
	{ "variant": 11, "rotation": 0 },
	{ "variant": 4, "rotation": deg_to_rad(120) },
	{ "variant": 5, "rotation": deg_to_rad(180) },
	{ "variant": 6, "rotation": deg_to_rad(240) },
	{ "variant": 12, "rotation": deg_to_rad(240) },
	{ "variant": 7, "rotation": deg_to_rad(300) },
	{ "variant": 8, "rotation": deg_to_rad(300) },
	{ "variant": 9, "rotation": deg_to_rad(120) },
	{ "variant": 11, "rotation": deg_to_rad(300) },
	{ "variant": 5, "rotation": deg_to_rad(120) },
	{ "variant": 12, "rotation": deg_to_rad(180) },
	{ "variant": 8, "rotation": deg_to_rad(240) },
	{ "variant": 11, "rotation": deg_to_rad(240) },
	{ "variant": 12, "rotation": deg_to_rad(120) },
	{ "variant": 11, "rotation": deg_to_rad(180) },
	{ "variant": 11, "rotation": deg_to_rad(120) },
	{ "variant": 13, "rotation": 0 }
]

var river_variant := 0

func update_tile(_neighbors: Array[MapTileData], _data: Dictionary = {}) -> bool:
	var river_indicator = 0
	if _neighbors[HexTools.E] is RiverMapTileData \
	or _neighbors[HexTools.E] is WaterMapTileData:
		river_indicator |= RIVER_E
	if _neighbors[HexTools.NE] is RiverMapTileData \
	or _neighbors[HexTools.NE] is WaterMapTileData:
		river_indicator |= RIVER_NE
	if _neighbors[HexTools.NW] is RiverMapTileData \
	or _neighbors[HexTools.NW] is WaterMapTileData:
		river_indicator |= RIVER_NW
	if _neighbors[HexTools.W] is RiverMapTileData \
	or _neighbors[HexTools.W] is WaterMapTileData:
		river_indicator |= RIVER_W
	if _neighbors[HexTools.SW] is RiverMapTileData \
	or _neighbors[HexTools.SW] is WaterMapTileData:
		river_indicator |= RIVER_SW
	if _neighbors[HexTools.SE] is RiverMapTileData \
	or _neighbors[HexTools.SE] is WaterMapTileData:
		river_indicator |= RIVER_SE
	
	if river_variant != river_indicator:
		river_variant = river_indicator
		var data := INDICATOR_TO_VARIANT[river_indicator]
		tile_changed.emit(0, data)
	return false


func generate_hex_tile(scenario: RID, world_pos: Vector3) -> HexTile:
	return RiverHexTile.new(scenario, world_pos)
