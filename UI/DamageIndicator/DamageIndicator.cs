using Godot;
using System;

public partial class DamageIndicator : Node2D
{
	[Export]
	public float speed = 100.0f;
	[Export]
	public int friction = 1;
	public Vector2 shiftDirection;
	public Label label;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		label = GetNode<Label>("Label");
		// set the label to random number
		label.Text = (new Random()).Next(1, 100).ToString();

		// shift direction left or right
		Random rnd = new Random();
		shiftDirection = new Vector2(rnd.Next(-1, 1), rnd.Next(-1, 1));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GlobalPosition += shiftDirection * speed * (float)delta;
		speed = Mathf.Max(0, speed - friction * (float)delta);
	}
}
