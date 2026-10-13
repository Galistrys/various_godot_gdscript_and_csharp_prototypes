class_name TerrainMeshGenerator
extends RefCounted


const POS_X_NEIGHBOR = 1
const POS_Z_NEIGHBOR = 2
const NEG_X_NEIGHBOR = 4
const NEG_Z_NEIGHBOR = 8

enum LodDirection {
	POS_X, POS_Z, NEG_X, NEG_Z
}

var _surface_tool := SurfaceTool.new()

var _chunk_size: int
var _size_int_chunks: int
var _result_mesh_size: int

var _max_coord: int
var _min_coord: int

var _detail_levels: Array[int]


func _init(chunk_size: int, detail_levels: Array[int]):
	_chunk_size = chunk_size
	_detail_levels = detail_levels
	
	_size_int_chunks = detail_levels.size() * 2 - 1
	_result_mesh_size = _size_int_chunks * chunk_size
	
	_min_coord = -_detail_levels.size() + 1
	_max_coord = _detail_levels.size() - 1


func generate_mesh() -> ArrayMesh:
	_surface_tool.begin(Mesh.PRIMITIVE_TRIANGLES)
	
	for z in range(_min_coord, _max_coord + 1):
		for x in range(_min_coord, _max_coord + 1):
			var dist := _chunk_distance(x, z, 0, 0)
			var detail_level := _detail_levels[dist]
			var lod_neighbor := _get_neighbor_lod(x, z)
			
			_add_chunk(Vector3(x, 0, z) * _chunk_size, detail_level, lod_neighbor)
	
	_surface_tool.generate_tangents()
	_surface_tool.index()
	
	return _surface_tool.commit()


func _chunk_distance(x1: int, z1: int, x2: int, z2: int) -> int:
	return maxi(absi(x1 - x2), absi(z1 - z2))


func _get_neighbor_lod(x: int, z: int) -> int:
	var current_lod := _detail_levels[_chunk_distance(x, z, 0, 0)]
	
	var pos_x := clampi(x + 1, _min_coord, _max_coord)
	var pos_z := clampi(x - 1, _min_coord, _max_coord)
	var neg_x := clampi(z + 1, _min_coord, _max_coord)
	var neg_z := clampi(z - 1, _min_coord, _max_coord)
	
	if   _detail_levels[_chunk_distance(pos_x, z, 0, 0)] < current_lod:
		return POS_X_NEIGHBOR
	elif _detail_levels[_chunk_distance(neg_x, z, 0, 0)] < current_lod:
		return NEG_X_NEIGHBOR
	elif _detail_levels[_chunk_distance(x, pos_z, 0, 0)] < current_lod:
		return POS_Z_NEIGHBOR
	elif _detail_levels[_chunk_distance(x, neg_z, 0, 0)] < current_lod:
		return NEG_Z_NEIGHBOR
	else:
		return 0

func _add_chunk(pos: Vector3, detail_level: int, lod_neighbors: int) -> void:
	var start_pos: Vector3 = pos - Vector3(_chunk_size / 2.0, 0.0, _chunk_size / 2.0)
	
	var pos_x_lod := lod_neighbors & POS_X_NEIGHBOR > 0
	var pos_z_lod := lod_neighbors & POS_Z_NEIGHBOR > 0
	var neg_x_lod := lod_neighbors & NEG_X_NEIGHBOR > 0
	var neg_z_lod := lod_neighbors & NEG_Z_NEIGHBOR > 0
	
	for x in range(0, _chunk_size, detail_level):
		for z in range(0, _chunk_size, detail_level):
			var chunk_pos := Vector3(x, 0, z)
			
			if x == 0 and neg_x_lod:
				_add_lod_quad_at(start_pos + chunk_pos, LodDirection.NEG_X, detail_level)
			elif z == 0 and neg_z_lod:
				_add_lod_quad_at(start_pos + chunk_pos, LodDirection.NEG_Z, detail_level)
			elif x == _chunk_size - detail_level and pos_x_lod:
				_add_lod_quad_at(start_pos + chunk_pos, LodDirection.POS_X, detail_level)
			elif z == _chunk_size - detail_level and pos_z_lod:
				_add_lod_quad_at(start_pos + chunk_pos, LodDirection.POS_Z, detail_level)
			else:
				_add_quad_at(start_pos + chunk_pos, detail_level)


func _add_quad_at(pos: Vector3, size: int) -> void:
	_add_quad(
		pos, pos + Vector3(size, 0, 0),
		pos + Vector3(size, 0, size), pos + Vector3(0, 0, size)
	)


func _add_lod_quad_at(pos: Vector3, direction: LodDirection, size: int) -> void:
	_add_lod_quad(
		pos, pos + Vector3(size, 0, 0),
		pos + Vector3(size, 0, size), pos + Vector3(0, 0, size),
		direction
	)


func _add_lod_quad(
	a: Vector3, b: Vector3, c: Vector3, d: Vector3,
	direction: LodDirection
) -> void:
	match direction:
		LodDirection.POS_X:
			var mid_point_pos_x := (b + c) * 0.5
			_add_triangle(a, b, mid_point_pos_x)
			_add_triangle(a, mid_point_pos_x, d)
			_add_triangle(d, mid_point_pos_x, c)
		LodDirection.POS_Z:
			var mid_point_pos_z := (d + c) * 0.5
			_add_triangle(a, mid_point_pos_z, d)
			_add_triangle(a, b, mid_point_pos_z)
			_add_triangle(b, c, mid_point_pos_z)
		LodDirection.NEG_X:
			var mid_point_neg_x := (a + d) * 0.5
			_add_triangle(a, b, mid_point_neg_x)
			_add_triangle(b, c, mid_point_neg_x)
			_add_triangle(mid_point_neg_x, c, d)
		LodDirection.NEG_Z:
			var mid_point_neg_z := (a + b) * 0.5
			_add_triangle(a, mid_point_neg_z, d)
			_add_triangle(d, mid_point_neg_z, c)
			_add_triangle(b, c, mid_point_neg_z)


func _add_quad(a: Vector3, b: Vector3, c: Vector3, d: Vector3) -> void:
	_add_triangle(a, b, c)
	_add_triangle(a, c, d)


func _add_triangle(a: Vector3, b: Vector3, c: Vector3) -> void:
	_surface_tool.set_normal(Vector3.UP)
	_surface_tool.set_uv(_get_uv_for_pos(a))
	_surface_tool.add_vertex(a)
	
	_surface_tool.set_normal(Vector3.UP)
	_surface_tool.set_uv(_get_uv_for_pos(b))
	_surface_tool.add_vertex(b)
	
	_surface_tool.set_normal(Vector3.UP)
	_surface_tool.set_uv(_get_uv_for_pos(c))
	_surface_tool.add_vertex(c)

func _get_uv_for_pos(pos: Vector3) -> Vector2:
	return (Vector2(pos.x, pos.z) - (Vector2(_result_mesh_size, _result_mesh_size) / 2.0)) / _result_mesh_size
