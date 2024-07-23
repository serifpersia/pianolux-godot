extends Node3D

@export var webcam_scene: PackedScene

var webcam_instance: Node = null

func _on_webcam_toggle_button_toggled(toggled_on):
	if toggled_on:
		if webcam_instance == null:
			webcam_instance = webcam_scene.instantiate()
			add_child(webcam_instance)
		webcam_instance.visible = true
	else:
		if webcam_instance != null:
			if has_node(webcam_instance.get_path()):
				webcam_instance.visible = false
				webcam_instance.queue_free()
				webcam_instance = null

