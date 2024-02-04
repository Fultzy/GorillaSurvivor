extends Camera2D

var zoom_min = Vector2(.80001, .80001)
var zoom_max = Vector2(10, 10)
var zoom_speed = Vector2(0.1, 0.1)

func _input(event):
	if event is InputEventMouseButton:
		if event.is_pressed():
			if event.button_index == MOUSE_BUTTON_WHEEL_DOWN:
				if zoom > zoom_min:
					zoom -= zoom_speed
			if event.button_index == MOUSE_BUTTON_WHEEL_UP:
				if zoom < zoom_max:
					zoom += zoom_speed
