using Godot;
using System;

public partial class CameraControler : Camera2D
{
	public Vector2 ZoomMin = new Vector2(0.2f, 0.2f);
	public Vector2 ZoomMax = new Vector2(2,2);
	public float ZoomSpeed = 0.1f;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void ZoomCamera()
	{
        var zoom = Zoom;
        
        if (Input.IsActionPressed("scrollup"))
		{
            zoom.X -= ZoomSpeed;
            zoom.Y -= ZoomSpeed;
        }
        if (Input.IsActionPressed("scrolldown"))
		{
            zoom.X += ZoomSpeed;
            zoom.Y += ZoomSpeed;
        }
        
        zoom.X = Mathf.Clamp(zoom.X, ZoomMin.X, ZoomMax.X);
        zoom.Y = Mathf.Clamp(zoom.Y, ZoomMin.Y, ZoomMax.Y);
        
        Zoom = zoom;
    }
}
