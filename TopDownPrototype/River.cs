using Godot;
using System;

public class River : StaticBody2D
{
	RigidBody2D log = null;
	[Signal]
	public delegate void LogEnteredSignal(string axis);

	string axis = null;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	   	if(this.IsInGroup("VerticalWater"))
	   		axis = "Vertical";
	   	else if(this.IsInGroup("HorizontalWater"))
	   		axis = "Horizontal";
	}


	public void _on_RiverArea_body_entered(PhysicsBody2D body)
	{
		if(body.IsInGroup("Log"))
		{
			log = (RigidBody2D)body;
			this.Connect("LogEnteredSignal", log, "ChangePosition");
			EmitSignal("LogEnteredSignal", axis);
		}
	}

	

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}


