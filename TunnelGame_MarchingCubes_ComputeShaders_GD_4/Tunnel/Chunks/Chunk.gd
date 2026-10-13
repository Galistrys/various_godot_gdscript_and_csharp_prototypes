extends RefCounted
class_name Chunk

const OFFSET = Vector3(
	ChunkMeshGenerator.CHUNK_SIZE,
	ChunkMeshGenerator.CHUNK_SIZE,
	ChunkMeshGenerator.CHUNK_SIZE
) / -2.0

const SURFACE_LEVEL = 0.5

enum DetailLevel
{
	LEVEL_4 = 1,
	LEVEL_3 = 2,
	LEVEL_2 = 4,
	LEVEL_1 = 8
}

var instance: RID = RID()
var mesh: ArrayMesh = null

var body: RID = RID()
var shape: ConcavePolygonShape3D = null

var key: String = ""

func _init(chunk_key: String):
	key = chunk_key

func is_empty() -> bool:
	return not instance.is_valid()

func remove_from_world() -> void:
	if instance.is_valid():
		RenderingServer.free_rid(instance)
		instance = RID()
		PhysicsServer3D.free_rid(body)
		body = RID()

func add_to_world(world_pos: Vector3) -> void:
	var xform = Transform3D(Basis(), world_pos)
	
	instance = RenderingServer.instance_create()
	RenderingServer.instance_set_scenario(instance, WorldMapData.get_scenario())
	RenderingServer.instance_set_base(instance, mesh.get_rid())
	RenderingServer.instance_set_transform(instance, xform)
	
	body = PhysicsServer3D.body_create()
	PhysicsServer3D.body_set_mode(body, PhysicsServer3D.BODY_MODE_STATIC)
	PhysicsServer3D.body_add_shape(body, shape, xform)
	PhysicsServer3D.body_set_space(body, WorldMapData.get_space())
#	PhysicsServer3D.body_set_state(body, PhysicsServer3D.BODY_STATE_TRANSFORM, xform)

# values_data should be passed as an argument later
func generate(
	world_pos: Vector3,
	detail_level: int
) -> void:
	var values_data: PackedFloat32Array = PackedFloat32Array()
	values_data.resize(ChunkMeshGenerator.CHUNK_SIZE_1_3)
	
	for x in range(0, ChunkMeshGenerator.CHUNK_SIZE_1):
		for y in range(0, ChunkMeshGenerator.CHUNK_SIZE_1):
			for z in range(0, ChunkMeshGenerator.CHUNK_SIZE_1):
				var idx: int = ChunkMeshGenerator.CHUNK_SIZE_1_2 * z \
					+ ChunkMeshGenerator.CHUNK_SIZE_1 * y + x
#				values_data[idx] = WorldMapData.get_value(Vector3(x, y, z) + world_pos)
				values_data[idx] = WorldMapData.get_map_data_v(Vector3(x, y, z) + world_pos)
	
	var verts: PackedVector3Array = ChunkMeshGenerator.generate_verticies(
		values_data, detail_level, SURFACE_LEVEL
	)
	
	if verts.is_empty():
		mesh = null
		remove_from_world()
	else:
		var normals: PackedVector3Array = ChunkMeshGenerator.generate_normals(verts)
		
		mesh = ChunkMeshGenerator.generate_mesh(
			verts, normals
		)
		
#		ResourceSaver.save(mesh, str("user://test", key, ".tres"))
		
		if detail_level == DetailLevel.LEVEL_4:
			shape = ChunkMeshGenerator.generate_shape(verts)
		else:
			verts = ChunkMeshGenerator.generate_verticies(values_data, DetailLevel.LEVEL_4, SURFACE_LEVEL)
			shape = ChunkMeshGenerator.generate_shape(verts)
		
		remove_from_world()
		add_to_world(world_pos)

func to_string() -> String:
	return str("Chunk[", ", ".join([str(instance.get_id()), str(mesh.get_rid().get_id()), key, str(is_empty())]), "]")
	
	
