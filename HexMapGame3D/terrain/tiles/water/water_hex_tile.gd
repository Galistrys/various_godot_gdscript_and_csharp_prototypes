class_name WaterHexTile
extends HexTile


const WATER_TILE_VARIANT_KEY = "water_tile_new_variant"

var _instance := RID()
var _model: Mesh = null
var water_variant: int = 0
var _current_variant: int = 0


func _init(scenario: RID, world_pos: Vector3, variant: int):
	super(scenario, world_pos)
	water_variant = variant
	_current_variant = variant
	_model = _get_model_from_variant(variant)


func _get_model_from_variant(variant: int) -> ArrayMesh:
	if variant == Constants.WaterTileVariant.Normal:
		return HexModels.water_normal_tile_model
	elif variant == Constants.WaterTileVariant.Rocks:
		return HexModels.water_rocks_tile_model
	else:
		return HexModels.water_island_tile_model


func show_tile() -> void:
	_instance = RenderingServer.instance_create()
	RenderingServer.instance_set_scenario(_instance, _scenario)
	RenderingServer.instance_set_base(_instance, _model)
	var transform := Transform3D(Basis(), _world_pos)
	RenderingServer.instance_set_transform(_instance, transform)


func update_tile(_radius: int, _data: Dictionary) -> void:
	if _current_variant != water_variant:
		_current_variant = water_variant
		_model = _get_model_from_variant(_current_variant)
		RenderingServer.instance_set_base(_instance, _model)


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
