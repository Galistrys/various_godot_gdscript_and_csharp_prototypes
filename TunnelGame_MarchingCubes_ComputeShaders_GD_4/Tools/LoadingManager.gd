extends Node

var _thread = Thread.new()

const ROOMS_COUNT = 128

const MIN_ROOM_RADIUS = 6
const MAX_ROOM_RADIUS = 12

const MIN_ROOM_SPHERES = 3
const MAX_ROOM_SPHERES = 8

const DECREASE_FACTOR = 0.95

const MAX_DEPTH = 128

const CORRIDOR_SIZE = 4

const POW = log(0.05) / log(0.5)

@onready var load_progress_bar: ProgressBar = %LoadProgressBar
@onready var stage_label: Label = %StageLabel

var _noise: FastNoiseLite = preload("res://Tunnel/RoomNoise.res")

class Graph:
	var n: int
	var graph: Array
	
	func _init(num_of_verts: int):
		n = num_of_verts
		graph = []
	
	func add_edge(u: int, v: int, w: float):
		graph.append([u, v, w])
	
	func find(parent: Array[int], i: int) -> int:
		if parent[i] == i:
			return i
		return find(parent, parent[i])
	
	func union(parent: Array[int], rank: Array[int], x: int, y: int):
		var xroot = find(parent, x)
		var yroot = find(parent, y)
		
		if rank[xroot] < rank[yroot]:
			parent[xroot] = yroot
		elif rank[xroot] > rank[yroot]:
			parent[yroot] = xroot
		else:
			parent[yroot] = xroot
			rank[xroot] += 1
	
	func KruskalMST():
		var result: Array = []
		var i: int = 0
		var e: int = 0
		
		graph.sort_custom(func(a, b): return a[2] < b[2])
		
		var parent: Array[int] = []
		var rank: Array[int] = []
		
		for node in range(n):
			parent.append(node)
			rank.append(0)
		
		while e < n - 1:
			var u: int = graph[i][0]
			var v: int = graph[i][1]
			var w: int = graph[i][2]
			i += 1
			var x: int = find(parent, u)
			var y: int = find(parent, v)
			
			if x != y:
				e += 1
				result.append([u, v, w])
				union(parent, rank, x, y)
		
		return result

func bresenham_3d(
	x1: int, y1: int, z1: int,
	x2: int, y2: int, z2: int
) -> Array[Vector3i]:
	var list_of_points: Array[Vector3i] = []
	
	list_of_points.append(Vector3i(x1, y1, z1))
	var dx: int = abs(x2 - x1)
	var dy: int = abs(y2 - y1)
	var dz: int = abs(z2 - z1)
	
	var xs: int = 0
	var ys: int = 0
	var zs: int = 0
	
	if x2 > x1:
		xs = 1
	else:
		xs = -1
	
	if y2 > y1:
		ys = 1
	else:
		ys = -1
	
	if z2 > z1:
		zs = 1
	else:
		zs = -1
	
	var p1: int = 0
	var p2: int = 0
	
	if dx >= dy and dx >= dz:
		p1 = 2 * dy - dx
		p2 = 2 * dz - dx
		
		while x1 != x2:
			x1 += xs
			if p1 >= 0:
				y1 += ys
				p1 -= 2 * dx
			if p2 >= 0:
				z1 += zs
				p2 -= 2 * dx
			p1 += 2 * dy
			p2 += 2 * dz
			list_of_points.append(Vector3i(x1, y1, z1))
	
	elif dy >= dx and dy >= dz:       
		p1 = 2 * dx - dy
		p2 = 2 * dz - dy
		while y1 != y2:
			y1 += ys
			if p1 >= 0:
				x1 += xs
				p1 -= 2 * dy
			if p2 >= 0:
				z1 += zs
				p2 -= 2 * dy
			p1 += 2 * dx
			p2 += 2 * dz
			list_of_points.append(Vector3i(x1, y1, z1))
	
	else:
		p1 = 2 * dy - dz
		p2 = 2 * dx - dz
		while z1 != z2:
			z1 += zs
			if p1 >= 0:
				y1 += ys
				p1 -= 2 * dz
			if p2 >= 0:
				x1 += xs
				p2 -= 2 * dz
			p1 += 2 * dy
			p2 += 2 * dx
			list_of_points.append(Vector3i(x1, y1, z1))
	
	return list_of_points

func _ready():
	_thread.start(Callable(self, "_thread_generate"))

