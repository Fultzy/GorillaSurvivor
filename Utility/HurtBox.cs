using Godot;
using System;

public partial class HurtBox : Area2D
{
	// three different types of hurtboxes are "Active", "HitOnce", "Disable".
	public enum HurtBoxType
	{
		Active,
		HitOnce,
		Disable
	}

	[Export]
	public HurtBoxType SavedHurtBoxType; 
	
	[Signal]
	public delegate void HurtSignalEventHandler(int Damage);
	[Signal]
	public delegate void reportHitCountEventHandler(int hitCount);
	
	public CollisionShape2D Collision;
	public Timer DisableTimer;
	public Timer ConsoleInfoTimer;
	public int hitCount = 0;
	
	public override void _Ready()
	{
		Collision = GetNode<CollisionShape2D>("CollisionShape2D");
		DisableTimer = GetNode<Timer>("DisableTimer");
		ConsoleInfoTimer = GetNode<Timer>("ConsoleInfoTimer");
	}
	
	// when an area2d enters this hurtbox
	private void _on_area_entered(Area2D area)
	{
		if (area is HitBox hitBox && DisableTimer.IsStopped())
		{
			// DEBUGGING
			hitCount++;

			if (hitBox.Damage > 0)
			{
				switch (SavedHurtBoxType)
				{
					// is able to get hit, disabled after getting hit
					case HurtBoxType.Active:
						Collision.CallDeferred("set", "disabled", true);
						DisableTimer.Start();
						
						break;

					// not used at this time
					case HurtBoxType.HitOnce:
						break;

					// if hurtbox disabled, disable the hitbox for some time
					case HurtBoxType.Disable:
						if (hitBox.HasMethod("TempDisable"))
						{
							hitBox.TempDisable();
						}

						break;
				}

				// emit a signal to the parent node to take damage
				EmitSignal(nameof(HurtSignal), hitBox.Damage);
			}
		}
	}

	// Re-enable this hurtbox after a certain amount of time
	private void _on_disable_timer_timeout()
	{
		Collision.CallDeferred("set", "disabled", false);
	}

	// DEBUGGING
	private void _on_console_info_timer_timeout()
	{
		EmitSignal(nameof(reportHitCount), hitCount);
		ConsoleInfoTimer.Start();
	}
}




