using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class Player : CharacterBody2D
{
	// basic stats
	[Export]
	public int level = 1;
	[Export]
	public float speed = 300.0f;
	[Export]
	public int maxHealth = 100;
	[Export]
	public int damage = 5;
	[Export]
	public float attackSpeed = 1.0f;

	[Export]
	public bool autoAttack = true;

	// instance xp and health
	public int xp = 0;
	public int xpToNextLevel;
	public int health = 100;
	public int bananas = 0;

	// special attacks
	[Export]
	public Node[] attacks { get; set; }
	public Node2D thrownPoop { get; set; }

	// basic attack
	[Export]
	public PackedScene projectileScene { get; set; }
	private Node _projectilesParent;
	public Timer basicAttackTimer { get; set; }


	// Camera and sprite
	public Sprite2D sprite;
	public DamageIndicator damageIndicator;
	public Camera2D camera;
	public Vector2 zoomMin = new Vector2(0.2f, 0.2f);
	public Vector2 zoomMax = new Vector2(2, 2);
	public float zoomSpeed = 0.1f;
	public Control PlayerUI;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// get the parent node for projectiles
		_projectilesParent = GetTree().GetFirstNodeInGroup("ProjectilesParent");

		// get the basic attack timer
		basicAttackTimer = GetNode<Timer>("BasicAttackTimer");

		// TODO: Attack delay should be calculated using attack speed, not directly
		basicAttackTimer.WaitTime = attackSpeed;

		// get the sprite and camera
        sprite = (Sprite2D)GetNode("Sprite");
		camera = (Camera2D)GetNode("Camera2D");
		PlayerUI = camera.GetNode<Control>("PlayerUI");
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionJustPressed("toggle_attack"))
		{
			autoAttack = !autoAttack;
			GD.Print("AutoAttack: " + autoAttack);
		} 

		CheckXp();

		Velocity = Movement();

		MoveAndSlide();


		if (autoAttack) TryAttack();

	}

	// player movement controls, return velocity vector
    private Vector2 Movement(Vector2 velocity = new Vector2())
	{
		if (Input.IsActionPressed("up")) velocity.Y -= speed;

		if (Input.IsActionPressed("down")) velocity.Y += speed;

		if (Input.IsActionPressed("left"))
		{
			sprite.FlipH = true; // face sprite left
			velocity.X -= speed;
		}
		if (Input.IsActionPressed("right"))
		{
			sprite.FlipH = false; // flip sprite right
			velocity.X += speed;
		}

		return velocity;
	}

	// called when the player is hit
	private void _on_hurt_box_hurt_signal(long Damage)
	{
		health -= (int)Damage;
		PlayerUI.GetNode<ProgressBar>("HealthBar").Value = health;

		if (health <= 0)
		{
            // TODO: Game Over
			// play death animation
			// disable player collision and hitboxes

        }
	}

	public void UpdateUI()
	{
		// update the UI with the players stats
		PlayerUI.GetNode<ProgressBar>("HealthBar").Value = health;
		PlayerUI.GetNode<ProgressBar>("xpBar").Value = xp;

		PlayerUI.GetNode<Label>("LevelLabel").Text = "Level: " + level;
		PlayerUI.GetNode<Label>("BananasLabel").Text = "Bananas: " + bananas;
	}

	// called on every tick and trys to start an attack
    private void TryAttack()
    {
		bool IsFalse = false;
		// currently attacks.count is always 0
        if (IsFalse)
        {
			// TODO: do not use loops on something that runs every tick!!
			// instead use a counter that ticks up, using that as an index.
            foreach (Attack attack in attacks)
            {
                Vector2 aim = GetNearestEnemyPosition() - GlobalPosition;

                attack.StartAttack(aim);
            }
        }
		// if no special attacks are found, use basic attack
        else if (basicAttackTimer.IsStopped())
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
		// create a projectile instance
        Projectile projectileInstance = projectileToShoot.Instantiate<Projectile>();
        projectileInstance.damage = damage;

		// add the projectile to the parent node
        _projectilesParent.AddChild(projectileInstance);
		projectileInstance.GlobalPosition = GlobalPosition;
		
		// apply force to the projectile
		projectileInstance.ApplyForce(aim);
		basicAttackTimer.Start();
    }

	// get the position of the nearest enemy and returns a vector2
	// if there are no enemies, returns Vector2.Zero (x=0, y=0)
    public Vector2 GetNearestEnemyPosition()
    {
        Array<Node> enemies = GetTree().GetNodesInGroup("enemy");
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

        Array<Node> enemies = GetTree().GetNodesInGroup("enemy");

		// remove dead enemies
		var targets = enemies.Where(enemy => (enemy as BasicEnemy).health > 0).ToArray();
		var targetNum = targets.Count();

		if (targetNum > 0)
		{
			Random random = new Random();
			int index = random.Next(0, targetNum);
			Node2D enemy = targets[index] as Node2D;
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
        if (xp >= level * 100)
		{
            LevelUp();
        }
    }

	// calculate how much xp is needed to level up
	public void CalculateNextLevel()
	{
		int xpNeeded = level * 500 * xp / 2;

		xpToNextLevel = xpNeeded;
	}

	public void LevelUp()
	{
		// increase stats, low numbers bc lvling up is too easy
		level += 1;
		PlayerUI.GetNode<Label>("LevelLabel").Text = "Level: " + level;

		maxHealth += 5;
		PlayerUI.GetNode<ProgressBar>("HealthBar").MaxValue = maxHealth;
		
		CalculateNextLevel();
		PlayerUI.GetNode<ProgressBar>("xpBar").MinValue = xp;
		PlayerUI.GetNode<ProgressBar>("xpBar").MaxValue = xpToNextLevel;

		damage += 1;
		speed += 1;
		
		// attack speed can't go below 0.001
		if (attackSpeed - 0.08f > 0) 
			attackSpeed -= 0.01f; // SLOW AF
        else 
			attackSpeed = 0.08f; // FAST AF

        basicAttackTimer.WaitTime = attackSpeed;

		// Write new stats to console
		GD.Print("\nLevel Up! Level: " + level);
		GD.Print("MaxHealth: " + maxHealth);
		GD.Print("Attack Speed: " + attackSpeed);
		GD.Print("Damage: " + damage);
		GD.Print("Speed: " + speed);
		GD.Print("XP: " + xp);
	}
}
