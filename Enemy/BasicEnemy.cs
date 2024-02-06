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
    public Area2D HitBox;
    public Timer DespawnTimer;

    // Player node
    public Player target;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Get the enemy components
        Sprite = (AnimatedSprite2D)GetNode("AnimatedSprite2D");
        CollisionShape = (CollisionShape2D)GetNode("CollisionShape2D");
        HurtBox = (Area2D)GetNode("HurtBox");
        HitBox = (Area2D)GetNode("HitBox");

        // assign the target player
        target = (Player)GetNode("../../Player");

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

        // If the enemy has a target and is alive

        

        if (target != null && health > 0)
        {
            MoveToTargetPlayer();
            
            MoveAndSlide();
        }
    }


    // Move the enemy towards the player
    private void MoveToTargetPlayer()
    {
        var direction = GlobalPosition.DirectionTo(target.GlobalPosition);
        Velocity = direction * speed;

        //CheckFlipSprite(direction);
    }

    // Flip the sprite based on the direction
    private void CheckFlipSprite(Vector2 direction)
    {
        if (direction.X < 0)
        {
            Sprite.FlipH = true;
        }
        else if (direction.X > 0)
        {
            Sprite.FlipH = false;
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
            HitBox.QueueFree();
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

