using Godot;
using System;

public partial class HurtBox : Area2D
{
	// three different types of hurtboxes are "Cooldown", "HitOnce", "Disable".
	public enum HurtBoxType
	{
		Cooldown,
		HitOnce,
		Disable
	}

	[Export]
	public HurtBoxType SavedHurtBoxType; 
	
	[Signal]
	public delegate void HurtSignalEventHandler(int Damage);
	
	public CollisionShape2D Collision;
	public Timer DisableTimer;
	
	public override void _Ready()
	{
		Collision = GetNode<CollisionShape2D>("CollisionShape2D");
		DisableTimer = GetNode<Timer>("DisableTimer");
	}
	
	private void _on_area_entered(Area2D area)
	{
		if (area is HitBox hitBox)
		{
			if (hitBox.Damage > 0)
			{
				switch (SavedHurtBoxType)
				{
					case HurtBoxType.Cooldown:
						Collision.CallDeferred("set", "disabled", true);
						DisableTimer.Start();
						
						break;

					case HurtBoxType.HitOnce:
						break;

					case HurtBoxType.Disable:
						if (hitBox.HasMethod("TempDisable"))
						{
							hitBox.TempDisable();
						}

						break;
				}

				EmitSignal(nameof(HurtSignal), hitBox.Damage);
			}
		}
	}

	private void _on_disable_timer_timeout()
	{
		Collision.CallDeferred("set", "disabled", false);
	}

}




