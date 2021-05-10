using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    public class Checkpoint : AnimationSprite
    {
        public Checkpoint(Vec2 pos) : base("triangle.png", 1, 1)
        {
            SetXY(pos.x, pos.y);
        }

        void Update()
        {
            Player player = ((MyGame)game).player;

            if (player.position.x > this.x)
            {
                if(player.currentCheckpoint.x < this.x)
                player.currentCheckpoint = new Vec2(x + width/2, y - 50);
            }
        }
    }
}
