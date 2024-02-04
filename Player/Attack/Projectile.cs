using Godot;
using System;

public partial class Projectile : RigidBody2D
{
    public Timer LifeTimer { get; set; }
    public HitBox HitBox { get; set; }


    [Export]
    public int Damage { get; set; } = 1;
    [Export]
    public int Speed { get; set; } = 100;
    [Export]
    public int LifeTime { get; set; } = 5;

    public override void _Ready()
    {
        LifeTimer = GetNode<Timer>("LifeTimer");
        HitBox = GetNode<HitBox>("HitBox");

        HitBox.Damage = Damage;

        LifeTimer.WaitTime = LifeTime;
        LifeTimer.Start();
    }

    public void _on_LifeTimer_timeout()
    {
        QueueFree();
    }
}
