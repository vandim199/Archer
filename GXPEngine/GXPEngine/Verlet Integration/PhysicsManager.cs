using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

class PhysicsManager : GameObject
{
    List<PhysicsBody> physicsBodies;
    private MyGame _myGame;

    private VerletCollisionInfo collisionInfo;

    public PhysicsManager(MyGame myGame)
    {
        _myGame = myGame;
        physicsBodies = new List<PhysicsBody>();
        collisionInfo = new VerletCollisionInfo();
    }

    public void Step()
    {
        UpdatePoints();
        IterateCollisions();
    }

    void IterateCollisions()
    {
        if (physicsBodies != null && physicsBodies.Count > 0)
        {
            foreach (PhysicsBody pb in physicsBodies)
            {
                UpdateConnections(pb);

                foreach (PhysicsBody other in physicsBodies)
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
        physicsBodies.Add(pb);
    }

    void UpdatePoints()
    {
        foreach (PhysicsBody pb in physicsBodies)
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
                Connection c = pb.connections[a];

                Vec2 distance = c.point2.position - c.point1.position;

                float distanceLength = distance.Length();

                float difference = distanceLength - c.originalLength;

                distance.Normalize();


                if(!c.point1.isSolid && !c.point2.isSolid)
                {
                    c.point1.position += distance * difference * 0.5f;
                    c.point2.position -= distance * difference * 0.5f;
                }
                else if(!c.point1.isSolid && c.point2.isSolid)
                {
                    c.point1.position += distance * difference;
                }
                else if(c.point1.isSolid && !c.point2.isSolid)
                {
                    c.point2.position -= distance * difference;
                }
            }
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

            Vec2 axis = new Vec2(c.point1.x - c.point2.x, c.point1.y - c.point2.y);
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
                collisionInfo.normal = axis;
                collisionInfo.c = c;
            }
        }

        collisionInfo.depth = minLength;

        if (collisionInfo.c.physicsParent != pb2)
        {
            PhysicsBody temp = pb2;
            pb2 = pb1;
            pb1 = temp;
        }

        int sign = Mathf.Sign(collisionInfo.normal.Dot(pb1.center - pb2.center));

        if (sign != 1)
        {
            collisionInfo.normal = -collisionInfo.normal;
        }

        float smallestD = 10000.0f;

        for (int i = 0; i < pb1.points.Count; i++)
        {
            float distance = collisionInfo.normal.Dot(pb1.points[i].position - pb2.center);

            if (distance < smallestD)
            {
                smallestD = distance;
                collisionInfo.p = pb1.points[i];
            }
        }

        return true;
    }

    void ProcessCollision()
    {
        Vec2 collisionVector = collisionInfo.normal * collisionInfo.depth;

        Point e1 = collisionInfo.c.point1;
        Point e2 = collisionInfo.c.point2;

        float t;
        if (Mathf.Abs(e1.position.x - e2.position.x) > Mathf.Abs(e1.position.y - e2.position.y))
        {
            t = (collisionInfo.p.position.x - collisionVector.x - e1.position.x) / (e2.position.x - e1.position.x);
        }
        else
        {
            t = (collisionInfo.p.position.y - collisionVector.y - e1.position.y) / (e2.position.y - e1.position.y);
        }

        float lambda = 1.0f / (t * t + (1 - t) * (1 - t));

        if(!e1.isSolid && !e2.isSolid)
        {
            e1.position -= collisionVector * (1 - t) * 0.5f * lambda;
            e2.position -= collisionVector * t * 0.5f * lambda;
        }
        else if(!e1.isSolid && e2.isSolid)
        {
            e1.position -= collisionVector * (1 - t) * lambda;
        }
        else if(e1.isSolid && !e2.isSolid)
        {
            e2.position -= collisionVector * t * lambda;
        }

        if (!collisionInfo.p.isSolid)
        {
            collisionInfo.p.position += collisionVector * 0.5f;
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