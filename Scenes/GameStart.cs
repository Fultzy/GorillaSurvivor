using Godot;
using System;

public partial class GameStart : Node2D
{

	public Node2D MainMenuNode;
	public Node2D OptionsNode;
	public PackedScene gameScene = (PackedScene)ResourceLoader.Load("res://Scenes/world.tscn");


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// get the main menu node
		MainMenuNode = GetNode<Node2D>("MainMenu");
		OptionsNode = GetNode<Node2D>("OptionsMenu");
		
		// hide the options node
		OptionsNode.Visible = false;
		
		// show the main menu node
		MainMenuNode.Visible = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

public void _on_play_button_button_down() 
	{
		// remove the main menu from the scene tree
		GetTree().CurrentScene.QueueFree();

		// add the game scene to the scene tree
		GetTree().Root.AddChild(gameScene.Instantiate());
	}

	public void _on_options_button_button_down()
	{
		MainMenuNode.Visible = false;
		OptionsNode.Visible = true;
	}

	public void _on_back_to_main_button_button_down()
	{
		MainMenuNode.Visible = true;
		OptionsNode.Visible = false;
	}
}
