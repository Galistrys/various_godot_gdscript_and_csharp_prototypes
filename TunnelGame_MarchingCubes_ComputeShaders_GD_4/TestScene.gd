extends Node

const FLOAT_SIZE = 4
const VEC_SIZE = 4
const TRI_SIZE = 4
const TRI_NUM = 5

func _ready():
#	get_tree().root.debug_draw = Viewport.DEBUG_DRAW_WIREFRAME
	
	var values_data: PackedFloat32Array = PackedFloat32Array()
	values_data.resize(ChunkMeshGenerator.CHUNK_SIZE_1_3)
	
	for x in range(0, ChunkMeshGenerator.CHUNK_SIZE_1):
		for y in range(0, ChunkMeshGenerator.CHUNK_SIZE_1):
			for z in range(0, ChunkMeshGenerator.CHUNK_SIZE_1):
				var idx: int = ChunkMeshGenerator.CHUNK_SIZE_1_2 * z \
					+ ChunkMeshGenerator.CHUNK_SIZE_1 * y + x
				values_data[idx] = WorldMapData.get_value(Vector3(x, y, z))
#				values_data[idx] = idx % 2
#				values_data[idx] = 1
#				values_data[idx] = 1.0 if x == 0 else 0.0
	
#	var points_to_add = [
#		Vector3(0, 0, 0),
#		Vector3(2, 0, 0),
#		Vector3(4, 0, 0),
#		Vector3(6, 0, 0),
#		Vector3(8, 0, 0),
#		Vector3(10, 0, 0),
#		Vector3(12, 0, 0),
#		Vector3(14, 0, 0),
#		Vector3(0, 0, 1),
#		Vector3(2, 0, 1),
#		Vector3(4, 0, 1),
#		Vector3(6, 0, 1),
#		Vector3(8, 0, 1),
#		Vector3(10, 0, 1),
#		Vector3(12, 0, 1),
#		Vector3(14, 0, 1),
#	]
##
#	for p_t_a in points_to_add:
#		var idx: int = ChunkMeshGenerator.CHUNK_SIZE_1_2 * p_t_a.z \
#			+ ChunkMeshGenerator.CHUNK_SIZE_1 * p_t_a.y + p_t_a.x
#		values_data[idx] = 1
	
	var triangles_data: PackedFloat32Array = PackedFloat32Array()
	triangles_data.resize(
		ChunkMeshGenerator.CHUNK_SIZE_3 \
		* TRI_NUM * TRI_SIZE * VEC_SIZE
	)
	for i in range(triangles_data.size()):
		triangles_data[i] = -1.0
	
	
	# We will be using our own RenderingDevice to handle the compute commands
	var rd := RenderingServer.create_local_rendering_device()

	# Create shader and pipeline
#	var shader_file: RDShaderFile = load("res://Tools/Compute/Test.glsl") as RDShaderFile
	var shader_file: RDShaderFile = load("res://Tools/Compute/MarchingCubes.glsl") as RDShaderFile
	var shader_spirv: RDShaderSPIRV = shader_file.get_spirv()
	var shader: RID = rd.shader_create_from_spirv(shader_spirv)
	var pipeline: RID = rd.compute_pipeline_create(shader)

	# Data for compute shaders has to come as an array of bytes
	var tri_data_pba: PackedByteArray = triangles_data.to_byte_array()
	var values_data_pba: PackedByteArray = values_data.to_byte_array()

	# Create storage buffer
	# Data not needed, can just create with length
#	var storage_buffer := rd.storage_buffer_create(64, pba)
	var tri_data_sb: RID = rd.storage_buffer_create(
		tri_data_pba.size(), tri_data_pba
	)
	var values_data_sb: RID = rd.storage_buffer_create(
		values_data_pba.size(), values_data_pba
	)

	# Create uniform set using the storage buffer
#	var u := RDUniform.new()
#	u.uniform_type = RenderingDevice.UNIFORM_TYPE_STORAGE_BUFFER
#	u.binding = 0
#	u.add_id(storage_buffer)
#	var uniform_set := rd.uniform_set_create([u], shader, 0)
	var tri_u := RDUniform.new()
	tri_u.uniform_type = RenderingDevice.UNIFORM_TYPE_STORAGE_BUFFER
	tri_u.binding = 0
	tri_u.add_id(tri_data_sb)
