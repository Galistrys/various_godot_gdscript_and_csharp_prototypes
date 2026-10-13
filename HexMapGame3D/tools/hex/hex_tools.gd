class_name HexTools
extends Object

const DEG_30_TO_RAD = deg_to_rad(30)
const DEG_60_TO_RAD = deg_to_rad(60)

const ONE_THIRD = 1.0 / 3.0
const TWO_THIRDS = 2.0 / 3.0
const SQRT_3 = sqrt(3)
const SQRT_3_D_2 = sqrt(3) / 2.0
const SQRT_3_D_3 = sqrt(3) / 3.0

const E = 0
const NE = 1
const NW = 2
const W = 3
const SW = 4
const SE = 5

const ODD_ROWS_DIFF = 6

#  NW NE
# W     E
#  SW SE

const CUBE_DIRECTION_VECTORS: Array[Vector3i] = [
	Vector3i(1, 0, -1), Vector3i(1, -1, 0), Vector3i(0, -1, 1),
	Vector3i(-1, 0, 1), Vector3i(-1, 1, 0), Vector3i(0, 1, -1)
]

const AXIAL_DIRECTION_VECTORS: Array[Vector2i] = [
	Vector2i(1, 0), Vector2i(1, -1), Vector2i(0, -1),
	Vector2i(-1, 0), Vector2i(-1, 1), Vector2i(0, 1)
]

const HEX_DIRECTION_DIFFERENCES: Array[Vector2i] = [
	Vector2i(1, 0), Vector2i(0, -1), Vector2i(-1, -1),
	Vector2i(-1, 0), Vector2i(-1, 1), Vector2i(0, 1),
	Vector2i(1, 0), Vector2i(1, -1), Vector2i(0, -1),
	Vector2i(-1, 0), Vector2i(0, 1), Vector2i(1, 1)
]


static func hex_corner(center: Vector2, size: float, i: int) -> Vector2:
	var angle := DEG_60_TO_RAD * i - DEG_30_TO_RAD
	return Vector2(
		center.x + size * cos(angle),
		center.y + size * sin(angle)
	)


static func cube_to_axial(cube: Vector3i) -> Vector2i:
	return Vector2i(cube.x, cube.y)


static func axial_to_cube(axial: Vector2i) -> Vector3i:
	return Vector3i(
		axial.x, axial.y, -(axial.x + axial.y)
	)

static func axial_to_cube_f(axial: Vector2) -> Vector3:
	return Vector3(
		axial.x, axial.y, -(axial.x + axial.y)
	)

static func cube_direction(direction: int) -> Vector3i:
	return CUBE_DIRECTION_VECTORS[direction]

static func cube_add(cube: Vector3i, vec: Vector3i) -> Vector3i:
	return cube + vec

static func cube_neighbor(cube: Vector3i, direction: int) -> Vector3i:
	return cube_add(cube, cube_direction(direction))

static func axial_direction(direction: int) -> Vector2i:
	return AXIAL_DIRECTION_VECTORS[direction]

static func axial_add(axial: Vector2i, vec: Vector2i) -> Vector2i:
	return axial + vec

static func axial_neighbor(axial: Vector2i, direction: int) -> Vector2i:
	return axial_add(axial, axial_direction(direction))

static func cube_subtract(a: Vector3i, b: Vector3i) -> Vector3i:
	return a - b

static func cube_distance(a: Vector3i, b: Vector3i) -> int:
	var vec := cube_subtract(a, b)
	@warning_ignore("integer_division")
	return (absi(vec.x) + absi(vec.y) + absi(vec.z)) / 2

static func axial_subtract(a: Vector2i, b: Vector2i) -> Vector2i:
	return a - b

static func axial_distance(a: Vector2i, b: Vector2i) -> int:
	var vec := axial_subtract(a, b)
	@warning_ignore("integer_division")
	return (absi(vec.x) + absi(vec.x + vec.y) + absi(vec.y)) / 2

static func cube_round(frac: Vector3) -> Vector3i:
	var x = roundi(frac.x)
	var y = roundi(frac.y)
	var z = roundi(frac.z)
	
	var x_diff := abs(x - frac.x)
	var y_diff := abs(y - frac.y)
	var z_diff := abs(z - frac.z)
	
	if x_diff > y_diff and x_diff > z_diff:
		x = -(y + z)
	elif y_diff > z_diff:
		y = -(x + z)
	else:
		z = -(x + y)
	
	return Vector3i(x, y, z)


static func axial_round(frac: Vector2) -> Vector2i:
	return cube_to_axial(cube_round(axial_to_cube_f(frac)))
