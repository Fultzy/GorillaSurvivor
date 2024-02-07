using Godot;
using System;

public partial class BasicEnemy : CharacterBody2D
{
    // Basic enemy stats with default values
    [Export]
    public string name = "Basic Enemy";
    [Export]
    public float speed = 100.0f;
    [Export]
    public int health = 20;
    [Export]
    public int damage = 10;
    [Export]
    public int xpGain = 10;
    [Export]
    public int despawnTime = 3;
    [Export]
    public float size = 1.0f;

    // color needs to be set to something othervise its invisible
    [Export]
    public Color color = new Color(1, 1, 1, 1);

    // Enemy components
    public AnimatedSprite2D Sprite;
    public CollisionShape2D CollisionShape;
    public Area2D HurtBox;
    public Timer DespawnTimer;

    // Player node
    public Player target;
    public int targetWidth;
    public int targetOffset = 9;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Get the enemy components
        Sprite = (AnimatedSprite2D)GetNode("AnimatedSprite2D");
        CollisionShape = (CollisionShape2D)GetNode("CollisionShape2D");
        HurtBox = (Area2D)GetNode("HurtBox");

        // assign the target player
        target = (Player)GetNode("../../Player");
        targetWidth = (int)target.sprite.Texture.GetSize().X;

        // Set the despawn timer
        DespawnTimer = (Timer)GetNode("DespawnTimer");
        DespawnTimer.WaitTime = despawnTime;

        // set size and color
        Scale = new Vector2(size, size);
        Sprite.Modulate = color;
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double _delta)
    {
        if (target != null && health > 0)
        {
            MoveToTargetPlayer();
        }
    }


    // Move the enemy towards the player
    private void MoveToTargetPlayer()
    {
        var direction = (target.GlobalPosition - GlobalPosition).Normalized();
        Velocity = direction * speed;
        
        MoveAndSlide();

        CheckFlipSprite(direction);
    }

    // Flip the sprite based on the direction
    private void CheckFlipSprite(Vector2 direction)
    {
        // Flip the sprite based on the direction
        if (direction.X < 0)
        {
            Sprite.FlipH = true;
        }
        else if (direction.X > 0)
        {
            Sprite.FlipH = false;
        }

        // Set Sprites z index based on the y position from player
        // Only do this if within x range of the player
        if (GlobalPosition.Y - targetOffset < target.GlobalPosition.Y)
        {
            Sprite.ZIndex = 0;
        }
        else
        {
            Sprite.ZIndex = 1;
        }
    }

    // Called when the enemy is hurt
    private void _on_hurt_box_hurt_signal(long Damage)
    {
        health -= (int)Damage;
        
        // If the enemy's health is 0 or less trigger death
        if (health <= 0)
        {
            // Trigger death animation
            DespawnTimer.Start();
            Sprite.Play("Death");

            // Disable enemy collision and hitboxes
            HurtBox.QueueFree();
            CollisionShape.QueueFree();

            DropLoot();
        }
    }

    public void DropLoot()
    {
        // temp add xp to player
        // this will be replaced with a loot table
        target.xp += xpGain;
    }

    // Called when enemy despawn timer is finished
    private void _on_DespawnTimer_timeout()
    {
        QueueFree();
    }
}

