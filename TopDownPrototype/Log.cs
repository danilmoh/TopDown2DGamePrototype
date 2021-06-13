using Godot;
using System;

public class Log : RigidBody2D
{
	int mass = 7;
	int velocity = 100;
	int gravity = 3500;
	Vector2 motion;

    bool entered = false;
    bool movingTimeout = true;

    Vector2 pos;
    bool posCheck = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    public void ChangePosition(string axis)
    {
    	entered = true;
    	if(axis == "Horizontal")
    		ApplyHorizontal();
    	else if(axis == "Vertical")
    		ApplyVertical();
    }

    public override void _PhysicsProcess(float delta)
    {

    }

    void ApplyVertical()
    {
		Timer timer = new Timer();
		AddChild(timer);
    	timer.Connect("timeout", this, "OnTimerTimeoutUp");
    	timer.OneShot = false;
    	timer.WaitTime = 0.25f;
    	timer.Start();

    	addMovingTimeTimer();
    }

    void ApplyHorizontal()
    {
		Timer timer = new Timer();
		AddChild(timer);
    	timer.Connect("timeout", this, "OnTimerTimeoutRight");
    	timer.OneShot = false;
    	timer.WaitTime = 0.25f;
    	timer.Start();

    	addMovingTimeTimer();
    }

    void OnTimerTimeoutUp()
    {
    	if(movingTimeout)
    	{
    		pos = Position;
	    	pos.y -= 10;
	    	Position = pos;
	    	posCheck = true;
    	}
    }

    void OnTimerTimeoutRight()
    {
    	if(movingTimeout)
    	{
    		pos = Position;
	    	pos.x += 10;
	    	posCheck = true;
    	}
    }

    void addMovingTimeTimer()
    {
    	Timer movingTimer = new Timer();
    	AddChild(movingTimer);
    	movingTimer.Connect("timeout", this, "OnMovingTimerTimeout");
    	movingTimer.OneShot = true;
    	movingTimer.WaitTime = 10.0f;
    	movingTimer.Start();
    }

    void OnMovingTimerTimeout()
    {
    	movingTimeout = false;
    }

    public override void _IntegrateForces(Physics2DDirectBodyState state)
    {
    	if(posCheck)
    	{
    		Position = pos;
    	}
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
