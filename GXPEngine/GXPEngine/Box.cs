using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    public class Box : Sprite
    {
        Vec2 min, max;

        public Box():base("barry.png")
        {
            min = new Vec2(0, 0);
            max = new Vec2(width, height);
        }
    }
}
