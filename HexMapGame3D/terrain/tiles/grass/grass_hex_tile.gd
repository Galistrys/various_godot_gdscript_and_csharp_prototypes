class_name GrassHexTile
extends HexTile


var _instance := RID()


func show_tile() -> void:
	_instance = RenderingServer.instance_create()
	RenderingServer.instance_set_scenario(_instance, _scenario)
	RenderingServer.instance_set_base(_instance, HexModels.grass_tile_model)
	var transform := Transform3D(Basis(), _world_pos)
	RenderingServer.instance_set_transform(_instance, transform)


func hide_tile() -> void:
	RenderingServer.free_rid(_instance)
	_instance = RID()


func set_scale(scale: float) -> void:
	RenderingServer.instance_set_transform(
		_instance,
		Transform3D(
			Basis().scaled(Vector3(
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
