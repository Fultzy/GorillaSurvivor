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
	public bool IsLeft = false;
	public Sprite2D sprite;
	public Camera2D camera;
	public Vector2 ZoomMin = new Vector2(0.2f, 0.2f);
	public Vector2 ZoomMax = new Vector2(2, 2);
	public float ZoomSpeed = 0.1f;


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


	public override void _PhysicsProcess(double delta)
	{
		CheckXp();

		Velocity = Movement();

		TryAttack();

		TurnSprite();
		MoveAndSlide();
	}


    private Vector2 Movement(Vector2 velocity = new Vector2())
	{
		if (Input.IsActionPressed("up"))
		{
			velocity.Y -= Speed;
		}
		if (Input.IsActionPressed("down"))
		{
			velocity.Y += Speed;
		}
		if (Input.IsActionPressed("left"))
		{
			IsLeft = true;
			velocity.X -= Speed;
		}
		if (Input.IsActionPressed("right"))
		{
			IsLeft = false;
			velocity.X += Speed;
		}

		return velocity;
	}

	public void TurnSprite()
	{
		if (IsLeft)
		{
			sprite.FlipH = true;
		}
		else
		{
			sprite.FlipH = false;
		}
	}

	private void _on_hurt_box_hurt_signal(long Damage)
	{
		Health -= (int)Damage;
		GD.Print("Player Hit! Health: " + Health);

		if (Health <= 0)
		{
            QueueFree();
        }
	}

	
    private void TryAttack()
    {
        if (Attacks.Count() > 0)
        {
            foreach (Attack attack in Attacks)
            {
                Vector2 aim = GetNearestEnemyPosition() - GlobalPosition;

                attack.StartAttack(aim);
            }
        }
        else if (basicAttackTimer.IsStopped())
        {
			Vector2 target = GetRandomEnemyPosition();
			if (target == Vector2.Zero) return;

            Vector2 aim = target - GlobalPosition;

            ShootProjectile(projectileScene, aim);
        }
    }

    private void ShootProjectile(PackedScene projectileToShoot, Vector2 aim)
    {
        Projectile projectileInstance = projectileToShoot.Instantiate<Projectile>();

        projectileInstance.Damage = Damage;


        _projectilesParent.AddChild(projectileInstance);

		projectileInstance.GlobalPosition = GlobalPosition;
		basicAttackTimer.Start();

		projectileInstance.ApplyForce(aim);
    }

    public Vector2 GetNearestEnemyPosition()
    {
        Array<Node> enemies = GetTree().GetNodesInGroup("Enemies");
        if (enemies.Count > 0)
        {
            Node2D nearestEnemy = null;
            float nearestDistance = float.MaxValue;
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

	public void _on_BasicAttackTimer_timeout()
	{
        basicAttackTimer.Stop();
	}

    private void CheckXp()
    {
        if (xp >= Level * 100)
		{
            LevelUp();
        }
    }

	public void LevelUp()
	{
		Level += 1;
		MaxHealth += 10;
		Damage += 1;
		Speed += 1;
		
		if (AttackSpeed - 0.01f > 0)
		{
			AttackSpeed -= 0.01f;
		}
        else
        {
			AttackSpeed = 0.001f;
        }

        basicAttackTimer.WaitTime = AttackSpeed;


        // reset health
        Health = MaxHealth;

		sprite.Modulate = new Color(1, 1, 1, 1);
		Label levelUpLabel = new Label();
		levelUpLabel.Text = "Level Up";
		sprite.AddChild(levelUpLabel);

		GD.Print("\nLevel Up! Level: " + Level);
		GD.Print("MaxHealth: " + MaxHealth);
		GD.Print("Attack Speed: " + AttackSpeed);
		GD.Print("Damage: " + Damage);
		GD.Print("Speed: " + Speed);
		GD.Print("XP: " + xp);
	}
}
