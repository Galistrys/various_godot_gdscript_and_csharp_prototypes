class_name ExteriorCamera
extends Camera3D


var _tracked_camera: Camera3D


@warning_ignore("unused_parameter")
func _process(delta: float) -> void:
	if _tracked_camera != null:
		transform = _tracked_camera.global_transform


func set_tracked_camera(camera: Camera3D) -> void:
	_tracked_camera = camera