#	var tri_u_set: RID = rd.uniform_set_create([tri_u], shader, 0)
	
	var vd_u := RDUniform.new()
	vd_u.uniform_type = RenderingDevice.UNIFORM_TYPE_STORAGE_BUFFER
	vd_u.binding = 1
	vd_u.add_id(values_data_sb)
#	var vd_u_set: RID = rd.uniform_set_create([vd_u], shader, 0)

	var uniform_set: RID = rd.uniform_set_create([tri_u, vd_u], shader, 0)

	# Start compute list to start recording our compute commands
	var compute_list: int = rd.compute_list_begin()
	# Bind the pipeline, this tells the GPU what shader to use
	rd.compute_list_bind_compute_pipeline(compute_list, pipeline)
	# Binds the uniform set with the data we want to give our shader
	rd.compute_list_bind_uniform_set(compute_list, uniform_set, 0)
	# Dispatch 1x1x1 (XxYxZ) work groups
	rd.compute_list_dispatch(compute_list, 2, 2, 2)
	#rd.compute_list_add_barrier(compute_list)
	# Tell the GPU we are done with this compute task
	rd.compute_list_end()
	
	var start: int = Time.get_ticks_usec()
	
	# Force the GPU to start our commands
	rd.submit()
	# Force the CPU to wait for the GPU to finish with the recorded commands
	rd.sync()
	
	var end: int = Time.get_ticks_usec()
	
	print("Time: ", float(end - start) / 1000, " ms")
	
	# Now we can grab our data from the storage buffer
#	var byte_data := rd.buffer_get_data(storage_buffer)
#	for i in range(16):
#		print(byte_data.decode_float(i*4))
	var tri_bd = rd.buffer_get_data(tri_data_sb)
	var tri_new_data: PackedFloat32Array = tri_bd.to_float32_array()
	var vectors_data: Array = []
	var normals_data: Array = []
	
	var file: File = File.new()
	file.open("res://testf.txt", File.WRITE)
#	file.store_buffer(tri_bd)
	for i in range(tri_new_data.size()):
		file.store_string(str(tri_new_data[i]))
		file.store_string(", ")

		if i % 16 == 15:
			file.store_string("\n")
	file.close()
#	return
	
	var row_step: int = TRI_NUM * TRI_SIZE * VEC_SIZE
	var tri_step = TRI_SIZE * VEC_SIZE
	for i in range(0, tri_new_data.size(), row_step):
		for j in range(0, row_step, tri_step):
			var index: int = i + j
			if tri_new_data[index] != -1.0:
				var vec1 := Vector3(
					tri_new_data[index + 0],
					tri_new_data[index + 1],
					tri_new_data[index + 2]
				)
				var vec2 := Vector3(
					tri_new_data[index + 4],
					tri_new_data[index + 5],
					tri_new_data[index + 6]
				)
				var vec3 := Vector3(
					tri_new_data[index + 8],
					tri_new_data[index + 9],
					tri_new_data[index + 10]
				)
				var normal := Vector3(
					tri_new_data[index + 12],
					tri_new_data[index + 13],
					tri_new_data[index + 14]
				)
				vectors_data.append(vec1)
				vectors_data.append(vec2)
				vectors_data.append(vec3)
				
				normals_data.append(normal)
				normals_data.append(normal)
				normals_data.append(normal)
	
#	print("VECTORS: ", vectors_data)
	
	var filef := File.new()
	filef.open("res://test.txt", File.WRITE)
	for i in range(vectors_data.size()):
		var cur_vec: Vector3 = vectors_data[i]
		filef.store_string(str(cur_vec))
		filef.store_string(", ")
		if i % 3 == 2:
			filef.store_string("\n")
	filef.close()
	
	if vectors_data.is_empty():
		print("EMPTY")
	else:
		var mesh = ChunkMeshGenerator.generate_mesh(vectors_data, normals_data)
		
		start = Time.get_ticks_usec()
		var v_data = ChunkMeshGenerator.generate_verticies(
			values_data, 1, 0.5
		)
		var n_data = ChunkMeshGenerator.generate_normals(
			v_data
		)
		end = Time.get_ticks_usec()
		
		print("Time: ", float(end - start) / 1000, " ms")
		
		var s_mesh = ChunkMeshGenerator.generate_mesh(
			v_data, n_data
		)
		
		var mi: MeshInstance3D = MeshInstance3D.new()
		mi.mesh = mesh
		mi.position = Vector3(-8, -8, -8)
		
		var mat = StandardMaterial3D.new()
		mat.albedo_color = Color.YELLOW
		mi.material_override = mat
		
		var mi_s: MeshInstance3D = MeshInstance3D.new()
		mi_s.mesh = s_mesh
		mi_s.position = Vector3(-8, -8, -8)
		
		var s_mat = StandardMaterial3D.new()
		s_mat.albedo_color = Color.RED
		mi_s.material_override = s_mat
		
		add_child(mi)
