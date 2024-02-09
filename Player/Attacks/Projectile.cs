using Godot;
using System;

public partial class Projectile : RigidBody2D
{

    // Basic stats
    [Export]
    public int damage { get; set; } = 1;
    [Export]
    public int speed { get; set; } = 100;
    [Export]
    public int lifeTime { get; set; } = 1;
    public Timer lifeTimer { get; set; }

    // Sounds
    public AudioStreamPlayer2D audioStreamPlayer2D;
    [Export]
    public AudioStream ThrowSound { get; set; }
    [Export]
    public AudioStream HitSound { get; set; }

    // Effects
    [Export]
    public PackedScene HitEffect { get; set; }


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Get Projectile Components
        lifeTimer = GetNode<Timer>("LifeTimer");
        audioStreamPlayer2D = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");

        // Assign a life time to the projectile and start the timer
        lifeTimer.WaitTime = lifeTime;
        lifeTimer.Start();
        
        // Play the throw sound then switch to the hit sound
        audioStreamPlayer2D.Stream = ThrowSound;
        audioStreamPlayer2D.Play();
        audioStreamPlayer2D.Stream = HitSound;
    }

    // After the projectile is thrown, Remove it after some time
    public void _on_LifeTimer_timeout()
    {
        QueueFree();
    }

    // When the projectile hits something, play the hit sound
    public void _on_HurtBox_area_entered(Area2D area)
    {

        if (area is HurtBox)
        {
            audioStreamPlayer2D.Play(GlobalPosition.X);
        }
    }
}
