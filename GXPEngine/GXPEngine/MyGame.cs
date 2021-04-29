using System;									// System contains a lot of default C# libraries 
using System.Drawing;                           // System.Drawing contains a library used for canvas drawing below
using System.Collections.Generic;
using GXPEngine;								// GXPEngine contains the engine

public class MyGame : Game
{
    public Vec2 gravity = new Vec2(0, 0.98f);
    public List<Ball> balls = new List<Ball>();
    public List<LineSegment> lines = new List<LineSegment>();
    Camera cam;

    ColliderManager colliderManager = new ColliderManager();
    Player player;

    Button startButton;

	public MyGame() : base(1920, 1080, false)		// Create a window that's 800x600 and NOT fullscreen
	{
        LoadMenu();
    }

    void Update()
	{
        if (startButton.Click())
        {
            startButton.LateDestroy();
            LoadGame();
        }

        if (cam != null)
        {
            if (Input.GetKey(Key.ZERO) && cam.scale >= 0.6f)
            {
                cam.scale -= 0.2f;
            }
            if (Input.GetKey(Key.NINE) && cam.scale <= 3)
            {
                cam.scale += 0.2f;
            }
        }
    }

	static void Main()							// Main() is the first method that's called when the program is run
	{
		new MyGame().Start();					// Create a "MyGame" and start it
	}

    void LoadMenu()
    {
        startButton = new Button(300, 300);
        AddChild(startButton);
    }

    void LoadGame()
    {
        player = new Player();
        AddChild(player);

        cam = new Camera(0, 0, width, height);
        cam.scale = 2f;
        player.AddChild(cam);

        //lines.Add(new LineSegment(this, 0, 0, width, 0));
        lines.Add(new LineSegment(this, 0, 510, 0, 200));
        lines.Add(new LineSegment(this, width, 0, width, height));
        lines.Add(new LineSegment(this, 300, 500, -10, 500, newFloor: true));
        lines.Add(new LineSegment(this, 300, 600, 300, 500));
        lines.Add(new LineSegment(this, 850, 500, 500, 500, newFloor: true));
        lines.Add(new LineSegment(this, 500, 500, 500, 600));
        lines.Add(new LineSegment(this, 800, 1000, 0, 1000, newFloor: true));

        lines.Add(new LineSegment(this, 1200, 420, 800, 510, newFloor: true));
        //lines.Add(new LineSegment(this, 400, 450, 0, 400, newFloor: true));

        //AddChild(new Box());

        foreach (LineSegment line in lines) AddChild(line);
        foreach (Ball ball in balls) AddChild(ball);
    }
}