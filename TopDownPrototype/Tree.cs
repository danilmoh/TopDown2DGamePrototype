using Godot;
using System;

public class Tree : KinematicBody2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    void BurnTree()
    {
    	var rng = new RandomNumberGenerator();
    	rng.Randomize();

    	Timer timer = new Timer();
    	AddChild(timer);
    	timer.Connect("timeout", this, "OnTimerTimeout");
    	timer.WaitTime = rng.RandfRange(2.0f, 4.0f);
    	timer.OneShot = true;
    	timer.Start();

    	GetNode<AnimatedSprite>("Fire").Show();
    }

    void OnTimerTimeout()
    {
    	CallDeferred("free");
    }

    void ChopTree()
    {
    	GetNode<Sprite>("Sprite").Hide();
    	GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;

    	var log = GetNode<RigidBody2D>("Log");
        if(log != null)
        {
            log.Show();
            log.GetNode<Sprite>("Log").Show();
            log.Sleeping = false;
            log.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
        }
    	
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
