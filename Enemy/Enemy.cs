using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
    [Export]
    public float Speed = 100.0f;
    [Export]
    public int Health = 20;
    [Export]
    public int Damage = 10;
    [Export]
    public int xpGain = 10;

    public bool IsLeft = false;
    public AnimatedSprite2D Sprite;
    public Player Player;

    public override void _Ready()
    {
        Sprite = (AnimatedSprite2D)GetNode("AnimatedSprite2D");
        Player = (Player)GetNode("../../Player");
    }


    public override void _PhysicsProcess(double _delta)
    {
        if (Player != null)
        {
            MoveToTargetPlayer();
        }

        TurnSprite();
        MoveAndSlide();
    }


    private void MoveToTargetPlayer()
    {
        var direction = GlobalPosition.DirectionTo(Player.GlobalPosition);
        Velocity = direction * Speed;

        if (Velocity.X < 0)
        {
            IsLeft = true;
        }
        else if (Velocity.X > 0)
        {
            IsLeft = false;
        }
    }
    private void _on_hurt_box_hurt_signal(long Damage)
    {
        Health -= (int)Damage;

        if (Health <= 0)
        {
            Player.xp += xpGain;
            QueueFree();
        }
    }

    private void TurnSprite()
    {
        if (IsLeft)
        {
            Sprite.FlipH = true;
        }
        else
        {
            Sprite.FlipH = false;
        }
    }
}

