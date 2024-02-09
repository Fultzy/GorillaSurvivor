using Godot;
using System;

public partial class OptionsMenu : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}



	public void _on_back_to_main_button_button_down()
	{
		PackedScene mainMenuScene = (PackedScene)ResourceLoader.Load("res://UI/MainMenu/main_menu.tscn");

		// remove the options menu from the scene tree
		GetTree().CurrentScene.QueueFree();
		
		// add the main menu scene to the scene tree
		GetTree().Root.AddChild(mainMenuScene.Instantiate());
	}
}
