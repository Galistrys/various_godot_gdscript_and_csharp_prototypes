class_name MapTileData
extends RefCounted


signal tile_changed(radius: int, data: Dictionary)

var height: int = 0


## Updates map tile data. Returns [code]true[/code] if neighbor tiles needs to be updated.
func update_tile(_neighbors: Array[MapTileData], _data: Dictionary = {}) -> bool:
	return false


func generate_hex_tile(_scenario: RID, _world_pos: Vector3) -> HexTile:
	return null