func _thread_generate() -> void:
	# Create POINTS
	stage_label.set_deferred("text", "Creating points")
	
	var rooms: Array[Vector3i] = []
	rooms.resize(ROOMS_COUNT)
	
	seed(0)
	
	rooms[0] = Vector3i(0, 0, 0)
	for i in range(1, rooms.size()):
		rooms[i] = Vector3i(
			randi() % WorldMapData.map_size_in_tiles,
			randi() % WorldMapData.map_size_in_tiles,
			randi() % WorldMapData.map_size_in_tiles
		)
		
	# Create POINTS
	
	# Create MST
	stage_label.set_deferred("text", "Creating MST")
	
	var g = Graph.new(ROOMS_COUNT)
	
	for i in range(rooms.size()):
		var u: Vector3i = rooms[i]
		var u_f := Vector3(u.x, u.y, u.z)
		for j in range(i + 1, rooms.size()):
			var v: Vector3i = rooms[j]
			var v_f := Vector3(v.x, v.y, v.z)
			g.add_edge(i, j, u_f.distance_squared_to(v_f))
	
	var corridors: Array = g.KruskalMST()
	
	# Create MST
	
	# Drill caves
	stage_label.set_deferred("text", "Drilling caves")
	for i in range(rooms.size()):
		prints(float(i) / rooms.size() * 100.0, "%")
		load_progress_bar.value = float(i) / rooms.size() * 100.0
		
		var room_origin = rooms[i]
		
		var spheres: Array = []
		for j in range (randi_range(MIN_ROOM_SPHERES, MAX_ROOM_SPHERES)):
			@warning_ignore(integer_division)
			var mrr_2 = MIN_ROOM_RADIUS
			spheres.append(
				room_origin + Vector3i(
					randi_range(-mrr_2, mrr_2),
					randi_range(-mrr_2, mrr_2),
					randi_range(-mrr_2, mrr_2)
				)
			)
		
		_drill_around(
				room_origin.x, room_origin.y, room_origin.z,
				randi_range(MIN_ROOM_RADIUS*2, MAX_ROOM_RADIUS), POW
			)
		
		for sphere in spheres:
			_drill_around(
				sphere.x, sphere.y, sphere.z,
				randi_range(MIN_ROOM_RADIUS, MAX_ROOM_RADIUS), POW
			)
	load_progress_bar.value = 100.0
	# Drill caves
	
	# Drill corridors
	stage_label.set_deferred("text", "Drilling corridors")
	for i in range(corridors.size()):
		prints(float(i) / corridors.size() * 100.0, "%")
		load_progress_bar.value = float(i) / corridors.size() * 100.0
		
		var corridor: Array = corridors[i]
		var corridor_origin: Vector3i = rooms[corridor[0]]
		var corridor_end: Vector3i = rooms[corridor[1]]
		
		var points := bresenham_3d(
			corridor_origin.x, corridor_origin.y, corridor_origin.z,
			corridor_end.x, corridor_end.y, corridor_end.z
		)
		
		for point in points:
			_drill_around(
				point.x, point.y, point.z,
				CORRIDOR_SIZE + randi_range(-1, 1), POW
			)
	
	load_progress_bar.value = 100.0
	# Drill corridors
	
	stage_label.set_deferred("text", "Finished!")
	call_deferred("_thread_finished")

func bias_function(
	x: int, y: int, z: int, radius: float, m_pow: float
):
	var distance = clampf(
		Vector3(x, y, z).distance_to(Vector3.ZERO) / radius,
		0.0,
		1.0
	)
	
	return pow(distance, m_pow)

func _drill_around(
	x: int, y: int, z: int,
	radius: int, m_pow: float
):
	for x_i in range (x - radius, x + radius + 1):
		for y_i in range (y - radius, y + radius + 1):
			for z_i in range (z - radius, z + radius + 1):
				if x_i < 0 or y_i < 0 or z_i < 0 \
					or x_i > WorldMapData.map_size_in_tiles - 1 \
					or y_i > WorldMapData.map_size_in_tiles - 1 \
					or z_i > WorldMapData.map_size_in_tiles - 1:
					pass
				else:
					var d_x = x_i - x
					var d_y = y_i - y
					var d_z = z_i - z
					
					var noise_val = remap(_noise.get_noise_3d(x_i, y_i, z_i), -0.65, 0.65, 0, 1)
					var bias_fun = bias_function(d_x, d_y, d_z, radius, m_pow)
					
					if bias_fun < noise_val:
						WorldMapData.set_map_data(x_i, y_i, z_i, 0.0)

func _thread_finished() -> void:
	_thread.wait_to_finish()
	print("THREAD FINISHED")
	await get_tree().create_timer(1).timeout
	get_tree().change_scene_to_file("res://Scenes/Game.scn")
