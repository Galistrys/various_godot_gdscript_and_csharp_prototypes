class_name TerrainChunk
extends RefCounted

#const CHUNK_MATERIAL = preload("res://terrain/low_poly.material")
const CHUNK_MATERIAL = preload("res://world/terrain/other_snow.material")

var lod: int = -1
var height_data := PackedFloat32Array()
var transform: Transform3D

var _instance: RID = RID()
var _mesh: ArrayMesh = null
var _current_lod: int = -1

var _body: RID = RID()
var _shape: ConcavePolygonShape3D = null
var _shape_added := false

func _init(chunk_pos: Vector2i):
	transform = Transform3D(
		Basis(),
		Vector3(
			chunk_pos.x * Constants.CHUNK_SIZE - Constants.CHUNK_SIZE / 2.0,
			0,
			chunk_pos.y * Constants.CHUNK_SIZE - Constants.CHUNK_SIZE / 2.0
		)
	)


func create_instance_and_body(scenario: RID, space: RID) -> void:
	_instance = RenderingServer.instance_create()
	RenderingServer.instance_set_scenario(_instance, scenario)
	RenderingServer.instance_set_transform(_instance, transform)
	
	_body = PhysicsServer3D.body_create()
	PhysicsServer3D.body_set_space(_body, space)
	PhysicsServer3D.body_set_mode(_body, PhysicsServer3D.BODY_MODE_STATIC)
	PhysicsServer3D.body_set_state(_body, PhysicsServer3D.BODY_STATE_TRANSFORM, transform)


func remove_from_world() -> void:
	if _instance.is_valid():
		RenderingServer.free_rid(_instance)
		_instance = RID()
		PhysicsServer3D.free_rid(_body)
		_body = RID()


func generate_collision_shape() -> void:
	var mesh_data = ChunkMeshGenerator.generate_chunk_mesh(
		Constants.CHUNK_SIZE_P_1, 0, height_data
	)
	_shape = ChunkShapeGenerator.create_physics_shape(mesh_data)


func update_chunk() -> void:
	if _current_lod == lod:
		return
	
	_current_lod = lod
	
	if _current_lod == -1:
		RenderingServer.instance_set_base(_instance, RID())
		_mesh = null
	else:
		var mesh_data := ChunkMeshGenerator.generate_chunk_mesh(
			Constants.CHUNK_SIZE_P_1, _current_lod, height_data
		)
		
		_mesh = mesh_data.create_mesh()
		_mesh.surface_set_material(0, CHUNK_MATERIAL)
		RenderingServer.instance_set_base(_instance, _mesh.get_rid())
	
	if _current_lod == 0:
		if not _shape_added:
			PhysicsServer3D.body_add_shape(_body, _shape)
			_shape_added = true
	else:
		if _shape_added:
			PhysicsServer3D.body_remove_shape(_body, 0)
			_shape_added = false


func is_empty() -> bool:
	return not _instance.is_valid()


func to_string() -> String:
	return str("Chunk[", ", ".join([str(_instance.get_id()), str(_mesh.get_rid().get_id()), " - Empty: ", str(is_empty())]), "]")