#	var x := roundi(frac.x)
#	var y := roundi(frac.y)
#
#	var x_diff := frac.x - x
#	var y_diff := frac.y - y
#
#	var dx := (roundi(x_diff + 0.5 * y_diff) if x_diff * x_diff >= y_diff * y_diff else 0)
#	var dy := (roundi(y_diff + 0.5 * x_diff) if y_diff * y_diff < x_diff * x_diff else 0)
#
#	return Vector2i(x + dx, y + dy)
	
#static func axial_round(frac: Vector2) -> Vector2i:
#	return cube_to_axial(cube_round(Vector3(
#		frac.x, frac.y, -(frac.x + frac.y)
#	)))


static func cube_linedraw(a: Vector3i, b: Vector3i) -> Array[Vector3i]:
	var n := cube_distance(a, b)
	var results: Array[Vector3i] = []
	for i in range(0, n + 1):
		results.append(cube_round(Vector3(a).lerp(Vector3(b), 1.0 / n * i)))
	return results


static func axial_linedraw(a: Vector2i, b: Vector2i) -> Array[Vector2i]:
	var n := axial_distance(a, b)
	var results: Array[Vector2i] = []
	for i in range(0, n + 1):
		results.append(axial_round(Vector2(a).lerp(Vector2(b), 1.0 / n * i)))
	return results


static func axial_to_world_pos(axial: Vector2i, size: float) -> Vector2:
	var x := size * (SQRT_3 * axial.x + SQRT_3_D_2 * axial.y)
	var y := size * (1.5 * axial.y)
	return Vector2(x, y)


static func world_pos_to_axial(world_pos: Vector2, size: float) -> Vector2i:
	var x := (SQRT_3_D_3 * world_pos.x - ONE_THIRD * world_pos.y) / size
	var y := (TWO_THIRDS * world_pos.y) / size
	return axial_round(Vector2(x, y))


static func axial_to_hex(axial: Vector2i) -> Vector2i:
	@warning_ignore("integer_division")
	var col := axial.x + (axial.y - (axial.y & 1)) / 2
	var row := axial.y
	return Vector2i(col, row)

static func hex_to_axial(hex: Vector2i) -> Vector2i:
	@warning_ignore("integer_division")
	var x := hex.x - (hex.y - (hex.y & 1)) / 2
	var y := hex.y
	return Vector2i(x, y)


static func cube_to_hex(cube: Vector3i) -> Vector2i:
	@warning_ignore("integer_division")
	var col := cube.x + (cube.y - (cube.y & 1)) / 2
	var row := cube.y
	return Vector2i(col, row)

static func hex_to_cube(hex: Vector2i) -> Vector3i:
	@warning_ignore("integer_division")
	var x := hex.x - (hex.y - (hex.y & 1)) / 2
	var y := hex.y
	return Vector3i(x, y, -(x + y))


static func hex_neighbor(hex: Vector2i, direction: int) -> Vector2i:
	var offset := (hex.y & 1) * ODD_ROWS_DIFF
	var diff = HEX_DIRECTION_DIFFERENCES[offset + direction]
	return hex + diff


static func hex_distance(a: Vector2i, b: Vector2i) -> int:
	var ac := hex_to_axial(a)
	var bc := hex_to_axial(b)
	return axial_distance(ac, bc)


static func hex_linedraw(a: Vector2i, b: Vector2i) -> Array[Vector2i]:
	var ca := hex_to_cube(a)
	var cb := hex_to_cube(b)
	
	var c_results := cube_linedraw(ca, cb)
	
	var results: Array[Vector2i] = []
	for cube in c_results:
		results.append(cube_to_hex(cube))
	return results


static func cube_scale(cube: Vector3i, factor: int) -> Vector3i:
	return cube * factor


static func cube_ring(center: Vector3i, radius: int) -> Array[Vector3i]:
	var results: Array[Vector3i] = []
	var cube := cube_add(center, cube_scale(cube_direction(4), radius))
	
	for i in range(0, 6):
		for j in range(0, radius):
			results.append(cube)
			cube = cube_neighbor(cube, i)
	return results


static func hex_to_world_pos(hex: Vector2i, size: float) -> Vector2:
	var x := size * SQRT_3 * (hex.x + 0.5 * (hex.y & 1))
	var y := size * 1.5 * hex.y
	return Vector2(x, y)

static func world_pos_to_hex(world_pos: Vector2, size: float) -> Vector2i:
	return axial_to_hex(world_pos_to_axial(world_pos, size))
