using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    class Projectile : PhysicsBody
    {
        private MyGame _myGame; //Reference to MyGame
        private Vec2 _velocity; //The velocity
        private Sprite _arrowGraphics; //The visuals of the arrow

        private Vec2 _oldPosition; //The old position
        private Vec2 _position; //The current position

        public bool hasBounced = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="myGame">Reference to MyGame</param>
        /// <param name="startingVelocity">The starting velocity of the arrow</param>
        /// <param name="startingPosition">The starting position of the arrow</param>
        public Projectile(MyGame myGame, Vec2 startingVelocity, Vec2 startingPosition)
        {
            _position = startingPosition;
            _velocity = startingVelocity;
            _myGame = myGame;

            _arrowGraphics = new Sprite("Arrow.png", addCollider: false);
            _arrowGraphics.SetOrigin(_arrowGraphics.width, _arrowGraphics.height / 2f);
            _arrowGraphics.SetXY(_position.x, _position.y);
            _arrowGraphics.rotation = _velocity.GetAngleDegrees();
            AddChild(_arrowGraphics);

            AddPoint(new Vec2(0, 0), false);
            //_collider = new Point(_myGame, new Vec2(0, 0), this);
            //myGame.AddChild(_collider);
        }

        void Update()
        {
            Move();

            bool collidedWithRope;
            VerletCollisionInfo collisionInfo = GetEarliestCollision(out collidedWithRope);

            if (collisionInfo != null)
            {
                if (collidedWithRope)
                {
                    ResolveCollision(collisionInfo);
                }
                else
                {
                    PhysicsManager.ProcessCollision(collisionInfo);

                    if (((collisionInfo.c.physicsParent.bounceArrow && collisionInfo.c.physicsParent != this)
                        || (collisionInfo.p.physicsParent.bounceArrow && collisionInfo.p.physicsParent != this))
                        && !hasBounced)
                    {
                        hasBounced = true;
                        _velocity.Reflect(collisionInfo.normal, 1);
                    }
                    else
                    {
                        this.LateDestroy();
                    }
                }
            }
        }

        /// <summary>
        /// Move the arrow
        /// </summary>
        private void Move()
        {
            _velocity += _myGame.gravity * 1.5f;
            _oldPosition = _position;
            _position += _velocity;
            _arrowGraphics.x = _position.x;
            _arrowGraphics.y = _position.y;
            points[0].position = _position;
            _arrowGraphics.rotation = _velocity.GetAngleDegrees();
        }

        /// <summary>
        /// Get the earliest collision of the arrow.
        /// </summary>
        /// <returns>VerletCollisionInfo containing information on the earliest collision (null if no collision occurred)</returns>
        private VerletCollisionInfo GetEarliestCollision(out bool collidedWithRope)
        {
            VerletCollisionInfo collisionInfo = null;
            collidedWithRope = false;

            //Foreach physics body in the physics manager
            foreach (PhysicsBody pb in _myGame.physicsManager.physicsBodies)
            {
                VerletCollisionInfo newCollision = null;

                if (pb.isRope)
                {
                    foreach(Connection c in pb.connections)
                    {
                        VerletCollisionInfo normalCollision = null;
                        VerletCollisionInfo reverseCollision = null;

                        VerletCollisionInfo nextCollision = null;

                        normalCollision = CheckLineCollision(c, false);
                        reverseCollision = CheckLineCollision(c, true);

                        if(normalCollision != null && reverseCollision != null)
                        {
                            if (normalCollision.timeOfImpact < reverseCollision.timeOfImpact)
                            {
                                nextCollision = normalCollision;
                            }
                            else
                            {
                                nextCollision = reverseCollision;
                            }
                        }
                        else if(normalCollision != null && reverseCollision == null)
                        {
                            nextCollision = normalCollision;
                        }
                        else if(normalCollision == null && reverseCollision != null)
                        {
                            nextCollision = reverseCollision;
                        }

                        if(nextCollision != null)
                        {
                            if(newCollision == null || nextCollision.timeOfImpact < newCollision.timeOfImpact)
                            {
                                newCollision = nextCollision;
                            }
                        }
                    }
                }
                else if(!pb.isPlayer)
                {
                    newCollision = CheckPhysicsBodyCollision(pb);
                }

                if (newCollision != null)
                {
                    if (collisionInfo == null || newCollision.timeOfImpact < collisionInfo.timeOfImpact)
                    {
                        collisionInfo = newCollision;
                        collidedWithRope = pb.isRope;
                    }
                }
            }

            return collisionInfo;
        }

        /// <summary>
        /// Check for collision against a Verlet connectiong
        /// </summary>
        /// <param name="connection">The connection to check collisions against</param>
        /// <param name="checkReverse">Whether the opposite side of the line should be checked for collisions</param>
        /// <returns>VerletCollisionInfo containing info on the collision (Null if no collision occurred)</returns>
        private VerletCollisionInfo CheckPhysicsBodyCollision(PhysicsBody pb)
        {
            float minLength = 1000.0f;
            VerletCollisionInfo colInfo = new VerletCollisionInfo();

            for (int i = 0; i < pb.connections.Count; i++)
            {
                Connection c = pb.connections[i];

                Vec2 axis = new Vec2(c.point1.y - c.point2.y, c.point2.x - c.point1.x);
                axis.Normalize();

                float minA, maxA, minB, maxB;
                this.ProjectToAxis(axis, out minA, out maxA);
                pb.ProjectToAxis(axis, out minB, out maxB);

                float distance = PhysicsManager.IntervalDistance(minA, maxA, minB, maxB);

                if (distance > 0.0f)
                {
                    return null;
                }
                else if (Mathf.Abs(distance) < minLength)
                {
                    minLength = Mathf.Abs(distance);
                    colInfo.normal = axis;
                    colInfo.c = c;
                }
            }

            colInfo.depth = minLength;

            int sing = Mathf.Sign(colInfo.normal.Dot(center - pb.center));

            if (sing != 1)
            {
                colInfo.normal = -colInfo.normal;
            }

            colInfo.p = points[0];
            return colInfo;
        }

        private VerletCollisionInfo CheckLineCollision(Connection connection, bool checkReverse)
        {
            Vec2 line;
            Vec2 lineStart;

            //Set the line and lineStart based on which side of the line should be checked for collisions
            if (!checkReverse)
            {
                line = connection.point2.position - connection.point1.position;
                lineStart = connection.point2.position;
            }
            else
            {
                line = connection.point1.position - connection.point2.position;
                lineStart = connection.point1.position;
            }

            Vec2 lineNormal = line.Normal();

            Vec2 lineStartToArrow = lineStart - _oldPosition;

            float a = lineNormal.Dot(lineStartToArrow);
            float b = lineNormal.Dot(_velocity);

            if (b > 0)
            {
                float t;

                if (a > 0) t = a / b;
                else if (a == 0) t = 0;
                else return null;

                Vec2 lineStartToPOI = lineStart - (_oldPosition + (_velocity * t));
                float distanceAlongLine = lineStartToPOI.Dot(line.Normalized());

                if (distanceAlongLine <= line.Length() && distanceAlongLine > 0)
                {
                    if (t <= 1 && t >= 0)
                    {
                        return new VerletCollisionInfo(-lineNormal, connection, points[0], t, _velocity.Length() * (1 - t));
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Resolve the collision based on the given collision info
        /// </summary>
        /// <param name="collisionInfo">The info related to the collision</param>
        private void ResolveCollision(VerletCollisionInfo collisionInfo)
        {
            //If the PhysicsBody we collided with is a rope
            if (collisionInfo.c.physicsParent is Rope rope)
            {
                rope.Break(collisionInfo.c);
            }
        }
    }
}
