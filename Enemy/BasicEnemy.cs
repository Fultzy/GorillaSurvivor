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
    [Export]
    public PackedScene xpDrop { get; set; }
    public AnimatedSprite2D Sprite;
    public CollisionShape2D CollisionShape;
    public Area2D HurtBox;
    public PackedScene DamageIndicator;
    public Timer DespawnTimer;

    public Node ObjectsParent;

    // Player node
    public Player target;
    public int targetWidth;
    public int targetOffset = 9;

    // DeBugging
    [Export]
    public bool IsPunchingBag = false;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Get the enemy components
        Sprite = (AnimatedSprite2D)GetNode("AnimatedSprite2D");
        CollisionShape = (CollisionShape2D)GetNode("CollisionShape2D");
        HurtBox = (Area2D)GetNode("HurtBox");
        DamageIndicator = (PackedScene)ResourceLoader.Load("res://UI/DamageIndicator/DamageIndicator.tscn");

        // assign the target player TODO: Make it Better
        target = (Player)GetNode("../../Player");

        // Get the Objects parent node
        ObjectsParent = GetTree().GetFirstNodeInGroup("objects");

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
        if (IsPunchingBag) return;

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
            // this is the same layer as the player
            Sprite.ZIndex = target.ZIndex;
        }
        else
        {
            // this is the layer above the player
            Sprite.ZIndex = target.ZIndex + 1;
        }
    }

    // Called when the enemy is hurt
    private void _on_hurt_box_hurt_signal(long Damage)
    {   
        if (IsPunchingBag) return;

        health -= (int)Damage;
        //CallDeferred("SpawnDamageIndicator", (int)Damage);
        if (Damage > 0) SpawnDamageIndicator((int)Damage);

        // If the enemy's health is 0 or less trigger death
        if (health <= 0)
        {
            // Trigger death animation
            DespawnTimer.Start();
            Sprite.ZIndex = 0;
            Sprite.Play("Death");

            // Disable enemy collision and hitboxes
            HurtBox.QueueFree();
            CollisionShape.QueueFree();

            // Drop loot
            CallDeferred("DropLoot");
        }
    }

    // Spawn a damage indicator
    private void SpawnDamageIndicator(int damage)
    {
        // Create a new damage indicator and set its position
        DamageIndicator dmgInd = DamageIndicator.Instantiate() as DamageIndicator;
        dmgInd.GlobalPosition = GlobalPosition;
        //dmgInd.label.Text = damage.ToString();

        // Add the damage indicator to the scene
        ObjectsParent.AddChild(dmgInd);
    }

    // Call this method like this: CallDeferred("DropLoot");
    // this prevents godot from spawning the drop on ths same frame as the call, which causes errors
    public void DropLoot()
    {
        // Create a new xp drop and set its position
        xpCollectable drop = xpDrop.Instantiate<xpCollectable>();
        drop.GlobalPosition = GlobalPosition;
        drop.xpValue = xpGain;

        // Add the xp drop to the scene
        ObjectsParent.AddChild(drop);
    }

    // Called when enemy despawn timer is finished
    private void _on_DespawnTimer_timeout()
    {
        QueueFree();
    }
}

