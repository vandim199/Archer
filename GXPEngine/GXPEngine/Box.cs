using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    public class Box : Sprite
    {
        public Vec2 position;
        public Vec2 min, max;
        MyGame myGame;

        LineSegment[] colliders = new LineSegment[4];

        public Box():base("barry.png")
        {
            myGame = ((MyGame)game);
            myGame.AddChild(this);

            //rotation = 70;
            position = new Vec2(300, 150);

            min = new Vec2(0, 0);
            max = new Vec2(width, height);

            colliders[0] = new LineSegment(this, min.x, min.y, min.x, max.y, newGlobal: true, newMoving:true, newFloor: true);
            colliders[1] = new LineSegment(this, min.x, max.y, max.x, max.y, newGlobal: true, newMoving: true, newFloor: true);
            colliders[2] = new LineSegment(this, max.x, max.y, max.x, min.y, newGlobal: true, newMoving: true, newFloor: true);
            colliders[3] = new LineSegment(this, max.x, min.y, min.x, min.y, newGlobal: true, newMoving: true, newFloor:true);

            for (int i = 0; i < colliders.Length; i++)
            {
                myGame.lines.Add(colliders[i]);
            }

            /*myGame.lines.Add(new LineSegment(this, min.x, min.y, min.x, max.y, newGlobal:true));
            myGame.lines.Add(new LineSegment(this, min.x, max.y, max.x, max.y, newGlobal: true));
            myGame.lines.Add(new LineSegment(this, max.x, max.y, max.x, min.y, newGlobal: true));
            myGame.lines.Add(new LineSegment(this, max.x, min.y, min.x, min.y, newGlobal: true));*/
        }

        void Update()
        {
            //position.x += 0.1f;
            rotation += 0.1f;

            /*min = position;
            max = position + new Vec2(width, height);
            max.RotateAroundDegrees(rotation, position);

            colliders[0].startPoint.SetXY(min.x, min.y);
            colliders[0].endPoint.SetXY(min.x, max.y);
            colliders[1].startPoint.SetXY(min.x, max.y);
            colliders[1].endPoint.SetXY(max.x, max.y);
            colliders[2].startPoint.SetXY(max.x, max.y);
            colliders[2].endPoint.SetXY(max.x, min.y);
            colliders[3].startPoint.SetXY(max.x, min.y);
            colliders[3].endPoint.SetXY(min.x, min.y);*/

            x = position.x;
            y = position.y;
        }
    }
}
