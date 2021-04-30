using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

class PhysicsBody : GameObject
{
    public List<Point> points;
    public List<Connection> connections;
    public Vec2 center
    {
        get
        {
            Vec2 total = new Vec2(0, 0);

            foreach (Point point in points)
            {
                total += point.position;
            }

            return total / points.Count;
        }
    }
    public float mass;
    private bool isSequential;

    public PhysicsBody(float newMass = 10, bool isSequential = false)
    {
        points = new List<Point>();
        connections = new List<Connection>();
        mass = newMass;
        this.isSequential = isSequential;
    }

    public void ProjectToAxis(Vec2 axis, out float min, out float max)
    {
        float dotP = axis.Dot(points[0].position);

        min = max = dotP;

        for (int i = 1; i < points.Count; i++)
        {
            dotP = axis.Dot(points[i].position);

            min = Mathf.Min(dotP, min);
            max = Mathf.Max(dotP, max);
        }
    }

    public void AddPoint(Vec2 pointPosition, bool solid)
    {
        Point point = new Point((MyGame)game, pointPosition, this, solid);

        if(points != null && points.Count > 0)
        {
            if (!isSequential)
            {
                foreach (Point p in points)
                {
                    AddConnection(new Connection(point, p, this));
                }
            }
            else
            {
                AddConnection(new Connection(point, points[points.Count - 1], this));
            }
        }

        points.Add(point);

        AddChild(point);
    }

    public void AddConnection(Connection connection)
    {
        connections.Add(connection);
        AddChild(connection);
    }
}
