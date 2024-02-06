using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

public partial class Player : CharacterBody2D
{
	// basic stats
	[Export]
	public int Level = 1;
	[Export]
	public float Speed = 300.0f;
	[Export]
	public int MaxHealth = 100;
	[Export]
	public int Damage = 5;
	[Export]
	public float AttackSpeed = 1.0f;

	// DEBUG
	[Export]
	public bool BasicAttack = true;

	public int xp = 0;
	public int Health = 100;

	// special attacks
	[Export]
	public Node[] Attacks { get; set; }
	public Node2D thrownPoop { get; set; }

	// basic attack
	public Timer basicAttackTimer { get; set; }
	private Node _projectilesParent;
	[Export]
	public PackedScene projectileScene { get; set; }
	[Export]
	public StringName ProjectilesParantGroup { get; set; } = "ProjectilesParent";


	// Camera and sprite
	public Sprite2D sprite;
	public Camera2D camera;
	public Vector2 ZoomMin = new Vector2(0.2f, 0.2f);
	public Vector2 ZoomMax = new Vector2(2, 2);
	public float ZoomSpeed = 0.1f;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_projectilesParent = GetTree().GetFirstNodeInGroup(ProjectilesParantGroup);

		if (_projectilesParent == null)
		{
			GD.PushWarning("No ProjectilesParent found");
		}

		basicAttackTimer = GetNode<Timer>("BasicAttackTimer");
		basicAttackTimer.WaitTime = AttackSpeed;


        sprite = (Sprite2D)GetNode("Sprite");
		camera = (Camera2D)GetNode("Camera2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		CheckXp();

		Velocity = Movement();

		TryAttack();

		MoveAndSlide();
	}

	// player movement controls, return velocity vector
    private Vector2 Movement(Vector2 velocity = new Vector2())
	{
		if (Input.IsActionPressed("up")) velocity.Y -= Speed;

		if (Input.IsActionPressed("down")) velocity.Y += Speed;

		if (Input.IsActionPressed("left"))
		{
			sprite.FlipH = true; // face sprite left
			velocity.X -= Speed;
		}
		if (Input.IsActionPressed("right"))
		{
			sprite.FlipH = false; // flip sprite right
			velocity.X += Speed;
		}

		return velocity;
	}

	// called when the player is hit
	// 
	private void _on_hurt_box_hurt_signal(long Damage)
	{
		Health -= (int)Damage;

		if (Health <= 0)
		{
            // TODO: Game Over
			// play death animation
			// disable player collision and hitboxes

        }
	}

	// called on every tick and trys to start an attack
    private void TryAttack()
    {
		// currently attacks.count is always 0
        if (Attacks.Count() > 0)
        {
			// TODO: do not use loops on something that runs every tick!!
			// instead use a counter that ticks up, using that as an index.
            foreach (Attack attack in Attacks)
            {
                Vector2 aim = GetNearestEnemyPosition() - GlobalPosition;

                attack.StartAttack(aim);
            }
        }
		// if no special attacks are found, use basic attack
        else if (basicAttackTimer.IsStopped() && BasicAttack)
        {
			Vector2 target = GetRandomEnemyPosition();
			if (target == Vector2.Zero) return; // no enemies found

            Vector2 aim = target - GlobalPosition;

            ShootProjectile(projectileScene, aim);
        }
    }

	// this is a temp implimentation that shoots poop. 
	// i dont like how its in the player script.... 
    private void ShootProjectile(PackedScene projectileToShoot, Vector2 aim)
    {
        Projectile projectileInstance = projectileToShoot.Instantiate<Projectile>();

        projectileInstance.Damage = Damage;

        _projectilesParent.AddChild(projectileInstance);

		projectileInstance.GlobalPosition = GlobalPosition;
		basicAttackTimer.Start();

		projectileInstance.ApplyForce(aim);
    }

	// get the position of the nearest enemy and returns a vector2
	// if there are no enemies, returns Vector2.Zero (x=0, y=0)
    public Vector2 GetNearestEnemyPosition()
    {
        Array<Node> enemies = GetTree().GetNodesInGroup("Enemies");
        if (enemies.Count > 0)
        {
            Node2D nearestEnemy = null;
            float nearestDistance = float.MaxValue;

			// BAD BAD BAD
            foreach (Node enemy in enemies)
            {
                Node2D enemy2D = enemy as Node2D;
                float distance = GlobalPosition.DistanceTo(enemy2D.GlobalPosition);
                if (distance < nearestDistance)
                {
                    nearestEnemy = enemy2D;
                    nearestDistance = distance;
                }
            }
			
            return nearestEnemy.GlobalPosition;
        }
        else
        {
            return Vector2.Zero;
        }
    }

	// get the position of a random enemy and returns a vector2
	// if there are no enemies, returns Vector2.Zero (x=0, y=0)
	public Vector2 GetRandomEnemyPosition()
	{
        Array<Node> enemies = GetTree().GetNodesInGroup("Enemies");
        if (enemies.Count > 0)
		{
            Random random = new Random();
            int index = random.Next(0, enemies.Count);
            Node2D enemy = enemies[index] as Node2D;
            return enemy.GlobalPosition;
        }
        else
		{
            return Vector2.Zero;
        }
    }

	// this is used to delay the basic attack
	// this should be implemented in a different way in the future
	// there should be a attack speed that modifies the basic attack timer
	// currently it is just a delay, it will try to go negative.
	public void _on_BasicAttackTimer_timeout()
	{
        basicAttackTimer.Stop();
	}

	// check if the player has enough xp to level up
	// XP needs to be calculated differently in the future
	// doing 100 per level is too easy. but for testing this is good. 
    private void CheckXp()
    {
		// this will be used to emit a signal to trigger the level up screen
        if (xp >= Level * 100)
		{
            LevelUp();
        }
    }

	public void LevelUp()
	{
		// increase stats, low numbers bc lvling up is too easy
		Level += 1;
		MaxHealth += 5;
		Damage += 1;
		Speed += 1;
		
		// attack speed can't go below 0.001
		if (AttackSpeed - 0.08f > 0) 
			AttackSpeed -= 0.01f; // SLOW AF
        else 
			AttackSpeed = 0.08f; // FAST AF

        basicAttackTimer.WaitTime = AttackSpeed;

        // reset health
        Health = MaxHealth;

		// Write new stats to console
		GD.Print("\nLevel Up! Level: " + Level);
		GD.Print("MaxHealth: " + MaxHealth);
		GD.Print("Attack Speed: " + AttackSpeed);
		GD.Print("Damage: " + Damage);
		GD.Print("Speed: " + Speed);
		GD.Print("XP: " + xp);
	}
}
