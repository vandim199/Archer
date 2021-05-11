using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
public class PhysicsManager : GameObject
{
    public List<PhysicsBody> physicsBodies { get; private set; }
    List<Connection> _extraConnections;
    private MyGame _myGame;

    private VerletCollisionInfo _collisionInfo;

    private static Sound SFXPlayerLanding = new Sound("sounds/short landing.wav");

    public PhysicsManager(MyGame myGame)
    {
        _myGame = myGame;
        physicsBodies = new List<PhysicsBody>();
        _collisionInfo = new VerletCollisionInfo();
        _extraConnections = new List<Connection>();
    }

    public void Step()
    {
        UpdatePoints();
        IterateCollisions();

        foreach (Connection extra in _extraConnections)
        {
            UpdateConnection(extra);
        }
    }

    void IterateCollisions()
    {
        if (physicsBodies != null && physicsBodies.Count > 0)
        {
            foreach (PhysicsBody pb in physicsBodies)
            {
                UpdateConnections(pb);

                if (!pb.isRope)
                {
                    foreach (PhysicsBody other in physicsBodies)
                    {
                        if (!other.isRope)
                        {
                            if (pb != other)
                            {
                                if (DetectCollision(pb, other))
                                {
                                    ProcessCollision(_collisionInfo);
                                }
                            }
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

    public void AddConnection(Connection connection)
    {
        _extraConnections.Add(connection);
        _myGame.AddChild(connection);
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

    public static void ProcessCollision(VerletCollisionInfo colInfo)
    {
        Vec2 collisionVector = colInfo.normal * colInfo.depth;

        Point e1 = colInfo.c.point1;
        Point e2 = colInfo.c.point2;
        e1.isColliding = true;
        e2.isColliding = true;

        float t;
        if (Mathf.Abs(e1.position.x - e2.position.x) > Mathf.Abs(e1.position.y - e2.position.y))
        {
            t = (colInfo.p.position.x - collisionVector.x - e1.position.x) / (e2.position.x - e1.position.x);
        }
        else
        {
            t = (colInfo.p.position.y - collisionVector.y - e1.position.y) / (e2.position.y - e1.position.y);
        }

        float cMultiplier = 1;
        float pMultiplier = 1;

        if (colInfo.c.physicsParent.isPlayer)
        {
            cMultiplier = 2;
            pMultiplier = 0;
        }
        else if (colInfo.p.physicsParent.isPlayer)
        {
            cMultiplier = 0;
            pMultiplier = 2;
        }

        float lambda = 1.0f / (t * t + (1 - t) * (1 - t));

        if (!e1.isSolid && !e2.isSolid)
        {
            e1.position -= collisionVector * (1 - t) * 0.5f * lambda * cMultiplier;
            e2.position -= collisionVector * t * 0.5f * lambda * cMultiplier;
        }
        else if (!e1.isSolid && e2.isSolid)
        {
            e1.position -= collisionVector * (1 - t) * lambda * cMultiplier;
        }
        else if (e1.isSolid && !e2.isSolid)
        {
            e2.position -= collisionVector * t * lambda * cMultiplier;
        }

        if (!colInfo.p.isSolid)
        {
            colInfo.p.position += collisionVector * 0.5f * pMultiplier;
            colInfo.p.position += new Vec2((colInfo.p.oldPosition.x - colInfo.p.position.x) * 0.1f, 0);
        }

        colInfo.c.physicsParent.OnCollided(colInfo.c, colInfo.p);


        Player player;
        if ((colInfo.c.angle > -75 && colInfo.c.angle < 75) || (colInfo.c.angle > 105 || colInfo.c.angle < -105))
        {
            if (colInfo.c.physicsParent is Player)
            {
                player = colInfo.c.physicsParent as Player;

                float averageCHeight = (colInfo.c.point1.y + colInfo.c.point2.y) / 2f;
                if (averageCHeight > colInfo.c.physicsParent.center.y + 20)
                {
                    player.grounded = true;
                    if (player.velocity.y > 7)
                    {
                        SFXPlayerLanding.Play();
                    }
                }
            }
            else if (colInfo.p.physicsParent is Player)
            {
                player = colInfo.p.physicsParent as Player;
                if (colInfo.p.y > colInfo.p.physicsParent.center.y + 20)
                {
                    player.grounded = true;
                    if(player.velocity.y > 7)
                    {
                        SFXPlayerLanding.Play();
                    }
                }
            }
        }
    }

    public static float IntervalDistance(float minA, float maxA, float minB, float maxB)
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