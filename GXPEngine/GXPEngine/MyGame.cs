using System;									// System contains a lot of default C# libraries 
using System.Drawing;                           // System.Drawing contains a library used for canvas drawing below
using System.Collections.Generic;
using GXPEngine;								// GXPEngine contains the engine

public class MyGame : Game
{
    public Vec2 gravity = new Vec2(0, 0.98f);
    public List<Ball> balls = new List<Ball>();
    public List<LineSegment> lines = new List<LineSegment>();

	public MyGame() : base(800, 600, false)		// Create a window that's 800x600 and NOT fullscreen
	{
        AddChild(new Player());
        lines.Add(new LineSegment(this, 0, 400, 400, 400, 0xffffffff, 3, false, false, false));
        //lines.Add(new LineSegment(this, 10, 40, 40, 40, 0xffffffff, 3, false, false, false));

        foreach (LineSegment line in lines) AddChild(line);
        foreach (Ball ball in balls) AddChild(ball);
    }

    void Update()
	{

	}

	static void Main()							// Main() is the first method that's called when the program is run
	{
		new MyGame().Start();					// Create a "MyGame" and start it
	}
}