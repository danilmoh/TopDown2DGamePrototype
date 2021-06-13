using Godot;
using System;

public class Enemy : KinematicBody2D
{
	float speed = 100.0f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
       
    }

    public override void _PhysicsProcess(float delta)
    {
    	var direction = (GetParent().GetNode<KinematicBody2D>("Player").Position - Position).Normalized();
    	var motion = direction * speed * delta;
    	Position += motion;
    }


    void BurnEnemy()
    {
    	var rng = new RandomNumberGenerator();
    	rng.Randomize();

		Timer timer = new Timer();
		AddChild(timer);
		timer.Connect("timeout", this, "OnEnemyTimeout");
		timer.WaitTime = rng.RandfRange(1.0f, 2.5f);
		timer.OneShot = true;
		timer.Start();

		GetNode<AnimatedSprite>("Fire").Show();
    }

    void OnEnemyTimeout()
    {
    	CallDeferred("free");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
