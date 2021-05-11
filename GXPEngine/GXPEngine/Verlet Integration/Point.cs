using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

public class Point : Sprite
{
    public Vec2 oldPosition;
    public Vec2 position
    {
        get
        {
            return new Vec2(x, y);
        }
        set
        {
            x = value.x;
            y = value.y;
        }
    }

    public PhysicsBody physicsParent { get; private set; }
    private MyGame _myGame;
    public bool isSolid;
    public bool isColliding;

    //===== DEBUGGING =====
    private int _radius = 5;
    //=====================

    public Point(MyGame myGame, Vec2 startingPosition, PhysicsBody physicsParent, bool beSolid = false) : base("circle.png", false, false)
    {
        SetOrigin(width / 2f, height / 2f);
        width = _radius;
        height = _radius;

        isSolid = beSolid;

        _myGame = myGame;
        this.physicsParent = physicsParent;

        position = startingPosition;
        oldPosition = position;
    }

    public void Move()
    {
        if (!isSolid)
        {
            Vec2 temp = position;
            Vec2 velocity = (position - oldPosition) * _myGame.friction;

            position += velocity + (_myGame.gravity / physicsParent.mass);
            oldPosition = temp;
        }
        isColliding = false;
    }
}
