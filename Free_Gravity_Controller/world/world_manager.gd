extends Node3D


func _ready():
	pass
#	var start_offset = Vector3(0, 20, 0)
#	var step = Vector3(20, 20, 20)
#
#	var msps := load("res://entities/station_modules/test_module/test_module_movable.scn") as PackedScene
#
#	var level_spatial: Node3D = $Level
#
#	for x in range(3):
#		for y in range(3):
#			for z in range(3):
#				var msm = msps.instantiate() as MovableStationModule
#				msm.position = Vector3(x, y, z) * step + start_offset
#				msm.linear_velocity = Vector3(
#					randf_range(-1.0, 1.0),
#					randf_range(-1.0, 1.0),
#					randf_range(-1.0, 1.0)
#				) * 0.1
#				msm.angular_velocity = Vector3(
#					randf_range(-1.0, 1.0),
#					randf_range(-1.0, 1.0),
#					randf_range(-1.0, 1.0)
#				)
#				level_spatial.add_child(msm)
#
#
