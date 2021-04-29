using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

class Point : Sprite
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
    public Vec2 velocity;
    private PhysicsBody _physicsParent;
    private MyGame _myGame;
    public bool isSolid;

    public Point(MyGame myGame, Vec2 startingPosition, PhysicsBody physicsParent, bool beSolid = false) : base("circle.png", false, false)
    {
        SetOrigin(width / 2f, height / 2f);
        width = 0;
        height = 0;

        isSolid = beSolid;

        _myGame = myGame;
        _physicsParent = physicsParent;

        position = startingPosition;
        oldPosition = position;
        velocity = position - oldPosition;
    }

    public void Move()
    {
        if (!isSolid)
        {
            Vec2 temp = position;
            position += position - oldPosition + (_myGame.gravity * _physicsParent.mass);
            oldPosition = temp;
        }
    }
}
