class_name ChunkShapeGenerator
extends Object


static func create_physics_shape(
	mesh_data: ChunkMeshData
) -> ConcavePolygonShape3D:
	var vertices := mesh_data.vertices
	var indices := mesh_data.triangles
	
	var result_array := PackedVector3Array()
	result_array.resize(indices.size())
	for i in range(indices.size()):
		result_array[i] = vertices[indices[i]]
	
	var shape := ConcavePolygonShape3D.new()
	shape.set_faces(result_array)
	return shape
