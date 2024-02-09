extends Node2D

# list of spawn info classes with spawn info
@export var enabled = true
@export var maxEnemies:int = 250
@export var spawns:Array[Spawn_info] = []

var time = 0

# used for finding view port sides for some reason. 
@onready var player = get_tree().get_first_node_in_group("player")

# Called once per second, auto starts with game
func _on_timer_timeout():
	time += 1
	var enemy_spawns = spawns

	# itterates through the spawn array and checks if the time is between the start and end time of the spawn
	for spawn in enemy_spawns:
		if time >= spawn.wave_time_start and time <= spawn.wave_time_end and enabled:

			# wait for spawn delay
			if spawn.spawn_delay_counter < spawn.enemy_spawn_delay:
				spawn.spawn_delay_counter += 1
			
			# spawn enemies if enemy count is less than max enemies
			else:
				spawn.spawn_delay_counter = 0
				var new_enemy = spawn.enemy
				var counter = 0

				# spawns the number of enemies in the spawn info
				if get_child_count() < maxEnemies and new_enemy != null:
					while  counter < spawn.enemies_per_delay:
						var enemy_spawn = new_enemy.instantiate()
						
						# set enemy position and stats according to the spawn info
						enemy_spawn.global_position = get_random_position_outside_viewport()
						enemy_spawn.health = spawn.enemy_health
						enemy_spawn.speed = spawn.enemy_speed
						enemy_spawn.damage = spawn.enemy_damage
						enemy_spawn.xpGain = spawn.xp_reward
						enemy_spawn.size = spawn.enemy_size
						enemy_spawn.color = spawn.enemy_color
						
						# add enemy to the scene
						add_child(enemy_spawn)
						counter += 1


# returns a random position just outside of the viewport
func get_random_position_outside_viewport():
	
	# find the viewport size and assign each corner to a variable
	var vpr = get_viewport_rect().size * randf_range(1.1,1.4)
	var top_left = Vector2(player.global_position.x - vpr.x/2, player.global_position.y - vpr.y/2)
	var top_right = Vector2(player.global_position.x + vpr.x/2, player.global_position.y - vpr.y/2)
	var bottom_left = Vector2(player.global_position.x - vpr.x/2, player.global_position.y + vpr.y/2)
	var bottom_right = Vector2(player.global_position.x + vpr.x/2, player.global_position.y + vpr.y/2)

	# pick a random side to spawn the enemy
	var pos_side = ["up","down","right","left"].pick_random()
	var spawn_pos1 = Vector2.ZERO
	var spawn_pos2 = Vector2.ZERO
	
	match pos_side:
		"up":
			spawn_pos1 = top_left
			spawn_pos2 = top_right
		"down":
			spawn_pos1 = bottom_left
			spawn_pos2 = bottom_right
		"right":
			spawn_pos1 = top_right
			spawn_pos2 = bottom_right
		"left":
			spawn_pos1 = top_left
			spawn_pos2 = bottom_left
	
	# return a random position on the chosen side
	var x_spawn = randf_range(spawn_pos1.x, spawn_pos2.x)
	var y_spawn = randf_range(spawn_pos1.y,spawn_pos2.y)

	return Vector2(x_spawn,y_spawn)


func _on_console_info_timer_timeout():
	# print the name of this node and the number of children it has
	var children = get_child_count() - 2
	if children > 0:
		print(get_name(), ": ", children)

