using Godot;
using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

public partial class ThrownPoop : Node2D
{
    [Export]
    public string name = "ThrownPoop Class Name";
    [Export]
    public int level = 1;
    [Export]
    public int damage = 100; 
    [Export]
    public int attackSpeed = 2;
    [Export]
    public int speed = 100;
    [Export]
    public int range = 100;
    [Export]
    public int knockback = 100;
    [Export]
    public int duration = 5;
    [Export]
    public float size = 1.0f;

    [Export]
    public int reloadTime = 5;
    [Export]
    public int reloadCount = 1;
    [Export]
    public int maxAmmo = 3;
    [Export]
    public int ammo = 3;

    public CharacterBody2D player;

    [Export]
    public bool isProjectile = true;
    [Export]
    public bool isMelee = false;

    public Vector2 target;
    public Vector2 angle;

    public override void _Ready()
    {
        player = GetTree().GetFirstNodeInGroup("Player") as CharacterBody2D;

        angle = GlobalPosition.DirectionTo(target);
        var rotation = angle.Angle();

        
    }

    public void LevelUpAttack()
    {
        level += 1;
        damage += 5;
        speed += 10;
        knockback += 10;
        size += 0.1f;
        duration += 1;
        reloadTime -= 0;
        attackSpeed += 1;
    }



}
