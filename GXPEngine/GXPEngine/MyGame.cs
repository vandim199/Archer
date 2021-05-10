using System;									// System contains a lot of default C# libraries 
using System.Drawing;                           // System.Drawing contains a library used for canvas drawing below
using System.Collections.Generic;
using GXPEngine;								// GXPEngine contains the engine

public class MyGame : Game
{
    public Vec2 gravity = new Vec2(0, 1f);
    public float friction = 0.995f;
    public float groundFriction = 0.9f;

    public List<Ball> balls = new List<Ball>();
    public List<LineSegment> lines = new List<LineSegment>();
    PhysicsManager physicsManager;
    Camera cam;

    ColliderManager colliderManager = new ColliderManager();
    Player player;

    Button startButton;
    Button exitButton;

    bool paused;

    public MyGame() : base(1920, 1080, false)       // Create a window that's 800x600 and NOT fullscreen
    {
        LoadMenu();
    }

    void Update()
	{
        this.scale = width / 1920f;
    
        Console.WriteLine(Time.deltaTime);
        if (physicsManager != null)
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

            if(!paused)
            physicsManager.Step();
            if (Input.GetKey(Key.ZERO) && cam.scale >= 0.6f)
            {
                cam.scale -= 0.2f;
            }
            if (Input.GetKey(Key.NINE) && cam.scale <= 3)
            {
                cam.scale += 0.2f;
            }
        }

        if(player != null)
        {
            Console.WriteLine(player.position);
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
        SetupRopePhysicsTest();
        //=======================

        foreach (LineSegment line in lines) AddChild(line);
        foreach (Ball ball in balls) AddChild(ball);
    }

    private void SetupPhysicsTest1()
    {
        PhysicsBody obj = new PhysicsBody(10);
        obj.AddPoint(new Vec2(700, 250), false);
        obj.AddPoint(new Vec2(800, 250), false);
        obj.AddPoint(new Vec2(800, 300), false);
        obj.AddPoint(new Vec2(700, 300), false);
        physicsManager.AddPhysicsBody(obj);
        AddChild(obj);

        PhysicsBody obj2 = new PhysicsBody(1f);
        obj2.AddPoint(new Vec2(750, 300), true);
        obj2.AddPoint(new Vec2(750, 500), true);
        obj2.AddPoint(new Vec2(500, 500), true);
        physicsManager.AddPhysicsBody(obj2);
        AddChild(obj2);
        

        PhysicsBody obj4 = new PhysicsBody(1f);
        obj4.AddPoint(new Vec2(300, 300), true);
        obj4.AddPoint(new Vec2(100, 300), true);
        obj4.AddPoint(new Vec2(300, 500), true);
        obj4.AddPoint(new Vec2(100, 500), true);
        physicsManager.AddPhysicsBody(obj4);
        AddChild(obj4);

        PhysicsBody obj5 = new PhysicsBody(10);
        obj5.AddPoint(new Vec2(300, 200), false);
        obj5.AddPoint(new Vec2(450, 200), false);
        obj5.AddPoint(new Vec2(450, 250), false);
        obj5.AddPoint(new Vec2(300, 250), false);
        physicsManager.AddPhysicsBody(obj5);
        AddChild(obj5);

        PhysicsBody obj6 = new PhysicsBody(1f);
        obj6.AddPoint(new Vec2(300, 475), true);
        obj6.AddPoint(new Vec2(325, 475), true);
        obj6.AddPoint(new Vec2(300, 500), true);
        obj6.AddPoint(new Vec2(325, 500), true);
        physicsManager.AddPhysicsBody(obj6);
        AddChild(obj6);

        PhysicsBody obj3 = new PhysicsBody(1f);
        obj3.AddPoint(new Vec2(0, 500), true);
        obj3.AddPoint(new Vec2(width, 500), true);
        obj3.AddPoint(new Vec2(0, 550), true);
        obj3.AddPoint(new Vec2(width, 550), true);
        physicsManager.AddPhysicsBody(obj3);
        AddChild(obj3);
    }

    private void SetupRopePhysicsTest()
    {
        PhysicsBody rope = new PhysicsBody(isSequential: true);
        rope.AddPoint(new Vec2(500, 50), true);
        rope.AddPoint(new Vec2(500, 100), false);
        rope.AddPoint(new Vec2(500, 150), false);
        rope.AddPoint(new Vec2(500, 200), false);
        rope.AddPoint(new Vec2(500, 250), false);
        physicsManager.AddPhysicsBody(rope);
        AddChild(rope);

        PhysicsBody danglingBlock = new PhysicsBody();
        danglingBlock.AddPoint(new Vec2(400, 275), false);
        danglingBlock.AddPoint(new Vec2(500, 250), false);
        danglingBlock.AddPoint(new Vec2(600, 275), false);
        danglingBlock.AddPoint(new Vec2(600, 325), false);
        danglingBlock.AddPoint(new Vec2(400, 325), false);
        physicsManager.AddPhysicsBody(danglingBlock);
        AddChild(danglingBlock);

        PhysicsBody block = new PhysicsBody();
        block.AddPoint(new Vec2(350, 100), false);
        block.AddPoint(new Vec2(450, 100), false);
        block.AddPoint(new Vec2(450, 150), false);
        block.AddPoint(new Vec2(350, 150), false);
        physicsManager.AddPhysicsBody(block);
        AddChild(block);

        physicsManager.AddConnection(new Connection(rope.points[4], danglingBlock.points[1], rope));
    }
}