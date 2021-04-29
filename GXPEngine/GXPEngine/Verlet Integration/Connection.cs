using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

class Connection : GameObject
{
    public Point point1;
    public Point point2;
    public PhysicsBody physicsParent;

    public float originalLength;

    public Connection(Point newPoint1, Point newPoint2, PhysicsBody newPhysicsParent)
    {
        originalLength = (newPoint2.position - newPoint1.position).Length();
        point1 = newPoint1;
        point2 = newPoint2;
        physicsParent = newPhysicsParent;
    }

    void Update()
    {
        Gizmos.DrawLine(point1.x, point1.y, point2.x, point2.y, color: 0xffffffff, width: 1);
    }
}