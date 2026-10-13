class_name ChunkMeshData
extends RefCounted

var vertices := PackedVector3Array()
#var normals := PackedVector3Array()
var triangles := PackedInt32Array()
var uvs := PackedVector2Array()

var triangle_index: int = 0

func _init(mesh_size: int) -> void:
	vertices.resize(mesh_size * mesh_size)
#	normals.resize(mesh_size * mesh_size)
	uvs.resize(mesh_size * mesh_size)
	triangles.resize((mesh_size - 1) * (mesh_size - 1) * 6)

func add_triangle(a: int, b: int, c: int) -> void:
	triangles[triangle_index] = a
	triangles[triangle_index + 1] = b
	triangles[triangle_index + 2] = c
	triangle_index += 3

func create_mesh() -> ArrayMesh:
	var mesh := ArrayMesh.new()
	
	var arrays = []
	arrays.resize(ArrayMesh.ARRAY_MAX)
	
	arrays[ArrayMesh.ARRAY_VERTEX] = vertices
#	arrays[ArrayMesh.ARRAY_NORMAL] = normals
	arrays[ArrayMesh.ARRAY_INDEX] = triangles
	arrays[ArrayMesh.ARRAY_TEX_UV] = uvs
	
	mesh.add_surface_from_arrays(Mesh.PRIMITIVE_TRIANGLES, arrays)
	
#	# TEMPORARY
#	var surface_tool := SurfaceTool.new()
#	surface_tool.create_from(mesh, 0)
#	surface_tool.generate_normals()
#	mesh = surface_tool.commit()
	
	return mesh
