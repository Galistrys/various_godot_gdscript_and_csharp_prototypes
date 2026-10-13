class_name Constants
extends Object


## Class holding various constants.
## 
## Holds easy accessible information for various of scripts.


## Provides common 3D Physics layers names
enum CollisionLayer {
	GENERAL = 1,
	ENTITY = 2,
	INTERACTABLE = 4,
	ENV_ZONE = 8,
	GRAVITY_ZONE = 16
}


func _init() -> void:
	assert(false, "Created an instance of class Constants")
