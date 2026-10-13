class_name Global
extends Object


## Class for holding all global, constant data and some helper methods to
## use them.
##
## It's not allowed to create an instance of this class


# -------- CREATURE SIZE -------- #

## Enum defining creature size
enum CreatureSize {
	INVALID, CREATURE_TINY, CREATURE_SMALL, CREATURE_MEDIUM, CREATURE_LARGE, CREATURE_HUGE
}

## Converts [enum CreatureSize] enum to [String]
static func creature_size_to_str(p_creature_size: CreatureSize) -> String:
	return CreatureSize.keys()[p_creature_size]

# -------- CREATURE SIZE -------- #

# -------- NAVIGATION -------- #

## Navigation chunk size
const NAV_CHUNK_SIZE = 16
## Navigation rasterization cell size
const NAV_CELL_SIZE = 0.0625
## Navigation rasterization cell height
const NAV_CELL_HEIGHT = 0.125
## Navigation max slope
const NAV_MAX_SLOPE = 50.0
## Navigation task thread sleep duration in msec
const NAV_TASK_DELAY = 1


static var _current_nav_level_id_cache: String = ""
static var _nav_chunks_groups_names_cache: Dictionary[Vector2i, StringName] = {}


## Create chunk's group name in format [code]<level_id>_CHUNK_<x>_<y>[/code]
static func get_nav_group_name(p_level_id: String, p_coords: Vector2i) -> StringName:
	return StringName("%s_CHUNK_%d_%d" % [p_level_id, p_coords.x, p_coords.y])


static func get_current_nav_cache_level_id() -> String:
	return _current_nav_level_id_cache


static func get_cached_nav_group_name(p_coords: Vector2i) -> StringName:
	return _nav_chunks_groups_names_cache[p_coords]


static func generate_nav_groups_cache(
	p_level_id: String,
	p_chunks_grid_size: Vector2i
) -> void:
	_current_nav_level_id_cache = p_level_id
	_nav_chunks_groups_names_cache = {}
	for x in range(p_chunks_grid_size.x):
		for z in range(p_chunks_grid_size.y):
			var chunk_coords := Vector2i(x, z)
			var group_name := get_nav_group_name(p_level_id, chunk_coords)
			_nav_chunks_groups_names_cache[chunk_coords] = group_name


## Returns chunk coords from global position
static func get_chunk_coords_from_global_pos(p_global_pos: Vector3) -> Vector2i:
	return Vector2i(
		floori(p_global_pos.x / NAV_CHUNK_SIZE),
		floori(p_global_pos.z / NAV_CHUNK_SIZE))


## Returns preconfigured [NavigationMesh] for establishing current's level [AABB] box.
static func create_dummy_nav_mesh() -> NavigationMesh:
	var nav_mesh := NavigationMesh.new()
	
	nav_mesh.geometry_parsed_geometry_type = \
			NavigationMesh.PARSED_GEOMETRY_STATIC_COLLIDERS
	nav_mesh.geometry_source_geometry_mode = \
			NavigationMesh.SOURCE_GEOMETRY_ROOT_NODE_CHILDREN
	
	nav_mesh.cell_size = NAV_CELL_SIZE
	nav_mesh.cell_height = NAV_CELL_HEIGHT
	
	return nav_mesh


## Returns preconfigured [NavigationMesh] for a [NavChunk].
static func create_chunk_nav_mesh(p_group_name: StringName, p_aabb: AABB) -> NavigationMesh:
	var nav_mesh := NavigationMesh.new()
	
	nav_mesh.sample_partition_type = NavigationMesh.SAMPLE_PARTITION_MONOTONE
	nav_mesh.geometry_parsed_geometry_type = NavigationMesh.PARSED_GEOMETRY_STATIC_COLLIDERS
	
	nav_mesh.geometry_source_geometry_mode = NavigationMesh.SOURCE_GEOMETRY_GROUPS_WITH_CHILDREN
	nav_mesh.geometry_source_group_name = p_group_name
	
	nav_mesh.filter_baking_aabb = p_aabb.grow(NAV_CHUNK_SIZE)
	nav_mesh.border_size = NAV_CHUNK_SIZE
	
	nav_mesh.detail_sample_distance = 2.0
	nav_mesh.detail_sample_max_error = 1.0
	
	nav_mesh.cell_size = NAV_CELL_SIZE
	nav_mesh.cell_height = NAV_CELL_HEIGHT
	
	nav_mesh.agent_max_slope = NAV_MAX_SLOPE
	
	return nav_mesh


# -------- NAVIGATION -------- #

func _init() -> void:
	assert(false, "Tried to create an instance of class Global")
