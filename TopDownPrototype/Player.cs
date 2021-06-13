using Godot;
using System;

public class Player : KinematicBody2D
{
	int moveSpeed = 250;
	int bowSpeed = 1000;
	bool timerTimeout = true;

	Vector2 vel = new Vector2();
	Vector2 facingDir = new Vector2();

	RayCast2D raycast;

	[Signal]
	public delegate void Sword();
	[Signal]
	public delegate void Bow();
	[Signal]
	public delegate void ChopTreeSignal();

	PackedScene bowScene = (PackedScene)ResourceLoader.Load("res://Arrow.tscn");
	Area2D bow = null;
	Vector2 spawnPos = new Vector2();
	Vector2 pos = new Vector2();

	AnimatedSprite animSprite;

	public override void _Ready()
	{
		animSprite = GetNode<AnimatedSprite>("AnimatedSprite");

		raycast = GetNode<RayCast2D>("RayCast2D");
		raycast.Enabled = false;

		Timer bowLimitation = new Timer();
		AddChild(bowLimitation);
		bowLimitation.Connect("timeout", this, "OnArrowTimeout");
		bowLimitation.WaitTime = 1.5f;
		bowLimitation.OneShot = false;
		bowLimitation.Start();

	}

	public override void _PhysicsProcess(float delta)
	{
		vel = new Vector2();

		// inputs
		if ( Input.IsActionPressed("ui_up") )
		{
			vel.y -= 1;
			facingDir = new Vector2(0, -1);
			animSprite.Animation = "WalkUp";
		}
		if ( Input.IsActionPressed("ui_down") )
		{
			vel.y += 1;
			facingDir = new Vector2(0, 1);
			animSprite.Animation = "WalkDown";
		}
		if ( Input.IsActionPressed("ui_left") )
		{
			vel.x -= 1;
			facingDir = new Vector2(-1, 0);
			animSprite.Animation = "WalkLeft";
		}
		if ( Input.IsActionPressed("ui_right") )
		{
			vel.x += 1;
			facingDir = new Vector2(1, 0);
			animSprite.Animation = "WalkLeft";
		}

		if(facingDir == new Vector2(1,0) || facingDir == new Vector2(1, -1) || facingDir == new Vector2(1,1) )
			animSprite.FlipH = true;
		else animSprite.FlipH = false;

		if( vel == new Vector2 (0,0) && facingDir == new Vector2 (0, -1) )
			animSprite.Animation = "IdleUp";
		else if ( vel == new Vector2 (0,0) && facingDir == new Vector2(0,1) )
			animSprite.Animation = "IdleDown";
		else if ( vel == new Vector2(0,0) )
			animSprite.Stop();
		else
			animSprite.Play();

		if(vel != new Vector2(0,0))
			animSprite.Play();


		vel = vel.Normalized();

		// move player
		MoveAndSlide(vel * moveSpeed);


		if(raycast.Enabled == true)
		{
			if(raycast.IsColliding())
			{
				var KinematicCollider = new KinematicBody2D();
				var RigidBodyCollider = new RigidBody2D();
				bool checkType = false;

				var obj = raycast.GetCollider();
				Type tp = obj.GetType();
				if(tp == typeof(Tree) || tp == typeof(Enemy))
				{
					KinematicCollider = (KinematicBody2D) obj;
					checkType = true;
				}
				else if(tp == typeof(RigidBody2D))
				{
					RigidBodyCollider = (RigidBody2D) obj;
					checkType = false;
				}
				
				if(checkType)
				{
					if(KinematicCollider.IsInGroup("Enemies"))
						KinematicCollider.CallDeferred("free");
					else if( KinematicCollider.IsInGroup("Trees"))
					{
						this.Connect("ChopTreeSignal", KinematicCollider, "ChopTree");
						EmitSignal("ChopTreeSignal");
					}
				}
				/*
				if(collider.IsInGroup("Enemies"))
					collider.CallDeferred("free");

				if(collider.IsInGroup("Trees"))
				{
					
					
				}*/
			}
		}

		// move bow
		if( bow != null )
		{
			var direction = (spawnPos - pos).Normalized();
			var motion = direction * bowSpeed * delta;
			bow.Position += motion;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if( Input.IsActionJustPressed("Sword"))
		{
			GetNode<RayCast2D>("RayCast2D").Enabled = true;
			animSprite.GetNode<Sprite>("Sword").Show();

			if (facingDir.x == 1)	// right
			{
				animSprite.GetNode<Sprite>("Sword").RotationDegrees = 135;
				raycast.RotationDegrees = -90;
			}
			else if (facingDir.x == -1)	// left
			{
				animSprite.GetNode<Sprite>("Sword").RotationDegrees = -45;
				raycast.RotationDegrees = 90;
			}
			else if (facingDir.y == -1)	// up
			{
				animSprite.GetNode<Sprite>("Sword").RotationDegrees = 45;
				raycast.RotationDegrees = -180;
			}
			else if (facingDir.y == 1)	// down
			{
				animSprite.GetNode<Sprite>("Sword").RotationDegrees = -135;
				raycast.RotationDegrees = 0;
			}

			//timer
			Timer timer = new Timer();
			AddChild(timer);
			timer.Connect("timeout", this, "OnTimerTimeout");
			timer.WaitTime = 0.5f;
			timer.OneShot = true;
			timer.Start();
		}

		if(Input.IsActionJustPressed("Bow"))
		{
			var bow = GetNode<Sprite>("Bow");
			if (facingDir.x == 1)	// right
			{
				bow.RotationDegrees = -45;
				if(timerTimeout)
					bow.Show();
				addTimer();

				InstanceArrow("right");
			}
			else if (facingDir.x == -1)	// left
			{
				bow.RotationDegrees = 135;
				if(timerTimeout)
					bow.Show();
				addTimer();

				InstanceArrow("left");
			}
			else if (facingDir.y == -1)	// up
			{
				bow.RotationDegrees = 225;
				if(timerTimeout)
					bow.Show();
				addTimer();

				InstanceArrow("up");
			}
			else if (facingDir.y == 1)	// down
			{
				bow.RotationDegrees = 45;
				if(timerTimeout)
					bow.Show();
				addTimer();

				InstanceArrow("down");
			}
		}
	}


	void OnTimerTimeout()
	{
		GetNode<RayCast2D>("RayCast2D").Enabled = false;
		animSprite.GetNode<Sprite>("Sword").Hide();
	}

	void InstanceArrow(string rotation)
	{
		if(timerTimeout)
		{
			if(bow != null)
				bow.QueueFree();

			timerTimeout = false;
			bow = (Area2D)bowScene.Instance();

			pos = new Vector2(Position);
			if (rotation == "right")
			{
				spawnPos = new Vector2(pos.x + 100, pos.y);
				bow.Scale = new Vector2(3,3);
			}
			else if (rotation == "left")
			{
				spawnPos = new Vector2(pos.x - 100, pos.y);
				bow.Scale = new Vector2(-3, 3);
			}
			else if (rotation == "up")
			{
				spawnPos = new Vector2(pos.x, pos.y - 100);
				bow.RotationDegrees = -90;
			}
			else if (rotation == "down")
			{
				spawnPos = new Vector2(pos.x, pos.y + 100);
				bow.RotationDegrees = 90;
			}

			bow.Position = Position;
			GetParent().AddChild(bow);
		}
	}

	void OnArrowTimeout()
	{
		timerTimeout = true;
	}

	void addTimer()
	{
		Timer bowTimer = new Timer();
		AddChild(bowTimer);
		bowTimer.Connect("timeout", this, "OnBowTimeout");
		bowTimer.WaitTime = 0.2f;
		bowTimer.OneShot = true;
		bowTimer.Start();
	}

	void OnBowTimeout()
	{
		GetNode<Sprite>("Bow").Hide();
	}
}
