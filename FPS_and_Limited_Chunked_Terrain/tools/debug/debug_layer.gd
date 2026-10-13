extends CanvasLayer

@onready var _rich_text_label := $Control/RichTextLabel as RichTextLabel
const FORMAT_STRING = """FPS: {fps}
Process time: {process_time}
Physics process time: {physics_process_time}
Memory static: {memory_static}
Object count: {object_count}
"""

func _process(_delta):
	_rich_text_label.text = FORMAT_STRING.format(
		{
			"fps": Performance.get_monitor(Performance.TIME_FPS),
			"process_time": Performance.get_monitor(Performance.TIME_PROCESS),
			"physics_process_time": Performance.get_monitor(Performance.TIME_PHYSICS_PROCESS),
			"memory_static": Performance.get_monitor(Performance.MEMORY_STATIC),
			"object_count": Performance.get_monitor(Performance.OBJECT_COUNT)
		}
	)
