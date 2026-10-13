class_name ClipmapTerrain
extends Node3D


@onready var _model_mesh_instance := $Model as MeshInstance3D


func _ready() -> void:
	var terrain_mesh_generator := TerrainMeshGenerator.new(
		64,
		[1, 1, 2, 4, 8, 16]
#		4,
#		[1, 2]
	)
	_model_mesh_instance.mesh = terrain_mesh_generator.generate_mesh()
