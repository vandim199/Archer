using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    class Player : AnimationSprite
    {
        MyGame myGame;
        Vec2 position;
        Vec2 velocity;
        int speed = 3;
        int combinedInputs;

        public Player():base("character_maleAdventurer_sheetHD.png", 9, 5)
        {
            myGame = ((MyGame)game);
            scale = 0.5f;

            myGame.lines.Add(new LineSegment(this, 0, 0, width, height));
        }

        void Update()
        {
            Animate();
            Movement();
            UpdateScreenPosition();
        }

        void Movement()
        {
            combinedInputs = (Input.GetKey(Key.A) ? 1 : 0) + (Input.GetKey(Key.D) ? 1 : 0) + (Input.GetKey(Key.W) ? 1 : 0) + (Input.GetKey(Key.S) ? 1 : 0);

            if (Input.GetKey(Key.A)) velocity.x = -speed;
            else if (Input.GetKey(Key.D)) velocity.x = speed;
            else velocity.x *= 0.95f;

            if (Input.GetKey(Key.W)) velocity.y = -speed;
            else if (Input.GetKey(Key.S)) velocity.y = speed;
            else velocity.y *= 0.95f;

            if (combinedInputs > 1) velocity = velocity.Normalized() * speed;
            if (combinedInputs == 0) SetCycle(0, 1);
            else SetCycle(24, 4);

            if (velocity.x < 0) Mirror(true, false);
            else Mirror(false, false);

            //velocity += myGame.gravity;
            position += velocity;
        }

        void UpdateScreenPosition()
        {
            x = position.x;
            y = position.y;
        }
    }
}
