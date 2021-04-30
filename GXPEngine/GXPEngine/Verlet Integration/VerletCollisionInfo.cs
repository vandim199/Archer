using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class VerletCollisionInfo
{
    public float depth;
    public Vec2 normal;
    public Connection c;
    public Point p;
    public float timeOfImpact;

    public VerletCollisionInfo()
    {

    }

    public VerletCollisionInfo(Vec2 normal, Connection connection, Point point, float timeOfImpact, float depth = 0)
    {
        this.normal = normal;
        c = connection;
        p = point;
        this.timeOfImpact = timeOfImpact;
        this.depth = depth;
    }
}
