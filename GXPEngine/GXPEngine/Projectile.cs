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
        private Point _collider; //The collider (Tip of the arrowhead)

        private Vec2 _oldPosition; //The old position
        private Vec2 _position
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
        } //The current position
        private int _radius = 2;

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
            AddChild(_arrowGraphics);

            _collider = new Point(_myGame, new Vec2(0, 0), this);
            AddChild(_collider);
        }

        void Update()
        {
            Move();
            VerletCollisionInfo collisionInfo = GetEarliestCollision();

            if (collisionInfo != null)
            {
                ResolveCollision(collisionInfo);
            }
        }

        /// <summary>
        /// Move the arrow
        /// </summary>
        private void Move()
        {
            _velocity += _myGame.gravity;
            _oldPosition = _position;
            _position += _velocity;
            _arrowGraphics.rotation = _velocity.GetAngleDegrees();
        }

        /// <summary>
        /// Get the earliest collision of the arrow.
        /// </summary>
        /// <returns>VerletCollisionInfo containing information on the earliest collision (null if no collision occurred)</returns>
        private VerletCollisionInfo GetEarliestCollision()
        {
            VerletCollisionInfo collisionInfo = null;

            //Foreach physics body in the physics manager
            foreach (PhysicsBody pb in _myGame.physicsManager.physicsBodies)
            {
                //Check against every connection of the collider
                foreach (Connection connection in pb.connections)
                {
                    VerletCollisionInfo newCollision = null;

                    VerletCollisionInfo normalCollision = null;
                    VerletCollisionInfo reverseCollision = null;

                    normalCollision = CheckCollision(connection, false);
                    reverseCollision = CheckCollision(connection, true);

                    if (normalCollision != null && reverseCollision != null)
                    {
                        if (normalCollision.timeOfImpact < reverseCollision.timeOfImpact)
                        {
                            newCollision = normalCollision;
                        }
                        else
                        {
                            newCollision = reverseCollision;
                        }
                    }
                    else if (normalCollision != null && reverseCollision == null)
                    {
                        newCollision = normalCollision;
                    }
                    else if (normalCollision == null && reverseCollision != null)
                    {
                        newCollision = reverseCollision;
                    }

                    if (newCollision != null)
                    {
                        if (collisionInfo == null || newCollision.timeOfImpact < collisionInfo.timeOfImpact)
                        {
                            collisionInfo = newCollision;
                        }
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
        private VerletCollisionInfo CheckCollision(Connection connection, bool checkReverse)
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
                        return new VerletCollisionInfo(lineNormal, connection, _collider, t);
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
            if (collisionInfo.c.physicsParent.isRope)
            {
                //Cut the connection segment of the rope that we collided with
                collisionInfo.c.physicsParent.RemoveConnection(collisionInfo.c);
            }
            else
            {
                LateDestroy();
            }
        }
    }
}
