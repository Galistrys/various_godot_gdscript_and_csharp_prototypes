class_name RiverHexTile
extends HexTile


const RIVER_VARIANTS: Array[ArrayMesh] = [
	HexModels.river_start_model,
	HexModels.river_end_model,
	HexModels.river_straight_model,
	HexModels.river_corner_model,
	HexModels.river_corner_sharp_model,
	HexModels.river_intersection_a_model,
	HexModels.river_intersection_b_model,
	HexModels.river_intersection_c_model,
	HexModels.river_intersection_d_model,
	HexModels.river_intersection_e_model,
	HexModels.river_intersection_f_model,
	HexModels.river_intersection_g_model,
	HexModels.river_intersection_h_model,
	HexModels.river_crossing_model
]


var river_rotation: float = 0.0
var river_variant: int = 0


var _instance := RID()


func show_tile() -> void:
	_instance = RenderingServer.instance_create()
	RenderingServer.instance_set_scenario(_instance, _scenario)
	RenderingServer.instance_set_base(_instance, RIVER_VARIANTS[river_variant])
	var transform := Transform3D(
		Basis().rotated(Vector3.UP, river_rotation),
		_world_pos
	)
	RenderingServer.instance_set_transform(_instance, transform)


func update_tile(_radius: int, data: Dictionary) -> void:
	river_rotation = data["rotation"]
	river_variant = data["variant"]
	
	RenderingServer.instance_set_base(_instance, RIVER_VARIANTS[river_variant])
	var transform := Transform3D(
		Basis().rotated(Vector3.UP, river_rotation),
		_world_pos
	)
	RenderingServer.instance_set_transform(_instance, transform)


func hide_tile() -> void:
	RenderingServer.free_rid(_instance)
	_instance = RID()


func set_scale(scale: float) -> void:
	RenderingServer.instance_set_transform(
		_instance,
		Transform3D(
			Basis()
			.rotated(Vector3.UP, river_rotation)
			.scaled(Vector3(
				scale, scale, scale
			)),
			_world_pos
		)
	)


func highlight() -> void:
	RenderingServer.instance_geometry_set_material_overlay(
		_instance, HIGHLIGHT_OVERLAY_MAT.get_rid()
	)


func dehighlight() -> void:
	RenderingServer.instance_geometry_set_material_overlay(
		_instance, RID()
	)
