using Godot;
using System;

public partial class world : Node2D
{
	public int hitCount = 0;
	public Timer consoleWriteTimer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		consoleWriteTimer = GetNode<Timer>("ConsoleWriteTimer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_report_hit_count(int newhitCount)
	{
		hitCount += newhitCount;
	}

	private void _on_console_write_timer_timeout()
	{
		GD.Print("Hit count: " + hitCount);
		consoleWriteTimer.Start();
	}
}
