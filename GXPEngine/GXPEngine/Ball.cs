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

        public bool firstTime = true;

        public float Mass
        {
            get
            {
                return 4 * radius * radius * Mathf.PI;
            }
        }
        

        public Ball(GameObject newRealParent, Vec2 newPos, float newRadius = 10, float newBounciness = 1, bool newMoving = false):base("images/Ball.png")
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
                previousCollision = currentCollision;
                _oldPosition = position;
                position += velocity;
                
                currentCollision = FindEarliestCollision();
                if (currentCollision != null)
                {
                    //if (previousCollision == null || currentCollision.timeOfImpact < previousCollision.timeOfImpact)
                    {
                        ResolveCollision(currentCollision);
                    }
                    //else ResolveCollision(previousCollision);
                }
                else if (realParent is Player player)
                {
                    player.grounded = false;
                }
            }
            x = position.x;
            y = position.y;
        }

        CollisionInfo FindEarliestCollision()
        {
            //for (int i = 0; i < myGame.balls.Count; i++)
            foreach(Ball other in myGame.balls)
            {
                //Ball ball = myGame.balls[i];
                if (other != this)
                {
                    Vec2 relativePosition = position - other.position;
                    if (relativePosition.Length() < radius + other.radius)
                    {
                        Vec2 ballNormal = relativePosition.Normalized();

                        Vec2 u = position - other.position;

                        float a = velocity.Length() * velocity.Length();
                        float b = (2 * u).Dot(velocity);
                        float c = u.Length() * u.Length() - (radius + other.radius) * (radius + other.radius);

                        if (Mathf.Abs(a) < 0.001f) return null;
                        float D = (b * b) - (4 * a * c);
                        if (D < 0) return null;

                        float ToI = (-b - Mathf.Sqrt(D)) / (2 * a);

                        if (Mathf.Abs(ToI) >= 0 && Mathf.Abs(ToI) < 1)
                        {
                            Vec2 PoI = _oldPosition + (velocity * ToI);
                            float distance = relativePosition.Length();
                            float overlap = radius + other.radius - distance;
                            //Vec2 PoI = relativePosition.Normalized() * overlap;
                            return new CollisionInfo(relativePosition.Normalized(), other, ToI, PoI);
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
                differenceVec = position - line.startPoint;
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
                    
                    else continue;

                    if (Mathf.Abs(ToI) <= 1)
                    {
                        Vec2 PoI = _oldPosition + (velocity * ToI);
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
                if (realParent is Player player)
                {
                    player.grounded = true;
                }
            }

            if (col.other is LineSegment)
            {
                LineSegment line = (LineSegment)col.other;

                velocity.Reflect(col.normal, bounciness);
                position = col.pointOfImpact;

                //if (Mathf.Abs(col.timeOfImpact) <= 0.001 && line.floor)
                {
                    position += velocity;
                }

                //(realParent as Player).velocity = velocity;
                if (realParent is Player player && line.floor)
                {
                    player.grounded = true;
                }
            }
        }
    }
}
