class_name Level
extends Node3D


## Level class, that handles level's data management


## Level's id
var level_id: String


var _chunks_grid_size: Vector2i


@onready var _terrain: Node3D = $Terrain


func _ready() -> void:
	pass


## Initialize Level and set it's id
func initialize_async(p_level_id: String) -> void:
	level_id = p_level_id
	await get_tree().physics_frame


## Assign every, physics-enabled node to a specific navigation group/groups
## for navigation mesh generation
func set_up_nav_data_async(p_chunks_grid_size: Vector2i) -> void:
	Global.generate_nav_groups_cache(level_id, p_chunks_grid_size)
	_chunks_grid_size = p_chunks_grid_size
	for x in range(p_chunks_grid_size.x):
		for z in range(p_chunks_grid_size.y):
			var chunk_coords := Vector2i(x, z)
			var chunk_id := Global.get_cached_nav_group_name(chunk_coords)
			_terrain.add_to_group(chunk_id)
	
	await get_tree().physics_frame
	
	# TODO: Set up other nodes


func _input(event: InputEvent) -> void:
	if event is InputEventKey:
		if event.pressed and event.keycode == KEY_SPACE:
			NavigationService.request_path(
					Global.CreatureSize.CREATURE_MEDIUM,
					Vector3(2, 2, 2), Vector3(56, 6, 64),
					_result_path)


func _result_path(path: PackedVector3Array) -> void:
	var path_3d := $Path3D as Path3D
	var curve := Curve3D.new()
	for point in path:
		curve.add_point(point)
	path_3d.curve = curve
