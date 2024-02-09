using Godot;
using System;

public partial class xpCollectable : Area2D
{

	[Export]
	public int healing;
	// healing is applied as damage to the player
	public int xpValue = 1;
	[Export]
	public float speed { get; set; } = 1.0f;
	public int time = 0;
	public Player player;
	public bool trackingPlayer = false;

	public Sprite2D Sprite;
	public AudioStreamPlayer2D audioStreamPlayer2D;
	[Export]
	public AudioStream CollectedSound { get; set; }


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Sprite = GetNode<Sprite2D>("Sprite2D");
		audioStreamPlayer2D = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
		player = (Player)GetNode("../../Player");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(trackingPlayer)
		{
			MoveToPlayer();
		}
		else
		{
			FloatSprite();
		}	

	}


	// when the players lootpickup area enters the xpCollectable area
	public void _on_xpCollectable_body_entered(Area2D area)
	{
		if (area.GetParent() is Player player)
		{
			if(area.IsInGroup("loot_pickup"))
			{
				trackingPlayer = true;

				// modify speed by player level
				speed = 2.0f + (float)player.level / 100;
			}
			
			if (area.Name == "HurtBox")
			{
				QueueFree();
				audioStreamPlayer2D.Stream = CollectedSound;
				audioStreamPlayer2D.Play();
				
				player.xp += xpValue;
				player.health += healing;
				player.bananas += 1;
				player.UpdateUI();
			}
		}
	}

	public void MoveToPlayer()
	{
		Vector2 playerPos = player.GlobalPosition;
		Vector2 pos = GlobalPosition;
		Vector2 direction = (playerPos - pos).Normalized();
		pos += direction * speed;
		GlobalPosition = pos;
	}


	public Vector2 FloatSprite()
	{
		Vector2 pos = GlobalPosition;



		return pos;
	}

	public void _on_despawn_timer_timeout() 
	{
		QueueFree();
	}

}
