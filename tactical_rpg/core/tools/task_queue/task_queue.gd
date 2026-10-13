class_name TaskQueue
extends Node


## Provides mechanism to process queue of tasks


var _queue: Array[Task] = []
var _processing_flag := false


## Add tasks to the queue.
## Calls [param p_task] as async method, and after it finishes, calls [param p_callback]
func add_task(
		p_task: Callable,
		p_callback: Callable = Callable(),
		p_at_front: bool = false
) -> void:
	var task := Task.new(p_task, p_callback)
	if p_at_front:
		_queue.push_front(task)
	else:
		_queue.push_back(task)


func clear_tasks() -> void:
	_queue.clear()


func _process_task() -> void:
	if _processing_flag or _queue.size() == 0:
		return
	
	_processing_flag = true
	
	var task := _queue[0]
	_queue.pop_front()
	
	await task.task.call()
	await task.callback.call()
	
	_processing_flag = false


class Task extends Object:
	var task: Callable
	var callback: Callable
	
	func _init(p_task: Callable, p_callback: Callable) -> void:
		task = p_task
		callback = p_callback
