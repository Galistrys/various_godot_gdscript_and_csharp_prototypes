extends Spatial

var test = Vector2.ZERO

func _ready():
	var vec = Vector2.ZERO

	print(vec.linear_interpolate(Vector2.ONE, 0.25))
	print(vec.linear_interpolate(Vector2.ONE, 0.5))
	print(vec.linear_interpolate(Vector2.ONE, 0.75))
	print(vec.linear_interpolate(Vector2.ONE, 1))

	print(vec.move_toward(Vector2.ONE, 0.25))
	print(vec.move_toward(Vector2.ONE, 0.5))
	print(vec.move_toward(Vector2.ONE, 0.75))
	print(vec.move_toward(Vector2.ONE, 1))

func _physics_process(_delta):
	pass


func _process(delta):
	print(test)
	test = test.linear_interpolate(Vector2.ONE, delta)