#		add_child(mi_s)
	
#	print("NORMALS: ", normals_data)
#	print("#####################################################")
#	print("VECTORS: ", ChunkMeshGenerator.generate_verticies(
#		values_data, 1, 0.5
#	))
	
#	print(tri_new_data)

#@onready var tex_rect: TextureRect = $TexRect
#var _noise: FastNoiseLite = preload("res://Tunnel/RoomNoise.res")
#
#const POW = log(0.05) / log(0.5)
#const CORRIDOR_POW = log(0.001) / log(0.8)
#
#func bias_function(
#	x: int, y: int, radius: float
#):
#	var distance = clampf(
#		Vector2(x, y).distance_to(Vector2.ZERO) / radius,
#		0.0,
#		1.0
#	)
#
#	return pow(distance, POW)
#
#func _ready():
#	randomize()
#	_noise.seed = randi()
#
#	var radius = 16
#	var center_x = 16
#	var center_y = 16
#
#	var image = Image.new()
#	image.create(33, 33, false, Image.FORMAT_RGB8)
#
#	for x in range (center_x - radius, center_x + radius + 1):
#		for y in range (center_y - radius, center_y + radius + 1):
#			var d_x = x - center_x
#			var d_y = y - center_y
#
#			var noise_val = range_lerp(_noise.get_noise_2d(x, y), -0.65, 0.65, 0, 1)
#			var bias_val = bias_function(d_x, d_y, radius)
#
#			if bias_val > noise_val:
#				image.set_pixel(x, y, Color.BLACK)
#			else:
#				image.set_pixel(x, y, Color.WHITE)
#
#	var it = ImageTexture.new()
#	it.set_image(image)
#	tex_rect.texture = it

#func _ready():
#	print((Vector3(-5, -5, 23) / 16).floor())
#
#var _data: PackedInt32Array = PackedInt32Array()


#func _ready():
#	_data.resize(64)
#
#	var group_idx = WorkerThreadPool.add_group_task(
#		Callable(self, "worker_func").bind("Truskawka", 23.0),
#		_data.size(), 2
#	)
#	print(group_idx)
#
#	WorkerThreadPool.wait_for_group_task_completion(group_idx)
#
#	group_idx = WorkerThreadPool.add_group_task(
#		Callable(self, "worker_func").bind("Truskawka", 23.0),
#		_data.size(), 2
#	)
#	print(group_idx)
#
#	WorkerThreadPool.wait_for_group_task_completion(group_idx)
#
#	print(_data)
#
#func worker_func(id: int, text: String, flt: float):
#	print(id, " - ", text, " - ", flt)
#	_data[id] = id
#
#func _ready():
#	var values_data: PackedFloat32Array = PackedFloat32Array()
#	values_data.resize(ChunkMeshGenerator.CHUNK_SIZE_1_3)
#
#	for x in range(0, ChunkMeshGenerator.CHUNK_SIZE_1):
#		for y in range(0, ChunkMeshGenerator.CHUNK_SIZE_1):
#			for z in range(0, ChunkMeshGenerator.CHUNK_SIZE_1):
#				var idx: int = ChunkMeshGenerator.CHUNK_SIZE_1_2 * z + ChunkMeshGenerator.CHUNK_SIZE_1 * y + x
#				values_data[idx] = WorldMapData.get_value(Vector3(x, y, z))
#
##	values_data: PackedFloat32Array,
##	chunk_size: int,
##	detail_level: int,
##	surface_level: float
#
#	var verts: PackedVector3Array = ChunkMeshGenerator.generate_verticies(
#		values_data, 1, 0.5
#	)
#	var normals: PackedVector3Array = ChunkMeshGenerator.generate_normals(verts)
#
#	var mesh: ArrayMesh = ChunkMeshGenerator.generate_mesh(
#		verts, normals
#	)
#
#	var mi = MeshInstance3D.new()
#	mi.mesh = mesh
#	add_child(mi)
