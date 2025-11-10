extends Node3D

@onready var sky: Sky = $"WorldEnvironment".environment.sky
@onready var water := $"Water"
@onready var camera := $"Camera3D"

func _ready() -> void:
	sky.sky_material.set_shader_parameter("coverage", 0)
	camera.rotation = Vector3.ZERO
	camera.fov = 90
	
	var echoTween := create_tween().set_trans(Tween.TRANS_LINEAR)
	echoTween.tween_method(func(echo: float): water.mesh.material.set_shader_parameter("echo", echo), 0.0, 0.5, 12)
	
	var introTween := create_tween().set_trans(Tween.TRANS_QUART).set_ease(Tween.EASE_OUT).set_parallel()
	introTween.tween_property(camera, "rotation", Vector3(deg_to_rad(15), 0, 0), 5)
	introTween.tween_property(camera, "fov", 70, 5)
	introTween.set_trans(Tween.TRANS_LINEAR)
	introTween.tween_method(func(coverage: float): sky.sky_material.set_shader_parameter("coverage", coverage), 0.0, 1.0, 8)
