class_name WaterMapTileData
extends MapTileData


const ROCKS_THRESHOLD = 0.8
const ISLAND_THRESHOLD = 0.9


var water_variant := -1


func generate_hex_tile(scenario: RID, world_pos: Vector3) -> HexTile:
	if water_variant == -1:
		var random_value := randf()
		if random_value > ISLAND_THRESHOLD:
			water_variant = Constants.WaterTileVariant.Island
		elif random_value > ROCKS_THRESHOLD:
			water_variant = Constants.WaterTileVariant.Rocks
		else:
			water_variant = Constants.WaterTileVariant.Normal
	return WaterHexTile.new(scenario, world_pos, water_variant)
