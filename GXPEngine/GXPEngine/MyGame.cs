using System;									// System contains a lot of default C# libraries 
using System.Drawing;                           // System.Drawing contains a library used for canvas drawing below
using System.Collections.Generic;
using GXPEngine;								// GXPEngine contains the engine

public class MyGame : Game
{
    public Vec2 gravity = new Vec2(0, 0.1f);
    public float friction = 0.995f;
    public float groundFriction = 0.9f;

    public List<Ball> balls = new List<Ball>();
    public List<LineSegment> lines = new List<LineSegment>();
    public PhysicsManager physicsManager;
    Camera cam;

    ColliderManager colliderManager = new ColliderManager();
    Player player;

    Button startButton;
    Button exitButton;

    bool paused;

    PhysicsBody rope;

    public MyGame() : base(1920, 1080, false)       // Create a window that's 800x600 and NOT fullscreen
    {
        LoadMenu();
    }

    void Update()
    {
        this.scale = width / 1920f;
        Console.WriteLine(currentFps);
        if (physicsManager != null && !paused)
        {
            physicsManager.Step();
        }

        if (startButton.Click())
        {
            startButton.LateDestroy();
            exitButton.LateDestroy();
            LoadGame();
        }
        if (exitButton.Click())
        {
            Environment.Exit(0);
        }

        if (cam != null)
        {
            if (Input.GetKeyDown(Key.ENTER))
            {
                foreach (GameObject obj in GetChildren())
                {
                    obj.LateRemove();
                }
                //paused = true;
                LoadMenu();
            }


            if (Input.GetKey(Key.ZERO) && cam.scale >= 0.6f)
            {
                cam.scale -= 0.2f;
            }
            if (Input.GetKey(Key.NINE) && cam.scale <= 3)
            {
                cam.scale += 0.2f;
            }
        }

        if (rope != null && Input.GetKeyDown(Key.C))
        {
            rope.RemoveConnection(2);
        }
    }

    static void Main()                          // Main() is the first method that's called when the program is run
    {
        new MyGame().Start();                   // Create a "MyGame" and start it
    }

    void LoadMenu()
    {
        startButton = new Button(300, 300, "PlayButton.png");
        AddChild(startButton);
        exitButton = new Button(300, 400, "ExitButton.png");
        AddChild(exitButton);
    }

    void LoadGame()
    {
        player = new Player();
        AddChild(player);

        cam = new Camera(0, 0, width, height);
        cam.scale = 1f;
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

        physicsManager = new PhysicsManager(this);

        //==== PHYSICS TESTS ====
        //SetupPhysicsTest1();
        SetupPuzzle1();
        //=======================

        foreach (LineSegment line in lines) AddChild(line);
        foreach (Ball ball in balls) AddChild(ball);
    }

    private void SetupPhysicsTest1()
    {

        Brick obj = new Brick(new Vec2(750, 275), 100, 50, isSolid: false);
        physicsManager.AddPhysicsBody(obj);
        AddChild(obj);

        Brick obj2 = new Brick(new Vec2(width / 2f, 525), width, 50, isSolid: true);
        physicsManager.AddPhysicsBody(obj2);
        AddChild(obj2);

        Brick obj3 = new Brick(new Vec2(300, 450), 25, 25, isSolid: true);
        physicsManager.AddPhysicsBody(obj3);
        AddChild(obj3);

        Brick obj4 = new Brick(new Vec2(200, 400), 200, 200, isSolid: true);
        physicsManager.AddPhysicsBody(obj4);
        AddChild(obj4);

        Brick obj5 = new Brick(new Vec2(375, 225), 150, 50, isSolid: false);
        physicsManager.AddPhysicsBody(obj5);
        AddChild(obj5);
    }

    private void SetupPuzzle1()
    {
        //Create a physics body that acts as rope
        rope = new PhysicsBody(isRope: true);

        //Add the different points of the rope
        rope.AddPoint(new Vec2(500, 50), true);
        rope.AddPoint(new Vec2(500, 100), false);
        rope.AddPoint(new Vec2(500, 150), false);
        rope.AddPoint(new Vec2(500, 200), false);
        rope.AddPoint(new Vec2(500, 250), false);
        physicsManager.AddPhysicsBody(rope);
        AddChild(rope);

        Brick danglingBlock = new Brick(new Vec2(600, 275), 200, 50);
        physicsManager.AddPhysicsBody(danglingBlock);
        AddChild(danglingBlock);

        Brick brick = new Brick(new Vec2(400, 225), 200, 50, -45);
        physicsManager.AddPhysicsBody(brick);
        AddChild(brick);

        Brick floor = new Brick(new Vec2(width / 2f, 525), width, 50, isSolid: true);
        physicsManager.AddPhysicsBody(floor);
        AddChild(floor);

        //Attach the end of the rope to the top left part of the dangling block
        physicsManager.AddConnection(new Connection(rope.points[4], danglingBlock.points[0], rope));
    }
}