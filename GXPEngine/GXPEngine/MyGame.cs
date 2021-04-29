using System;									// System contains a lot of default C# libraries 
using System.Drawing;                           // System.Drawing contains a library used for canvas drawing below
using System.Collections.Generic;
using GXPEngine;								// GXPEngine contains the engine

public class MyGame : Game
{
    public Vec2 gravity = new Vec2(0, 0.1f);
    public List<Ball> balls = new List<Ball>();
    public List<LineSegment> lines = new List<LineSegment>();
    PhysicsManager physicsManager;

	public MyGame() : base(800, 600, false)		// Create a window that's 800x600 and NOT fullscreen
	{
        AddChild(new Player());
        //lines.Add(new LineSegment(this, 0, 0, width, 0));
        lines.Add(new LineSegment(this, 0, 500, 0, 0));
        lines.Add(new LineSegment(this, width, 0, width, height));
        lines.Add(new LineSegment(this, 300, 500, 0, 500));
        lines.Add(new LineSegment(this, 300, 600, 300, 500));
        lines.Add(new LineSegment(this, 800, 500, 500, 500));
        lines.Add(new LineSegment(this, 500, 500, 500, 600));

        physicsManager = new PhysicsManager(this);

        PhysicsBody obj = new PhysicsBody(1);
        obj.AddPoint(new Vec2(300, 100), false);
        obj.AddPoint(new Vec2(400, 200), false);
        obj.AddPoint(new Vec2(400, 250), false);
        obj.AddPoint(new Vec2(300, 150), false);
        physicsManager.AddPhysicsBody(obj);
        AddChild(obj);

        PhysicsBody obj2 = new PhysicsBody(0.1f);
        obj2.AddPoint(new Vec2(250, 400), true);
        obj2.AddPoint(new Vec2(450, 350), true);
        obj2.AddPoint(new Vec2(450, 400), true);
        obj2.AddPoint(new Vec2(250, 450), true);
        physicsManager.AddPhysicsBody(obj2);
        AddChild(obj2);

        foreach (LineSegment line in lines) AddChild(line);
        foreach (Ball ball in balls) AddChild(ball);
    }

    void Update()
	{
        physicsManager.Step();
    }

	static void Main()							// Main() is the first method that's called when the program is run
	{
		new MyGame().Start();					// Create a "MyGame" and start it
	}
}