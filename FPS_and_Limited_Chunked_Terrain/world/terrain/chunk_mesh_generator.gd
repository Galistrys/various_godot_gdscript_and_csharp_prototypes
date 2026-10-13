class_name ChunkMeshGenerator
extends Object


static func coords_to_index(x: int, y: int, chunk_size: int) -> int:
	return y * chunk_size + x

static func generate_chunk_mesh(
	chunk_size: int,
	detail_level: int,
	height_data: PackedFloat32Array
) -> ChunkMeshData:
#	var mesh_simpl_incr := 1 if detail_level == 0 else detail_level * 2
	var mesh_simpl_incr := pow(2, detail_level)
	
	var verticies_per_line := int((chunk_size - 1) / mesh_simpl_incr + 1)
	
	var mesh_data := ChunkMeshData.new(verticies_per_line)
	var vertex_index := 0
	
	for y in range(0, chunk_size, mesh_simpl_incr):
		for x in range(0, chunk_size, mesh_simpl_incr):
			var h_index: int = coords_to_index(
				x, y, chunk_size
			)
			
			mesh_data.vertices[vertex_index] = Vector3(
				x, height_data[h_index] * Constants.HEIGHT_MULTIPLIER, y
			)
			
#			mesh_data.normals[vertex_index] = mesh_data.vertices[vertex_index].normalized()
			
			mesh_data.uvs[vertex_index] = Vector2(
				x / float(chunk_size), y / float(chunk_size)
			)
			
			if x < chunk_size - 1 and y < chunk_size - 1:
				mesh_data.add_triangle(
					vertex_index,
					vertex_index + verticies_per_line + 1,
					vertex_index + verticies_per_line
				)
				mesh_data.add_triangle(
					vertex_index + verticies_per_line + 1,
					vertex_index,
					vertex_index + 1
				)
			
			vertex_index += 1
	
	return mesh_data
#	for i in range(0, mesh_data.triangle_index, 6):
#		var vert_idx = mesh_data.triangles[i]
#		var vec_1 = mesh_data.vertices[vert_idx]
#		var vec_2 = mesh_data.vertices[mesh_data.triangles[i + 1]]
#		var vec_3 = mesh_data.vertices[mesh_data.triangles[i + 2]]
#
#		var normal = (vec_3 - vec_1).cross(vec_2 - vec_1)
#		mesh_data.normals[vert_idx] = normal
#
#		vert_idx = mesh_data.triangles[i + 4]
#		vec_1 = mesh_data.vertices[mesh_data.triangles[i + 3]]
#		vec_2 = mesh_data.vertices[vert_idx]
#		vec_3 = mesh_data.vertices[mesh_data.triangles[i + 5]]
#
#		normal = (vec_3 - vec_1).cross(vec_2 - vec_1)
#		mesh_data.normals[vert_idx] = normal
#
#	return mesh_data
