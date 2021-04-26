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
        public Vec2 velocity;
        int speed = 6;
        int combinedInputs;

        Ball newBall;

        public bool grounded = false;

        public Player():base("character_maleAdventurer_sheetHD.png", 9, 5)
        {
            myGame = ((MyGame)game);
            SetOrigin(width / 2, height / 2);
            scale = 0.5f;
            position.SetXY(40, 40);

            //myGame.lines.Add(new LineSegment(this, 0, height * 2, width * 2, height * 2));
            //myGame.lines.Add(new LineSegment(this, width * 2, height * 2, width * 2, 0));
            //myGame.lines.Add(new LineSegment(this, width*2, 0, 0, 0));
            //myGame.lines.Add(new LineSegment(this, 0, 0, 0, height * 2));

            newBall = new Ball(this, position + new Vec2(0, 15), 50, 0, true);
            newBall.visible = false;
            //myGame.AddChild(newBall);
            myGame.balls.Add(newBall);
        }

        void Update()
        {
            if (Input.GetKeyDown(Key.R)) newBall.position.SetXY(40, 40);

            position = newBall.position - new Vec2(0, 15);
            newBall.velocity = velocity;
            Animate();
            Movement();
            UpdateScreenPosition();
        }

        void Movement()
        {
            combinedInputs = (Input.GetKey(Key.A) ? 1 : 0) + (Input.GetKey(Key.D) ? 1 : 0) + (Input.GetKey(Key.W) ? 1 : 0);

            if (Input.GetKey(Key.A)) velocity.x = -speed;
            else if (Input.GetKey(Key.D)) velocity.x = speed;
            else velocity.x *= 0.95f;

            if (Input.GetKey(Key.W) && grounded) velocity.y = -20;
            else velocity.y *= 0.95f;

            //if (combinedInputs > 1) velocity = velocity.Normalized() * speed;
            if (combinedInputs == 0) SetCycle(0, 1);
            else SetCycle(24, 4);

            if (velocity.x < 0) Mirror(true, false);
            else Mirror(false, false);

            if(!grounded)velocity += myGame.gravity;
            position += velocity;

            Console.WriteLine(velocity.y);
            Mathf.Clamp(velocity.y, -1000, myGame.gravity.y);
        }

        void UpdateScreenPosition()
        {
            x = position.x;
            y = position.y;
        }
    }
}
