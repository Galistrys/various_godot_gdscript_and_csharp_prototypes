class_name Chunk
extends Object

const CHUNK_MATERIAL = preload("res://terrain/low_poly.material")

var instance: RID = RID()
var mesh: ArrayMesh = null
var world_pos: Vector3

#var body: RID = RID()
#var shape: ConcavePolygonShape3D = null

func _init(pos: Vector2i):
	world_pos = Vector3(
		pos.x * Constants.CHUNK_SIZE - Constants.CHUNK_SIZE / 2.0,
		0,
		pos.y * Constants.CHUNK_SIZE - Constants.CHUNK_SIZE / 2.0
	)


func add_to_world(scenario: RID, _space: RID) -> void:
	var xform = Transform3D(Basis(), world_pos)
	
	instance = RenderingServer.instance_create()
	RenderingServer.instance_set_scenario(instance, scenario)
	RenderingServer.instance_set_base(instance, mesh.get_rid())
	RenderingServer.instance_set_transform(instance, xform)
	
#	body = PhysicsServer3D.body_create()
#	PhysicsServer3D.body_set_mode(body, PhysicsServer3D.BODY_MODE_STATIC)
#	PhysicsServer3D.body_add_shape(body, shape, xform)
#	PhysicsServer3D.body_set_space(body, space)


func remove_from_world() -> void:
	if instance.is_valid():
		RenderingServer.free_rid(instance)
		instance = RID()
#		PhysicsServer3D.free_rid(body)
#		body = RID()


func generate(
	detail_level: int,
	height_data: PackedFloat32Array
) -> void:
	var mesh_data := ChunkMeshGenerator.generate_chunk_mesh(
		Constants.CHUNK_SIZE_P_1, detail_level, height_data
	)
	
	mesh = mesh_data.create_mesh()
	mesh.surface_set_material(0, CHUNK_MATERIAL)


func update_mesh() -> void:
	RenderingServer.instance_set_base(instance, mesh.get_rid())


func is_empty() -> bool:
	return not instance.is_valid()


func to_string() -> String:
	return str("Chunk[", ", ".join([str(instance.get_id()), str(mesh.get_rid().get_id()), " - Empty: ", str(is_empty())]), "]")
