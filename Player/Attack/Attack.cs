using Godot;
using System;

public partial class Attack : Node
{
    [Export]
    public string name;
    [Export] 
    public int level;
    [Export]
    public int damage;
    [Export]
    public int attackSpeed;
    [Export]
    public int speed;
    [Export]
    public int range;
    [Export]
    public int knockback;
    [Export] 
    public int duration;
    [Export]
    public float size = 1.0f;

    [Export]
    public int reloadTime; 
    [Export]
    public int reloadCount;
    [Export]
    public int maxAmmo;
    public int ammo;


    [Export]
    public bool isProjectile;
    [Export]
    public bool isMelee;

    [Export]
    public Sprite2D sprite;

    public Vector2 target;

    public Timer attackTimer;
    public Timer reloadTimer;

    public bool waiting = false; 

    public override void _Ready()
    {
        attackTimer = GetNode<Timer>("AttackTimer");
        reloadTimer = GetNode<Timer>("ReloadTimer");

        attackTimer.WaitTime = attackSpeed;
        reloadTimer.WaitTime = reloadTime;

        ammo = maxAmmo;
    }

    public void StartAttack(Vector2 aim)
    {

        GD.Print($"{name} start attack");

        if (ammo > 0 && waiting == false)
        {
            target = aim;
            ammo -= 1;

            GD.Print($"{name} attack! ammo: {ammo} aim:{aim.X}, {aim.Y}");

            attackTimer.Start();
            waiting = true;
        }

        if (ammo < maxAmmo)
        {
            reloadTimer.Start();
        }
    }

    public void _on_attack_timer_timeout()
    {
        
        attackTimer.Stop();
        waiting = false;
    }

    public void _on_reload_timer_timeout()
    {
        if (ammo < maxAmmo)
        {
            if(reloadCount + ammo > maxAmmo)
            {
                ammo = maxAmmo;
            }
            else
            {
                ammo += reloadCount;
            }
        }
    }
    
    public bool IsWithinRange(Vector2 aim)
    {
        float distance = aim.DistanceTo(target);
        return distance <= range;
    }
}
