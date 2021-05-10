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

    public Sound soundLanding = new Sound("sounds/landing.mp3");
    public Sound soundArrow = new Sound("sounds/arrow swoosh.wav");
    Sprite skybox;
    Sprite background;

    public MyGame() : base(1920, 1080, false)       // Create a window that's 800x600 and NOT fullscreen
    {
        LoadMenu();
    }

    void Update()
    {;
        this.scale = width / 1920f;
        Console.WriteLine(currentFps);
        if (physicsManager != null && !paused)
        {
            player.grounded = false;
            physicsManager.Step();
            player.Step();
        }

        if (startButton.Click())
        {
            background.LateDestroy();
            exitButton.LateDestroy();
            exitButton.active = false;
            LoadGame();
            startButton.LateDestroy();
            startButton.active = false;
        }
        if (exitButton.Click())
        {
            Environment.Exit(0);
        }

        if (cam != null)
        {
            cam.x = player.center.x;
            cam.y = player.center.y;
            skybox.x = cam.x;
            if (Input.GetKeyDown(Key.ENTER))
            {
                foreach (GameObject obj in GetChildren())
                {
                    obj.LateRemove();
                    obj.LateDestroy();
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
    }

    static void Main()                          // Main() is the first method that's called when the program is run
    {
        new MyGame().Start();                   // Create a "MyGame" and start it
    }

    void LoadMenu()
    {
        background = new Sprite("BG.png");
        background.scale = 2;
        AddChild(background);
        startButton = new Button(width/2, 600, "PlayButton.png");
        AddChild(startButton);
        exitButton = new Button(width/2, 700, "ExitButton.png");
        AddChild(exitButton);
    }

    void LoadGame()
    {
        physicsManager = new PhysicsManager(this);

        player = new Player(new Vec2(200, 300));
        physicsManager.AddPhysicsBody(player);
        AddChild(player);

        cam = new Camera(0, 0, width, height);
        cam.scale = 1f;
        player.AddChild(cam);

        skybox = new Sprite("columns.png");
        skybox.SetOrigin(skybox.width / 2, skybox.height / 2);
        skybox.y = height / 3;
        skybox.scale = 0.7f;
        AddChild(skybox);
        SetChildIndex(skybox, 0);

        //lines.Add(new LineSegment(this, 0, 0, width, 0));
        //lines.Add(new LineSegment(this, 0, 510, 0, 200));
        //lines.Add(new LineSegment(this, width, 0, width, height));
        //lines.Add(new LineSegment(this, 300, 500, -10, 500, newFloor: true));
        //lines.Add(new LineSegment(this, 300, 600, 300, 500));
        //lines.Add(new LineSegment(this, 850, 500, 500, 500, newFloor: true));
        //lines.Add(new LineSegment(this, 500, 500, 500, 600));
        //lines.Add(new LineSegment(this, 800, 1000, 0, 1000, newFloor: true));

        //lines.Add(new LineSegment(this, 1200, 420, 800, 510, newFloor: true));
        //lines.Add(new LineSegment(this, 400, 450, 0, 400, newFloor: true));

        //AddChild(new Box());

        //==== PHYSICS TESTS ====
        //SetupPhysicsTest1();
        SetupPuzzle1();
        //=======================

        foreach (LineSegment line in lines) AddChild(line);
        foreach (Ball ball in balls) AddChild(ball);
    }

    private void SetupPhysicsTest1()
    {

        Brick obj = new Brick(new Vec2(750, 275), 100, 50, "square.png", isSolid: false);
        physicsManager.AddPhysicsBody(obj);
        AddChild(obj);

        Brick obj2 = new Brick(new Vec2(width / 2f, 525), width, 50, "square.png", isSolid: true, _isFloor:true);
        physicsManager.AddPhysicsBody(obj2);
        AddChild(obj2);

        Brick obj3 = new Brick(new Vec2(300, 450), 25, 25, "square.png", isSolid: true);
        physicsManager.AddPhysicsBody(obj3);
        AddChild(obj3);

        Brick obj4 = new Brick(new Vec2(200, 400), 200, 200, "square.png", isSolid: true);
        physicsManager.AddPhysicsBody(obj4);
        AddChild(obj4);

        Brick obj5 = new Brick(new Vec2(375, 225), 150, 50, "square.png", isSolid: false);
        physicsManager.AddPhysicsBody(obj5);
        AddChild(obj5);
    }

    private void SetupPuzzle1()
    {
        Brick danglingBlock = new Brick(new Vec2(600, 275), 200, 50, "square.png");
        physicsManager.AddPhysicsBody(danglingBlock);
        AddChild(danglingBlock);

        Rope rope = new Rope(new Vec2(500, 50), danglingBlock.points[0].position);
        physicsManager.AddPhysicsBody(rope);
        AddChild(rope);

        Brick brick = new Brick(new Vec2(400, 225), 200, 50, "square.png", -45);
        physicsManager.AddPhysicsBody(brick);
        AddChild(brick);

        Brick floor = new Brick(new Vec2(width / 2f, 525), width, 50, "square.png", isSolid: true, mass:0, _isFloor:true);
        physicsManager.AddPhysicsBody(floor);
        AddChild(floor);

        //Attach the end of the rope to the top left part of the dangling block
        rope.AddConnection(new Connection(rope.points[rope.points.Count - 1], danglingBlock.points[0], rope));
        //Preferably the points that you want to connect together should have the same position
    }
}