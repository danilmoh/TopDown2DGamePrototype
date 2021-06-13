using Godot;
using System;

public class Arrow : Area2D
{
	[Signal]
	public delegate void BurnTreeSignal();
	[Signal]
	public delegate void BurnEnemySignal();
    
    bool isFiring = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    	
    }

    void _on_Arrow_body_entered(PhysicsBody2D body)
    {
    	if(body.IsInGroup("Enemies"))
    	{
    		if(isFiring)
    		{
    			this.Connect("BurnEnemySignal", body, "BurnEnemy");
    			BurnEnemy();
    		}
    		else
    			body.CallDeferred("free");
    		Hide();
    	}

    	if(body.IsInGroup("Trees"))
    	{
    		if(isFiring)
    		{
    			this.Connect("BurnTreeSignal", body, "BurnTree");
    			BurnTree();
    		}
    		Hide();
    	}

    	if(body.IsInGroup("Fire"))
    	{
    		GetNode<AnimatedSprite>("AnimatedSprite").Show();
    		isFiring = true;
    	}
    }

    void BurnTree()
    {
    	EmitSignal("BurnTreeSignal");	
    }

    void BurnEnemy()
    {
    	EmitSignal("BurnEnemySignal");
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
