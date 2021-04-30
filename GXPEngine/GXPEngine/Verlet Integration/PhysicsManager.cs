using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

class PhysicsManager : GameObject
{
    List<PhysicsBody> _physicsBodies;
    List<Connection> _extraConnections;
    private MyGame _myGame;

    private VerletCollisionInfo _collisionInfo;

    public PhysicsManager(MyGame myGame)
    {
        _myGame = myGame;
        _physicsBodies = new List<PhysicsBody>();
        _collisionInfo = new VerletCollisionInfo();
        _extraConnections = new List<Connection>();
    }

    public void Step()
    {
        UpdatePoints();
        IterateCollisions();

        foreach(Connection extra in _extraConnections)
        {
            UpdateConnection(extra);
        }
    }

    void IterateCollisions()
    {
        if (_physicsBodies != null && _physicsBodies.Count > 0)
        {
            foreach (PhysicsBody pb in _physicsBodies)
            {
                UpdateConnections(pb);

                foreach (PhysicsBody other in _physicsBodies)
                {
                    if (pb != other)
                    {
                        if (DetectCollision(pb, other))
                        {
                            ProcessCollision();
                        }
                    }
                }
            }
        }
    }

    public void AddPhysicsBody(PhysicsBody pb)
    {
        _physicsBodies.Add(pb);
    }

    public void AddConnection(Connection connection)
    {
        _extraConnections.Add(connection);
    }

    void UpdatePoints()
    {
        foreach (PhysicsBody pb in _physicsBodies)
        {
            for (int i = 0; i < pb.points.Count; i++)
            {
                Point p = pb.points[i];

                p.Move();
            }
        }
    }

    void UpdateConnections(PhysicsBody pb)
    {
        for (int i = 0; i < 100; i++)
        {
            for (int a = 0; a < pb.connections.Count; a++)
            {
                UpdateConnection(pb.connections[a]);
            }
        }
    }

    void UpdateConnection(Connection c)
    {
        Vec2 distance = c.point2.position - c.point1.position;

        float distanceLength = distance.Length();

        float difference = distanceLength - c.originalLength;

        distance.Normalize();


        if (!c.point1.isSolid && !c.point2.isSolid)
        {
            c.point1.position += distance * difference * 0.5f;
            c.point2.position -= distance * difference * 0.5f;
        }
        else if (!c.point1.isSolid && c.point2.isSolid)
        {
            c.point1.position += distance * difference;
        }
        else if (c.point1.isSolid && !c.point2.isSolid)
        {
            c.point2.position -= distance * difference;
        }
    }

    bool DetectCollision(PhysicsBody pb1, PhysicsBody pb2)
    {
        float minLength = 10000.0f;

        for (int i = 0; i < pb1.connections.Count + pb2.connections.Count; i++)
        {
            Connection c;

            if (i < pb1.connections.Count)
            {
                c = pb1.connections[i];
            }
            else
            {
                c = pb2.connections[i - pb1.connections.Count];
            }

            Vec2 axis = new Vec2(c.point1.y - c.point2.y, c.point2.x - c.point1.x);
            axis.Normalize();

            float minA, maxA, minB, maxB;

            pb1.ProjectToAxis(axis, out minA, out maxA);
            pb2.ProjectToAxis(axis, out minB, out maxB);

            float distance = IntervalDistance(minA, maxA, minB, maxB);

            if (distance > 0.0f)
            {
                return false;
            }
            else if (Mathf.Abs(distance) < minLength)
            {
                minLength = Mathf.Abs(distance);
                _collisionInfo.normal = axis;
                _collisionInfo.c = c;
            }
        }

        _collisionInfo.depth = minLength;

        if (_collisionInfo.c.physicsParent != pb2)
        {
            PhysicsBody temp = pb2;
            pb2 = pb1;
            pb1 = temp;
        }

        int sign = Mathf.Sign(_collisionInfo.normal.Dot(pb1.center - pb2.center));

        if (sign != 1)
        {
            _collisionInfo.normal = -_collisionInfo.normal;
        }

        float smallestD = 10000.0f;

        for (int i = 0; i < pb1.points.Count; i++)
        {
            float distance = _collisionInfo.normal.Dot(pb1.points[i].position - pb2.center);

            if (distance < smallestD)
            {
                smallestD = distance;
                _collisionInfo.p = pb1.points[i];
            }
        }

        return true;
    }

    void ProcessCollision()
    {
        Vec2 collisionVector = _collisionInfo.normal * _collisionInfo.depth;

        Point e1 = _collisionInfo.c.point1;
        Point e2 = _collisionInfo.c.point2;
        e1.isColliding = true;
        e2.isColliding = true;

        float t;
        if (Mathf.Abs(e1.position.x - e2.position.x) > Mathf.Abs(e1.position.y - e2.position.y))
        {
            t = (_collisionInfo.p.position.x - collisionVector.x - e1.position.x) / (e2.position.x - e1.position.x);
        }
        else
        {
            t = (_collisionInfo.p.position.y - collisionVector.y - e1.position.y) / (e2.position.y - e1.position.y);
        }

        float lambda = 1.0f / (t * t + (1 - t) * (1 - t));

        if (!e1.isSolid && !e2.isSolid)
        {
            e1.position -= collisionVector * (1 - t) * 0.5f * lambda;
            e2.position -= collisionVector * t * 0.5f * lambda;
        }
        else if (!e1.isSolid && e2.isSolid)
        {
            e1.position -= collisionVector * (1 - t) * lambda;
        }
        else if (e1.isSolid && !e2.isSolid)
        {
            e2.position -= collisionVector * t * lambda;
        }

        if (!_collisionInfo.p.isSolid)
        {
            _collisionInfo.p.position += collisionVector * 0.5f;
        }
    }

    float IntervalDistance(float minA, float maxA, float minB, float maxB)
    {
        if (minA < minB)
        {
            return minB - maxA;
        }
        else
        {
            return minA - maxB;
        }
    }
}