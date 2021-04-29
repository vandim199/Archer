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

        public Box():base("barry.png")
        {
            myGame = ((MyGame)game);
            myGame.AddChild(this);

            //rotation = 70;
            position = new Vec2(300, 150);

            min = new Vec2(0, 0);
            max = new Vec2(width, height);

            myGame.lines.Add(new LineSegment(this, min.x, min.y, min.x, max.y, newGlobal:true));
            myGame.lines.Add(new LineSegment(this, min.x, max.y, max.x, max.y, newGlobal: true));
            myGame.lines.Add(new LineSegment(this, max.x, max.y, max.x, min.y, newGlobal: true));
            myGame.lines.Add(new LineSegment(this, max.x, min.y, min.x, min.y, newGlobal: true));
        }

        void Update()
        {
            //position.x += 0.1f;
            rotation += 0.1f;

            x = position.x;
            y = position.y;
        }
    }
}
