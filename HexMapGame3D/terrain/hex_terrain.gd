class_name HexTerrain
extends Node3D


@export_range(8, 128) var map_size := 10
@export var enable_debug_draw := false


@onready var _hex_renderer := $HexRenderer as HexRenderer
var _map_data: MapData


func _init():
	randomize()


func _ready() -> void:
	if enable_debug_draw:
		_add_debug_grid()
	
	_map_data = MapData.new(map_size)
	_map_data.generate_map_data()
	
	_hex_renderer.generate_tiles_from_map_data(_map_data)


func _on_hex_renderer_tile_coords_clicked(coords: Vector2i):
	var tile_data: MapTileData
	if _map_data.get_tile_data(coords) is GrassMapTileData:
		tile_data = RiverMapTileData.new()
	else:
		tile_data = GrassMapTileData.new()
	
	_hex_renderer.reload_tile(coords, tile_data)
	_map_data.change_tile(coords, tile_data)


func _add_debug_grid() -> void:
	var im := ImmediateMesh.new()
	im.surface_begin(Mesh.PRIMITIVE_LINES)
	
	for x in range(map_size):
		for y in range(map_size):
			var grid_coords := Vector2i(x, y)
			var world_coords := HexTools.hex_to_world_pos(grid_coords, Constants.TILE_SIZE)
			
			var corners: Array[Vector2] = []
			for i in range(0, 6):
				corners.append(HexTools.hex_corner(world_coords, Constants.TILE_SIZE, i))
			
			for i in range(0, 6):
				var a := corners[i]
				var b := corners[i + 1 if i != 5 else 0]
				im.surface_add_vertex(Vector3(a.x, 0, a.y))
				im.surface_add_vertex(Vector3(b.x, 0, b.y))
	
	im.surface_end()
	
	var material := StandardMaterial3D.new()
	material.albedo_color = Color.BLACK
	
	im.surface_set_material(0, material)
	
	var mesh_instance := MeshInstance3D.new()
	mesh_instance.mesh = im
	mesh_instance.position.y += 0.201
	add_child(mesh_instance)
