using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    public class Ball : Sprite
    {
        public Vec2 position;
        Vec2 _oldPosition;
        public Vec2 velocity;

        public float speed = 5;
        public float radius;

        Vec2 differenceVec = new Vec2(0, 0);
        Vec2 oldDifferenceVec;

        bool moving;
        GameObject realParent;
        MyGame myGame;

        public float Mass
        {
            get
            {
                return 4 * radius * radius * Mathf.PI;
            }
        }
        

        public Ball(GameObject newRealParent, Vec2 newPos, float newRadius = 13, bool newMoving = false):base("Ball.png")
        {
            realParent = newRealParent;
            SetOrigin(width/2, height/2);
            moving = newMoving;
            radius = newRadius;
            position = newPos;

            if (radius == 0) visible = false;
            myGame = ((MyGame)game);
        }

        void Update()
        {
            if (moving)
            {
                _oldPosition = position;
                position += velocity;

                CollisionInfo firstCollision = FindEarliestCollision();
                if (firstCollision != null)
                {
                    ResolveCollision(firstCollision);
                }
            }
            x = position.x;
            y = position.y;
        }

        CollisionInfo FindEarliestCollision()
        {
            //for (int i = 0; i < myGame.balls.Count; i++)
            foreach(Ball ball in myGame.balls)
            {
                //Ball ball = myGame.balls[i];
                if (ball != this)
                {
                    Vec2 relativePosition = position - ball.position;
                    if (relativePosition.Length() < radius + ball.radius)
                    {
                        float distance = relativePosition.Length();
                        float overlap = radius + ball.radius - distance;
                        Vec2 PoI = relativePosition.Normalized() * overlap;
                        return new CollisionInfo(relativePosition.Normalized(), ball, 0, PoI);
                    }
                }
            }
            
            for (int i = 0; i < myGame.lines.Count; i++)
            {
                oldDifferenceVec = differenceVec;

                LineSegment line = myGame.lines[i];
                Vec2 lineSegment = (line.endPoint - line.startPoint);
                Vec2 lineNormal = lineSegment.Normal();
                differenceVec = position - line.endPoint;
                float ballDistance = differenceVec.Dot(lineNormal);

                if (ballDistance < radius)
                {
                    Vec2 PoI = (-ballDistance + radius) * lineNormal + position;

                    Vec2 a = PoI - _oldPosition;

                    float ToI = a.Length() / velocity.Length();

                    if (Mathf.Abs(ToI) <= 1)
                    {
                        float d = (line.endPoint - PoI).Dot(lineSegment.Normalized());

                        if (d >= 0 && d <= lineSegment.Length())
                        {
                            return new CollisionInfo(lineNormal, line, ToI, PoI);
                        }
                    }
                }
            }

            return null;
        }

        void ResolveCollision(CollisionInfo col)
        {
            if (col.other is Ball)
            {
                Ball otherBall = (Ball)col.other;

                velocity.Reflect(col.normal);
                position += col.pointOfImpact;
            }

            if (col.other is LineSegment)
            {
                LineSegment line = (LineSegment)col.other;
                
                position = col.pointOfImpact;
                velocity.Reflect(col.normal);
            }
        }
    }
}
