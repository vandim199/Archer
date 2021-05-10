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
    public Vec2 camPosition;
    Camera cam;

    ColliderManager colliderManager = new ColliderManager();
    public Player player;

    Button startButton;
    Button exitButton;

    bool paused;

    public Sound soundLanding = new Sound("sounds/landing.mp3");
    public Sound soundArrow = new Sound("sounds/arrow swoosh.wav");
    Sprite skybox;
    Sprite skybox2;
    Sprite background;
    int skyboxWrap;

    public MyGame() : base(1920, 1080, false)       // Create a window that's 800x600 and NOT fullscreen
    {
        LoadMenu();
    }

    void Update()
    {;
        this.scale = width / 1920f;
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
            startButton.LateDestroy();
            startButton.active = false;
            LoadGame();
        }
        if (exitButton.Click())
        {
            Environment.Exit(0);
        }

        if (cam != null)
        {
            float offset = (player.position.x / skybox.width) * 0.15f;
            offset = Mathf.Ceiling(offset-1);

            cam.x = player.center.x;
            cam.y = player.center.y;
            camPosition = new Vec2(cam.x, cam.y);

            skybox.x = offset * skybox.width + cam.x * 0.85f;
            skybox2.x = skybox.x + skybox.width;
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
            if (Input.GetKey(Key.EIGHT))
            {
                cam.scale = 1;
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

        player = new Player(new Vec2(100, 300));
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

        skybox2 = new Sprite("columns.png");
        skybox2.SetOrigin(skybox2.width / 2, skybox2.height / 2);
        skybox2.y = height / 3;
        skybox2.scale = 0.7f;
        AddChild(skybox2);

        SetChildIndex(skybox, -1);
        SetChildIndex(skybox2, -2);

        //==== PHYSICS TESTS ====
        //SetupPhysicsTest1();
        //SetupPuzzle1();
        SetupPrototype();
        //ParallaxTest();
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

        Brick floor = new Brick(new Vec2(width / 2f, 525), width, 50, "pillar.png", isSolid: true, mass:0, _isFloor:true);
        physicsManager.AddPhysicsBody(floor);
        AddChild(floor);

        //Attach the end of the rope to the top left part of the dangling block
        rope.AddConnection(new Connection(rope.points[rope.points.Count - 1], danglingBlock.points[0], rope));
        //Preferably the points that you want to connect together should have the same position
    }

    private void SetupPrototype()
    {
        Brick wallLeft = new Brick(new Vec2(-35, 360), 400, 70, "stoneplatform.png", isSolid: true, mass: 0, startRotation: 90);
        physicsManager.AddPhysicsBody(wallLeft);
        AddChild(wallLeft);

        Brick floor = new Brick(new Vec2(200, 525), 400, 70, "stoneplatform.png", isSolid: true, mass: 0, _isFloor: true);
        physicsManager.AddPhysicsBody(floor);
        AddChild(floor);

        Brick floor2 = new Brick(new Vec2(1180, 525), 400, 70, "stoneplatform.png", isSolid: true, mass: 0, _isFloor: true);
        physicsManager.AddPhysicsBody(floor2);
        AddChild(floor2);

        Brick floor3 = new Brick(new Vec2(1750, 525), 400, 70, "stoneplatform.png", isSolid: true, mass: 0, _isFloor: true);
        physicsManager.AddPhysicsBody(floor3);
        AddChild(floor3);

        Brick holeLeft = new Brick(new Vec2(430, 640), 300, 70, "stoneplatform.png", isSolid: true, mass: 0, startRotation: 90);
        physicsManager.AddPhysicsBody(holeLeft);
        AddChild(holeLeft);

        Brick holeRight = new Brick(new Vec2(950, 640), 300, 70, "stoneplatform.png", isSolid: true, mass: 0, startRotation: 90);
        physicsManager.AddPhysicsBody(holeRight);
        AddChild(holeRight);

        Brick holeBottom = new Brick(new Vec2(690, 755), 450, 70, "stoneplatform.png", isSolid: true, mass: 0, _isFloor: true);
        physicsManager.AddPhysicsBody(holeBottom);
        AddChild(holeBottom);

        Brick danglingBlock = new Brick(new Vec2(430, 200), 430, 130, "pillar.png", startRotation:90);
        physicsManager.AddPhysicsBody(danglingBlock);
        AddChild(danglingBlock);

        Brick holeLeft2 = new Brick(new Vec2(1400, 650), 320, 70, "stoneplatform.png", isSolid: true, mass: 0, startRotation: 90);
        physicsManager.AddPhysicsBody(holeLeft2);
        AddChild(holeLeft2);

        Brick holeRight2 = new Brick(new Vec2(1520, 650), 320, 70, "stoneplatform.png", isSolid: true, mass: 0, startRotation: 90);
        physicsManager.AddPhysicsBody(holeRight2);
        AddChild(holeRight2);

        Brick holeBottom2 = new Brick(new Vec2(1460, 820), 190, 30, "stoneplatform.png", isSolid: true, mass: 0, _isFloor: true);
        physicsManager.AddPhysicsBody(holeBottom2);
        AddChild(holeBottom2);

        Brick danglingBlock2 = new Brick(new Vec2(1460, 330), 315, 45, "pillar.png", startRotation: 90);
        physicsManager.AddPhysicsBody(danglingBlock2);
        AddChild(danglingBlock2);

        Rope rope = new Rope(new Vec2(1435, 50), danglingBlock2.points[0].position);
        physicsManager.AddPhysicsBody(rope);
        AddChild(rope);

        rope.AddConnection(new Connection(rope.points[rope.points.Count - 1], danglingBlock2.points[0], rope));

        Checkpoint puzzle1 = new Checkpoint(new Vec2(100, 425));
        AddChild(puzzle1);

        Checkpoint puzzle2 = new Checkpoint(new Vec2(1100, 425));
        AddChild(puzzle2);
    }

    private void ParallaxTest()
    {
        Brick floor = new Brick(new Vec2(10000, 1000), 20000, 300, "stoneplatform.png", isSolid: true, mass: 0, _isFloor: true);
        physicsManager.AddPhysicsBody(floor);
        AddChild(floor);
    }
}