using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class HurtBox : Area2D
{
	public enum HurtBoxMode
	{
		Active,
		HitOnce,
		Disabled
	}

	[Export]
	public HurtBoxMode SavedHurtBoxMode; 
	[Export]
	public StringName Team;

	public Node2D ParentNode;
	public CollisionShape2D Collision;
	public Timer DisableTimer;
	

	[Signal]
	public delegate void HurtSignalEventHandler(int Damage);


	
	public override void _Ready()
	{
		Collision = GetNode<CollisionShape2D>("CollisionShape2D");
		DisableTimer = GetNode<Timer>("DisableTimer");

		// Assign Parentnode and Team
		ParentNode = (Node2D)GetParent();

		if (ParentNode.IsInGroup("player"))
		{
			Team = "player";
		}
		else if (ParentNode.IsInGroup("enemy"))
		{
			Team = "enemy";
		}

		// set the hurtbox to active
		SavedHurtBoxMode = HurtBoxMode.Active;
	}
	

	// when a CollisionObject2D enters this hurtbox
	private void _on_area_entered(CollisionObject2D node)
	{
		if (node is CollisionObject2D hitBox && DisableTimer.IsStopped())
		{
			// Check if this CollisionObject2D is in the attack group
			if (true)
			{
				// get the parent node of the hitbox
				var parent = hitBox.GetParent();

				// find out if this parant node is in the same group as the hurtbox
				if (parent.IsInGroup(Team)) return;
			
				var damage = (int)parent.Get("damage");
	
				switch (SavedHurtBoxMode)
				{
					// is able to get hit, disabled after getting hit
					case HurtBoxMode.Active:
						Collision.CallDeferred("set", "disabled", true);
						DisableTimer.Start();
				
						// emit a signal to the parent node to take damage
						EmitSignal(nameof(HurtSignal), damage);
				
						break;

					// not used at this time
					case HurtBoxMode.HitOnce:
						break;
				}
			}
		}
	}

	// Re-enable this hurtbox after .5 seconds
	private void _on_disable_timer_timeout()
	{
		Collision.CallDeferred("set", "disabled", false);
	}
}




