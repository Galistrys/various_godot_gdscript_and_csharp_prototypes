extends Node

func _ready():
#	load_texture("res://Resources/Data/Diaries/Authors/TEST_AUTHOR.png", "res://Resources/Data/Diaries/Authors/TEST_AUTHOR.tex")
#	load_sound("res://Resources/Data/Diaries/Data/TestAudioDiary/TestAudioDiary.ogg", "res://Resources/Data/Diaries/Data/TestAudioDiary/TestAudioDiary.oggstr")
	print($AnimationPlayer.get_animation($AnimationPlayer.current_animation).track_get_key_value(0, 0))

func load_texture(path, dest):
	var stex : StreamTexture = load(path)
	var itex = ImageTexture.new()
	itex.create_from_image(stex.get_data())
	print(ResourceSaver.save(dest, itex))

func load_sound(path, dest):
	var ass : AudioStreamOGGVorbis = load(path)
	print(ResourceSaver.save(dest, ass))
