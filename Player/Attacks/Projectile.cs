using Godot;
using System;

public partial class Projectile : RigidBody2D
{
    public Timer lifeTimer { get; set; }


    [Export]
    public int damage { get; set; } = 1;
    [Export]
    public int speed { get; set; } = 100;
    [Export]
    public int lifeTime { get; set; } = 1;

    public override void _Ready()
    {
        lifeTimer = GetNode<Timer>("LifeTimer");

        lifeTimer.WaitTime = lifeTime;
        lifeTimer.Start();
    }

    public void _on_LifeTimer_timeout()
    {
        QueueFree();
    }
}
