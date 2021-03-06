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
    private Sound _SFXAmbience = new Sound("sounds/ambience 1.mp3", looping:true);
    private Sound _bgm = new Sound("sounds/bgm.mp3", looping: true);
    Sprite skybox;
    Sprite skybox2;
    Sprite waterfall;
    Sprite waterfall2;
    Sprite background;
    Sprite logo;

    Sprite tips;

    public MyGame() : base(1920, 1080, false)       // Create a window that's 800x600 and NOT fullscreen
    {
        LoadMenu();
        _SFXAmbience.Play(volume:3);
        _bgm.Play(volume:0.2f);
    }

    void Update()
    {
        if (physicsManager != null && !paused)
        {
            player.grounded = false;
            physicsManager.Step();
            player.Step();
        }

        if (startButton.Click())
        {
            background.LateDestroy();
            logo.LateDestroy();
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
            if (player.position.x > 4500) LoadVictoryScreen();

            float offset = (player.position.x / skybox.width) * 0.15f;
            offset = Mathf.Ceiling(offset - 1);

            float offset2 = (player.position.x / waterfall.width) * 0.25f;
            offset2 = Mathf.Ceiling(offset2 - 1);

            cam.x = player.center.x;
            cam.y = player.center.y;
            camPosition = new Vec2(cam.x, cam.y);

            skybox.x = offset * skybox.width + cam.x * 0.85f;
            skybox2.x = skybox.x + skybox.width;

            waterfall.x = offset2 * waterfall.width + cam.x * 0.75f - 120;
            waterfall2.x = waterfall.x + waterfall.width;

            if (player.position.y > 600)
            {
                tips.visible = true;
            }
            else tips.visible = false;

            if (Input.GetKeyDown(Key.ENTER))
            {
                LoadMenu();
            }
        }
    }

    static void Main()                          // Main() is the first method that's called when the program is run
    {
        new MyGame().Start();                   // Create a "MyGame" and start it
    }

    private void Clear()
    {
        foreach (GameObject obj in GetChildren())
        {
            obj.LateRemove();
            obj.LateDestroy();
        }
    }

    void LoadMenu()
    {
        Clear();
        background = new Sprite("images/BG.png");
        background.scale = 2;
        AddChild(background);
        logo = new Sprite("images/Kasa Logo.png");
        logo.SetOrigin(logo.width / 2, logo.height / 2);
        logo.SetXY(width / 2, height / 3);
        logo.scale = 0.6f;
        AddChild(logo);
        startButton = new Button(width / 2, 600, "images/PlayButton.png");
        AddChild(startButton);
        exitButton = new Button(width / 2, 700, "images/ExitButton.png");
        AddChild(exitButton);
    }

    private void LoadVictoryScreen()
    {
        Clear();
        background = new Sprite("images/BG.png");
        background.scale = 2;
        AddChild(background);

        Sprite youWin = new Sprite("images/YouWin.png");
        youWin.SetOrigin(youWin.width / 2, youWin.height / 2);
        youWin.SetXY(width / 2, 300);
        AddChild(youWin);

        exitButton = new Button(width / 2f, 600, "images/ExitButton.png");
        exitButton.active = true;
        AddChild(exitButton);
    }

    void LoadGame()
    {
        physicsManager = new PhysicsManager(this);
        
        player = new Player(new Vec2(-1000, 400));
        physicsManager.AddPhysicsBody(player);
        
        cam = new Camera(0, 0, width, height);
        cam.scale = 1f;
        player.AddChild(cam);

        tips = new Sprite("images/Reset.png");
        tips.SetOrigin(width / 2, height / 2);

        skybox = new Sprite("images/columns.png");
        skybox.SetOrigin(skybox.width / 2, skybox.height / 2);
        skybox.y = height / 2;
        skybox.scale = 0.7f;
        AddChild(skybox);

        skybox2 = new Sprite("images/columns.png");
        skybox2.SetOrigin(skybox2.width / 2, skybox2.height / 2);
        skybox2.y = height / 2;
        skybox2.scale = 0.7f;
        AddChild(skybox2);

        waterfall = new Sprite("images/waterfallsketch.png");
        waterfall.SetOrigin(waterfall.width / 2, waterfall.height / 2);
        waterfall.y = height / 2.5f;
        waterfall.scale = 0.8f;
        AddChild(waterfall);

        waterfall2 = new Sprite("images/waterfallsketch.png");
        waterfall2.SetOrigin(waterfall2.width / 2, waterfall2.height / 2);
        waterfall2.y = height / 2.5f;
        waterfall2.scale = 0.8f;
        AddChild(waterfall2);

        //skybox.alpha = 0;
        //skybox2.alpha = 0;

        Sprite overlay = new Sprite("images/Layout33.png");
        overlay.SetXY(-1320-830, -325);
        //overlay.scale = 1.867f;
        //AddChild(overlay);

        SetChildIndex(skybox, 1);
        SetChildIndex(skybox2, 0);

        //SetChildIndex(waterfall, 2);
        //SetChildIndex(waterfall2, 3);
        //SetChildIndex(overlay, 100000);

        //==== PHYSICS TESTS ====
        //SetupPhysicsTest1();
        //SetupPuzzle1();
        SetupPrototype();
        //ParallaxTest();
        //=======================

        AddChild(player);
        AddChild(overlay);
        cam.AddChild(tips);

        foreach (LineSegment line in lines) AddChild(line);
        foreach (Ball ball in balls) AddChild(ball);
    }

    private void SetupPhysicsTest1()
    {

        Brick obj = new Brick(new Vec2(750, 275), 100, 50, "square.png", isSolid: false);
        physicsManager.AddPhysicsBody(obj);
        AddChild(obj);

        Brick obj2 = new Brick(new Vec2(width / 2f, 525), width, 50, "square.png", isSolid: true);
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

        Brick floor = new Brick(new Vec2(width / 2f, 525), width, 50, "pillar.png", isSolid: true, mass: 0);
        physicsManager.AddPhysicsBody(floor);
        AddChild(floor);

        //Attach the end of the rope to the top left part of the dangling block
        rope.AddConnection(new Connection(rope.points[rope.points.Count - 1], danglingBlock.points[0], rope));
        //Preferably the points that you want to connect together should have the same position
    }

    private void SetupPrototype()
    {
        #region ceilings
        Brick ceiling = new Brick(new Vec2(-900, 0), 600, 200, "images/stoneplatform.png", isSolid: true, mass: 0);
        physicsManager.AddPhysicsBody(ceiling);
        AddChild(ceiling);

        Brick ceiling1 = new Brick(new Vec2(1500, -100), 4200, 200, "images/stoneplatform.png", isSolid: true, mass: 0);
        physicsManager.AddPhysicsBody(ceiling1);
        AddChild(ceiling1);
        
        Brick ceiling2 = new Brick(new Vec2(4200, 100), 800, 100, "images/stoneplatform.png", isSolid: true, mass: 0);
        physicsManager.AddPhysicsBody(ceiling2);
        AddChild(ceiling2);

        Brick ceiling3 = new Brick(new Vec2(4750, 230), 400, 50, "images/stoneplatform.png", isSolid: true, mass: 0, startRotation: 35);
        physicsManager.AddPhysicsBody(ceiling3);
        AddChild(ceiling3);

        Brick ceiling4 = new Brick(new Vec2(4900, 500), 400, 50, "images/stoneplatform.png", isSolid: true, mass: 0, startRotation: 90);
        physicsManager.AddPhysicsBody(ceiling4);
        AddChild(ceiling4);

        Brick ceiling5 = new Brick(new Vec2(3700, 50), 280, 50, "images/stoneplatform.png", isSolid: true, mass: 0, startRotation: 35);
        physicsManager.AddPhysicsBody(ceiling5);
        AddChild(ceiling5);
        #endregion

        #region floors
        Brick LeftWall = new Brick(new Vec2(-1250, 390), 600, 100, "images/stoneplatform.png", isSolid: true, mass: 0, startRotation: 90);
        physicsManager.AddPhysicsBody(LeftWall);
        AddChild(LeftWall);

        Brick floor00 = new Brick(new Vec2(-900, 690), 600, 200, "images/stoneplatform.png", isSolid: true, mass: 0);
        physicsManager.AddPhysicsBody(floor00);
        AddChild(floor00);

        Brick floor0 = new Brick(new Vec2(-300, 690), 600, 200, "images/stoneplatform.png", isSolid: true, mass: 0);
        physicsManager.AddPhysicsBody(floor0);
        AddChild(floor0);

        Brick floor = new Brick(new Vec2(165, 540), 600, 100, "images/stoneplatform.png", isSolid: true, mass: 0);
        physicsManager.AddPhysicsBody(floor);
        AddChild(floor);

        Brick floor2 = new Brick(new Vec2(1160, 525), 500, 70, "images/stoneplatform.png", isSolid: true, mass: 0);
        physicsManager.AddPhysicsBody(floor2);
        AddChild(floor2);

        Brick floor3 = new Brick(new Vec2(1750, 525), 500, 70, "images/stoneplatform.png", isSolid: true, mass: 0);
        physicsManager.AddPhysicsBody(floor3);
        AddChild(floor3);

        Brick floor4 = new Brick(new Vec2(2250, 525), 500, 70, "images/stoneplatform.png", isSolid: true, mass: 0);
        physicsManager.AddPhysicsBody(floor4);
        AddChild(floor4);

        Brick floor5 = new Brick(new Vec2(3200, 525), 500, 70, "images/stoneplatform.png", isSolid: true, mass: 0);
        physicsManager.AddPhysicsBody(floor5);
        AddChild(floor5);

        Brick floor6 = new Brick(new Vec2(3700, 525), 500, 70, "images/stoneplatform.png", isSolid: true, mass: 0);
        physicsManager.AddPhysicsBody(floor6);
        AddChild(floor6);

        Brick floor7 = new Brick(new Vec2(4050, 605), 300, 70, "images/stoneplatform.png", isSolid: true, mass: 0, startRotation: 35);
        physicsManager.AddPhysicsBody(floor7);
        AddChild(floor7);

        Brick floor8 = new Brick(new Vec2(4530, 685), 700, 70, "images/stoneplatform.png", isSolid: true, mass: 0);
        physicsManager.AddPhysicsBody(floor8);
        AddChild(floor8);
        #endregion

        #region puzzle1
        Brick tipOver = new Brick(new Vec2(250, 430), 120, 120, "images/blocksketch.png", isSolid: true, mass: 0);
        physicsManager.AddPhysicsBody(tipOver);
        AddChild(tipOver);

        Brick holeLeft = new Brick(new Vec2(430, 690), 200, 70, "images/stoneplatform.png", isSolid: true, mass: 0, startRotation: 90);
        physicsManager.AddPhysicsBody(holeLeft);
        AddChild(holeLeft);

        Brick holeRight = new Brick(new Vec2(950, 675), 230, 70, "images/stoneplatform.png", isSolid: true, mass: 0, startRotation: 90);
        physicsManager.AddPhysicsBody(holeRight);
        AddChild(holeRight);

        Brick holeBottom = new Brick(new Vec2(690, 755), 450, 70, "images/stoneplatform.png", isSolid: true, mass: 0);
        physicsManager.AddPhysicsBody(holeBottom);
        AddChild(holeBottom);

        Brick danglingBlock = new Brick(new Vec2(400, 275), 430, 130, "images/plank_withered.png", startRotation: 90);
        physicsManager.AddPhysicsBody(danglingBlock);
        AddChild(danglingBlock);
        #endregion

        #region puzzle2
        Brick holeLeft2 = new Brick(new Vec2(1375, 685), 250, 70, "images/stoneplatform.png", isSolid: true, mass: 0, startRotation: 90);
        physicsManager.AddPhysicsBody(holeLeft2);
        AddChild(holeLeft2);

        Brick holeRight2 = new Brick(new Vec2(1535, 685), 250, 70, "images/stoneplatform.png", isSolid: true, mass: 0, startRotation: 90);
        physicsManager.AddPhysicsBody(holeRight2);
        AddChild(holeRight2);

        Brick holeBottom2 = new Brick(new Vec2(1455, 820), 230, 30, "images/stoneplatform.png", isSolid: true, mass: 0);
        physicsManager.AddPhysicsBody(holeBottom2);
        AddChild(holeBottom2);

        Brick danglingBlock2 = new Brick(new Vec2(1460, 330), 315, 65, "images/plank_withered.png", startRotation: 90);
        physicsManager.AddPhysicsBody(danglingBlock2);
        AddChild(danglingBlock2);

        Rope rope = new Rope(new Vec2(1435, -50), danglingBlock2.points[0].position, segmentLength: 50);
        physicsManager.AddPhysicsBody(rope);
        AddChild(rope);

        rope.AddConnection(new Connection(rope.points[rope.points.Count - 1], danglingBlock2.points[0], rope));

        Brick wallRight0 = new Brick(new Vec2(2100, 125), 280, 70, "images/stoneplatform.png", isSolid: true, mass: 0, startRotation: 90);
        physicsManager.AddPhysicsBody(wallRight0);
        AddChild(wallRight0);
        #endregion

        #region puzzle3
        Brick holeLeft3 = new Brick(new Vec2(2465, 675), 230, 70, "images/stoneplatform.png", isSolid: true, mass: 0, startRotation: 90);
        physicsManager.AddPhysicsBody(holeLeft3);
        AddChild(holeLeft3);

        Brick holeRight3 = new Brick(new Vec2(2985, 675), 230, 70, "images/stoneplatform.png", isSolid: true, mass: 0, startRotation: 90);
        physicsManager.AddPhysicsBody(holeRight3);
        AddChild(holeRight3);

        Brick holeBottom3 = new Brick(new Vec2(2725, 755), 450, 70, "images/stoneplatform.png", isSolid: true, mass: 0);
        physicsManager.AddPhysicsBody(holeBottom3);
        AddChild(holeBottom3);

        Brick wallRight = new Brick(new Vec2(3285, 125), 280, 70, "images/stoneplatform.png", isSolid: true, mass: 0, startRotation: 90);
        physicsManager.AddPhysicsBody(wallRight);
        AddChild(wallRight);

        Brick danglingBlock3 = new Brick(new Vec2(2760, 290), 200, 200, "images/blocksketch.png", startRotation: 90);
        physicsManager.AddPhysicsBody(danglingBlock3);
        AddChild(danglingBlock3);

        Rope rope2 = new Rope(new Vec2(2735, -50), danglingBlock3.points[0].position, segmentLength: 50);
        physicsManager.AddPhysicsBody(rope2);
        AddChild(rope2);

        rope2.AddConnection(new Connection(rope2.points[rope2.points.Count - 1], danglingBlock3.points[0], rope2));

        Brick danglingBlock4 = new Brick(new Vec2(3000, 200), 400, 100, "images/plank_withered.png", startRotation: 90);
        physicsManager.AddPhysicsBody(danglingBlock4);
        AddChild(danglingBlock4);
        #endregion

        #region Checkpoints
        Checkpoint puzzle1 = new Checkpoint(new Vec2(50, 370));
        AddChild(puzzle1);

        Checkpoint puzzle2 = new Checkpoint(new Vec2(1100, 370));
        AddChild(puzzle2);

        Checkpoint puzzle3 = new Checkpoint(new Vec2(2200, 370));
        AddChild(puzzle3);
        #endregion Checkpoints
    }

    private void ParallaxTest()
    {
        Brick floor = new Brick(new Vec2(8000, 1000), 20000, 300, "images/stoneplatform.png", isSolid: true, mass: 0);
        physicsManager.AddPhysicsBody(floor);
        AddChild(floor);
    }
}