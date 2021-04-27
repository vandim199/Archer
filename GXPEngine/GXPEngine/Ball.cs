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

        float bounciness;

        CollisionInfo currentCollision;
        CollisionInfo previousCollision;

        public float Mass
        {
            get
            {
                return 4 * radius * radius * Mathf.PI;
            }
        }
        

        public Ball(GameObject newRealParent, Vec2 newPos, float newRadius = 10, float newBounciness = 1, bool newMoving = false):base("Ball.png")
        {
            realParent = newRealParent;
            SetOrigin(width/2, height/2);
            moving = newMoving;
            radius = newRadius;
            position = newPos;
            bounciness = newBounciness;
            previousCollision = new CollisionInfo(new Vec2(), this, 2);

            if (radius == 0) visible = false;
            myGame = ((MyGame)game);

            scale = radius / 10;
        }

        void Update()
        {
            if (moving)
            {
                if (previousCollision != null && currentCollision != null)
                {
                    Console.WriteLine(previousCollision.timeOfImpact - currentCollision.timeOfImpact);
                }

                previousCollision = currentCollision;
                _oldPosition = position;
                position += velocity;
                
                currentCollision = FindEarliestCollision();
                if (currentCollision != null)
                {
                    if (previousCollision == null || currentCollision.timeOfImpact < previousCollision.timeOfImpact)
                    {
                        ResolveCollision(currentCollision);
                    }
                    else ResolveCollision(previousCollision);
                }
                else (realParent as Player).grounded = false;
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
                        Vec2 ballNormal = relativePosition.Normalized();

                        float a = Mathf.Abs(Mathf.Pow(velocity.Length(), 2));
                        float b = 2 * ballNormal.Dot(velocity);
                        float c = Mathf.Abs(Mathf.Pow(ballNormal.Length(), 2)) - Mathf.Pow((radius + ball.radius), 2);

                        if (Mathf.Abs(a) < 0.001f) return null;
                        float D = b * b - 4 * a * c;
                        if (D < 0) return null;

                        float ToI = -b - Mathf.Sqrt(D) / (2 * a);

                        if (ToI >= 0)
                        {
                            Vec2 PoI = position + velocity * ToI;
                            float distance = relativePosition.Length();
                            float overlap = radius + ball.radius - distance;
                            //Vec2 PoI = relativePosition.Normalized() * overlap;
                            return new CollisionInfo(ballNormal, ball, ToI, PoI);
                        }
                    }
                }
            }
            
            for (int i = 0; i < myGame.lines.Count; i++)
            {
                //oldDifferenceVec = differenceVec;

                LineSegment line = myGame.lines[i];
                Vec2 lineSegment = (line.endPoint - line.startPoint);
                Vec2 lineNormal = lineSegment.Normal();
                oldDifferenceVec = _oldPosition - line.startPoint;
                differenceVec = position - line.endPoint;
                float ballDistance = differenceVec.Dot(lineNormal);

                //if (ballDistance < radius)
                {
                    //Vec2 PoI = (-ballDistance + radius) * lineNormal + position;

                    float a = lineNormal.Dot(oldDifferenceVec) - radius;
                    float b = -(lineNormal.Dot(velocity));

                    if (b <= 0) continue;
                    if (a < 0) continue;

                    //Vec2 a = PoI - _oldPosition;

                    float ToI;
                    if (a >= 0)
                    ToI = a / b;

                    else if (a >= -radius)
                    ToI = 0;

                    else return null;

                    if (Mathf.Abs(ToI) <= 1)
                    {
                        Vec2 PoI = _oldPosition + velocity * ToI;
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

                velocity.Reflect(col.normal, bounciness);
                position = col.pointOfImpact;

                //(realParent as Player).velocity = velocity;
                (realParent as Player).grounded = true;
            }

            if (col.other is LineSegment)
            {
                LineSegment line = (LineSegment)col.other;
                
                position = col.pointOfImpact;
                velocity.Reflect(col.normal, bounciness);

                //(realParent as Player).velocity = velocity;
                //if(col.other.y < position.y)
                (realParent as Player).grounded = true;
            }
        }
    }
}
