using Godot;
using System;

public partial class HitBox : Area2D
{
	[Export]
	public int Damage = 1;

	public CollisionShape2D Collision;
	public Timer DisableTimer;

	public override void _Ready()
	{
		Collision = GetNode<CollisionShape2D>("CollisionShape2D");
		DisableTimer = GetNode<Timer>("DisableTimer");
	}
	
	public void TempDisable()
	{
		Collision.CallDeferred("set", "disabled", true);
		DisableTimer.Start();
	}

	public void _on_DisableTimer_timeout()
	{
		Collision.CallDeferred("set", "disabled", false);
	}
}
