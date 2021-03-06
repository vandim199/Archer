using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using GXPEngine.Core;

public class Connection : GameObject
{
    public Point point1;
    public Point point2;
    public PhysicsBody physicsParent;
    public float angle
    {
        get
        {
            return (point2.position - point1.position).GetAngleDegrees();
        }
    }

    public float originalLength;

    private uint _ropeWidth = 5;

    //===== DEBUGGING =====
    private bool _showLine = false;
    private uint _width = 2;
    //=====================

    public Connection(Point newPoint1, Point newPoint2, PhysicsBody newPhysicsParent)
    {
        originalLength = (newPoint2.position - newPoint1.position).Length();
        point1 = newPoint1;
        point2 = newPoint2;
        physicsParent = newPhysicsParent;
    }

    void Update()
    {
        //Gizmos.DrawLine(point1.x, point1.y, point2.x, point2.y, game, color: 0xffffffff, width: 1);
    }

    protected override void RenderSelf(GLContext glContext)
    {
        if (_showLine || physicsParent.isRope)
        {
            uint color = 0xffffffff;
            uint width = _width;
            
            if (physicsParent.isRope)
            {
                color = 0xFF6B4E3A;
                width = _ropeWidth;
            }
            
            Gizmos.RenderLine(point1.x, point1.y, point2.x, point2.y, color, width);
        }
    }
}