using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    class Player : AnimationSprite
    {
        MyGame myGame;
        public Vec2 position;
        //public Vec2 velocity;
        int speed = 7;
        int combinedInputs;

        Ball newBall;

        public bool grounded = false;

        public Player():base("character_maleAdventurer_sheetHD.png", 9, 5)
        {
            myGame = ((MyGame)game);
            SetOrigin(width / 2, height / 2);
            scale = 0.5f;
            position.SetXY(60, 60);

            //myGame.lines.Add(new LineSegment(this, 0, height * 2, width * 2, height * 2));
            //myGame.lines.Add(new LineSegment(this, width * 2, height * 2, width * 2, 0));
            //myGame.lines.Add(new LineSegment(this, width*2, 0, 0, 0));
            //myGame.lines.Add(new LineSegment(this, 0, 0, 0, height * 2));

            newBall = new Ball(this, position + new Vec2(0, 15), 50, 0.1f, true);
            newBall.visible = false;
            //myGame.AddChild(newBall);
            myGame.balls.Add(newBall);
        }

        void Update()
        {
            if (Input.GetKeyDown(Key.R) || position.y > game.height) newBall.position.SetXY(60, 60);
            position = newBall.position - new Vec2(0, 15);
            //velocity = newBall.velocity;
            Animate();
            Movement();
            UpdateScreenPosition();
        }

        void Movement()
        {
            combinedInputs = (Input.GetKey(Key.A) ? 1 : 0) + (Input.GetKey(Key.D) ? 1 : 0) + (Input.GetKey(Key.W) ? 1 : 0);

            if (Input.GetKey(Key.A)) newBall.velocity.x = -speed;
            else if (Input.GetKey(Key.D)) newBall.velocity.x = speed;
            else newBall.velocity.x *= 0.95f;

            if (Input.GetKeyDown(Key.W)) newBall.velocity.y = -20;
            else if (Input.GetKey(Key.S)) newBall.velocity.y = speed;
            else newBall.velocity.y *= 0.95f;

            //if (combinedInputs > 1) velocity = velocity.Normalized() * speed;
            if (combinedInputs == 0) SetCycle(0, 1);
            else SetCycle(24, 4);

            if (newBall.velocity.x < 0) Mirror(true, false);
            else Mirror(false, false);

            //if(!grounded)
            newBall.velocity += myGame.gravity;
            //position += velocity;
        }

        void UpdateScreenPosition()
        {
            x = position.x;
            y = position.y;
        }
    }
}
