class_name RoadHexTile
extends HexTile


const ROAD_HEIGHT_OFFSET = 0.2
const ROAD_VARIANTS: Array[ArrayMesh] = [
	HexModels.road_start_model,
	HexModels.road_end_model,
	HexModels.road_straight_model,
	HexModels.road_corner_model,
	HexModels.road_corner_sharp_model,
	HexModels.road_intersection_a_model,
	HexModels.road_intersection_b_model,
	HexModels.road_intersection_c_model,
	HexModels.road_intersection_d_model,
	HexModels.road_intersection_e_model,
	HexModels.road_intersection_f_model,
	HexModels.road_intersection_g_model,
	HexModels.road_intersection_h_model,
	HexModels.road_crossing_model
]


var road_rotation: float = 0.0
var road_variant: int = 0


var _grass_instance := RID()
var _road_instance := RID()


func show_tile() -> void:
	_grass_instance = RenderingServer.instance_create()
	RenderingServer.instance_set_scenario(_grass_instance, _scenario)
	RenderingServer.instance_set_base(_grass_instance, HexModels.grass_tile_model)
	var grass_transform := Transform3D(Basis(), _world_pos)
	RenderingServer.instance_set_transform(_grass_instance, grass_transform)
	
	_road_instance = RenderingServer.instance_create()
	RenderingServer.instance_set_scenario(_road_instance, _scenario)
	RenderingServer.instance_set_base(_road_instance, ROAD_VARIANTS[road_variant])
	var road_transform := Transform3D(
		Basis().rotated(Vector3.UP, road_rotation),
		_world_pos + Vector3.UP * ROAD_HEIGHT_OFFSET
	)
	RenderingServer.instance_set_transform(_road_instance, road_transform)


func update_tile(_radius: int, data: Dictionary) -> void:
	road_rotation = data["rotation"]
	road_variant = data["variant"]
	
	RenderingServer.instance_set_base(_road_instance, ROAD_VARIANTS[road_variant])
	var road_transform := Transform3D(
		Basis().rotated(Vector3.UP, road_rotation),
		_world_pos + Vector3.UP * ROAD_HEIGHT_OFFSET
	)
	RenderingServer.instance_set_transform(_road_instance, road_transform)


func hide_tile() -> void:
	RenderingServer.free_rid(_grass_instance)
	RenderingServer.free_rid(_road_instance)
	_grass_instance = RID()
	_road_instance = RID()


func set_scale(scale: float) -> void:
	RenderingServer.instance_set_transform(
		_grass_instance,
		Transform3D(
			Basis().scaled(Vector3(
				scale, scale, scale
			)),
			_world_pos
		)
	)
	RenderingServer.instance_set_transform(
		_road_instance,
		Transform3D(
			Basis()
			.rotated(Vector3.UP, road_rotation)
			.scaled(Vector3(
				scale, scale, scale
			)),
			_world_pos + Vector3.UP * ROAD_HEIGHT_OFFSET * scale
		)
	)


func highlight() -> void:
	RenderingServer.instance_geometry_set_material_overlay(
		_grass_instance, HIGHLIGHT_OVERLAY_MAT.get_rid()
	)
	RenderingServer.instance_geometry_set_material_overlay(
		_road_instance, HIGHLIGHT_OVERLAY_MAT.get_rid()
	)


func dehighlight() -> void:
	RenderingServer.instance_geometry_set_material_overlay(
		_grass_instance, RID()
	)
	RenderingServer.instance_geometry_set_material_overlay(
		_road_instance, RID()
	)
