extends Node


## EventBus autoload for entire game
## 
## This class holds signals needed for various objects in the game
## to keep low-coupled architecture


@warning_ignore("unused_signal")
signal spawn_ping_signal(
	world_pos: Vector3,
	text: String,
	color: Color,
	time: float
)
