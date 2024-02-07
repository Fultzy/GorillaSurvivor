extends Resource

class_name Spawn_info
# this class is meant to house information about an enemy spawn. 
# the variables in this class are put into a list
# this class will be able to spawn similar enemies but in different waves
# with different stats, sizes, and maybe colors? pretty useful. 

@export var name:String

# Set Wave start and end times according to enemy spawner time
@export var wave_time_start:int = 1
@export var wave_time_end:int = 2000000

# set the delay counter for each batch of spawns
@export var enemy_spawn_delay:int = 1
# set how many enemies are spawned at a time
@export var enemies_per_delay:int = 1

# contains the enemy PackedScene to be spawned
@export var enemy:Resource
@export var enemy_color:Color

# assignments for enemy to spawn
@export var enemy_health:int
@export var enemy_speed:int
@export var enemy_damage:int
@export var xp_reward:int
@export var enemy_size:float = 1.0

# used by enemy spawner
var spawn_delay_counter = 0
