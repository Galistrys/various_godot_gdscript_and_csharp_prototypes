class_name EndlessTerrain
extends Node3D

const MAX_VIEW_DISTANCE = 48
@warning_ignore(integer_division)
const CHUNKS_VISIBLE_IN_VIEW_DISTANCE = roundi(MAX_VIEW_DISTANCE / Constants.CHUNK_SIZE)

const VISIBLE_KEY = "visible_last_frame"
const CHUNK_OBJ_KEY = "chunk_obj"
const LOD_KEY = "lod"

@export_node_path(Node3D) var _target_node_path := NodePath()
@onready var _target := get_node(_target_node_path) as Node3D

var _terrain_noise_tools := TerrainNoiseTools.new()

var _terrain_chunk_dict := {}

var _current_chunk_coord_x := 9223372036854775807
var _current_chunk_coord_y := 9223372036854775807

var _scenario: RID
var _space: RID

var _flat_map_data: PackedFloat32Array


func _ready():
	_scenario = get_world_3d().scenario
	_space = get_world_3d().space
	
	_flat_map_data = PackedFloat32Array()
	_flat_map_data.resize(Constants.CHUNK_SIZE_P_1 * Constants.CHUNK_SIZE_P_1)
	for x in range(Constants.CHUNK_SIZE_P_1):
		for y in range(Constants.CHUNK_SIZE_P_1):
			_flat_map_data[y * Constants.CHUNK_SIZE_P_1 + x] = 0


func _process(_delta):
	var current_chunk_coord_x = roundi(_target.global_position.x / Constants.CHUNK_SIZE)
	var current_chunk_coord_y = roundi(_target.global_position.z / Constants.CHUNK_SIZE)
	
	if current_chunk_coord_x != _current_chunk_coord_x \
		or current_chunk_coord_y != _current_chunk_coord_y:
			_current_chunk_coord_x = current_chunk_coord_x
			_current_chunk_coord_y = current_chunk_coord_y
			_update_visible_chunks()


func _get_chunk_data(world_pos: Vector2i) -> PackedFloat32Array:
	var result := PackedFloat32Array()
	result.resize(Constants.CHUNK_SIZE_P_1 * Constants.CHUNK_SIZE_P_1)
	
	for y in range(0, Constants.CHUNK_SIZE_P_1):
		for x in range(0, Constants.CHUNK_SIZE_P_1):
			var noise := _terrain_noise_tools.get_noise_for_coord(
				x + world_pos.x, y + world_pos.y
			)
			result[y * Constants.CHUNK_SIZE_P_1 + x] = noise
	
	return result


func _update_visible_chunks() -> void:
	for chunk_key in _terrain_chunk_dict:
		_terrain_chunk_dict[chunk_key][VISIBLE_KEY] = false
	
	for y_offset in range(
		-CHUNKS_VISIBLE_IN_VIEW_DISTANCE,
		CHUNKS_VISIBLE_IN_VIEW_DISTANCE + 1
	):
		for x_offset in range(
			-CHUNKS_VISIBLE_IN_VIEW_DISTANCE,
			CHUNKS_VISIBLE_IN_VIEW_DISTANCE + 1
		):
			
			var viewed_chunk_coord := Vector2i(
				_current_chunk_coord_x + x_offset,
				_current_chunk_coord_y + y_offset
			)
			
			var lod_val := 0
			if abs(viewed_chunk_coord.x - _current_chunk_coord_x) > 1 \
				or abs(viewed_chunk_coord.y - _current_chunk_coord_y) > 1:
				lod_val = 1
			
			if _terrain_chunk_dict.has(viewed_chunk_coord):
				var chunk_data := _get_chunk_data(viewed_chunk_coord * Constants.CHUNK_SIZE)
				_terrain_chunk_dict[viewed_chunk_coord][VISIBLE_KEY] = true
				var lod_old: int = _terrain_chunk_dict[viewed_chunk_coord][LOD_KEY]
				if lod_old != lod_val:
					_terrain_chunk_dict[viewed_chunk_coord][LOD_KEY] = lod_val
					var chunk := _terrain_chunk_dict[viewed_chunk_coord][CHUNK_OBJ_KEY] as Chunk
					chunk.generate(lod_val, chunk_data)
					chunk.remove_from_world()
					chunk.add_to_world(_scenario, _space)
			else:
				var chunk_data := _get_chunk_data(viewed_chunk_coord * Constants.CHUNK_SIZE)
				var chunk := Chunk.new(viewed_chunk_coord * Constants.CHUNK_SIZE)
				_terrain_chunk_dict[viewed_chunk_coord] = {
					VISIBLE_KEY: true,
					CHUNK_OBJ_KEY: chunk,
					LOD_KEY: lod_val
				}
				chunk.generate(lod_val, chunk_data)
				chunk.add_to_world(_scenario, _space)
	
	var to_remove := []
	for chunk_key in _terrain_chunk_dict:
		if not _terrain_chunk_dict[chunk_key][VISIBLE_KEY]:
			to_remove.append(chunk_key)
	
	for chunk_key in to_remove:
		var chunk_obj := _terrain_chunk_dict[chunk_key][CHUNK_OBJ_KEY] as Chunk
		chunk_obj.remove_from_world()
		chunk_obj.free()
		_terrain_chunk_dict.erase(chunk_key)
