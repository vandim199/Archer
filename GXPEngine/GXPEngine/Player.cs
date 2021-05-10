using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    class Player : PhysicsBody
    {
        MyGame myGame;
        int moveSpeed = 1;
        int jumpSpeed = 20;
        private float aimSensitivity = 10;
        int groundFriction = 2;

        private Vec2 startAimPosition;
        private bool isAiming;

        public bool grounded = false;

        private AnimationSprite _graphics;
        private int spriteWidth = 75;
        private int spriteHeight = 150;

        public Player(Vec2 spawnPosition):base(0.1f, isPlayer: true)
        {
            myGame = ((MyGame)game);
            _graphics = new AnimationSprite("Kasa_spritesheet.png", 6, 5);
            _graphics.SetOrigin(0, 0);
            CreatePhysicsBody(spawnPosition);
            AddChild(_graphics);
            friction = 2;
        }

        public void Step()
        {
            _graphics.Animate();
            Movement();

            if (Input.GetKeyDown(Key.R))
            {
                //
            }

            if (Input.GetMouseButtonDown(0))
            {
                StartAiming();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Shoot();
            }

            if (isAiming)
            {
                Aim();
            }

            _graphics.x = points[0].position.x;
            _graphics.y = points[0].position.y;
        }

        private void CreatePhysicsBody(Vec2 spawnPosition)
        {
            AddPoint(new Vec2(spawnPosition.x - (spriteWidth / 2f), spawnPosition.y - (spriteHeight / 2f)), false);
            AddPoint(new Vec2(spawnPosition.x + (spriteWidth / 2f), spawnPosition.y - (spriteHeight / 2f)), false);
            AddPoint(new Vec2(spawnPosition.x + (spriteWidth / 2f), spawnPosition.y + (spriteHeight / 2f)), false);
            AddPoint(new Vec2(spawnPosition.x - (spriteWidth / 2f), spawnPosition.y + (spriteHeight / 2f)), false);
        }

        void Movement()
        {
            foreach(Point point in points)
            {
                Vec2 currentPosition = point.position;
                currentPosition.RotateAroundDegrees(-physicsAngle, center);
                point.position = currentPosition;
            }

            Vec2 movement = new Vec2(0, 0);

            if (Input.GetKey(Key.D))
            {
                movement += new Vec2(1, 0);
            }
            if (Input.GetKey(Key.A))
            {
                movement += new Vec2(-1, 0);
            }

            movement = movement.Normalized() * moveSpeed;

            if (Input.GetKey(Key.W) && grounded)
            {
                movement += new Vec2(0, -jumpSpeed);
                grounded = false;
            }

            foreach (Point point in points)
            {
                point.position += movement;
            }

            if(points[0].position.x - points[0].oldPosition.x < 0)
            {
                _graphics.Mirror(true, false);
            }
            else
            {
                _graphics.Mirror(false, false);
            }

            SetAnimation();
        }

        private void SetAnimation()
        {
            if(Mathf.Abs(points[0].position.x - points[0].oldPosition.x) < 0.1f)
            {
                _graphics.SetCycle(0, 1, 10);
            }
            else
            {
                _graphics.SetCycle(1, 26, 10);
            }
        }

        private void StartAiming()
        {
            startAimPosition = new Vec2(Input.mouseX, Input.mouseY);
            isAiming = true;
        }

        private void Aim()
        {
            Vec2 mousePosition = new Vec2(Input.mouseX, Input.mouseY);
            Gizmos.DrawLine(startAimPosition.x, startAimPosition.y, mousePosition.x, mousePosition.y, color: 0xffffffff, width: 5);
        }

        private void Shoot()
        {
            isAiming = false;
            Vec2 mousePosition = new Vec2(Input.mouseX, Input.mouseY);

            Vec2 relativeMousePosition = startAimPosition - mousePosition;

            Vec2 spawnPosition = center + (relativeMousePosition.Normalized() * 100);

            myGame.soundArrow.Play();

            myGame.AddChild(new Projectile(myGame, relativeMousePosition / aimSensitivity, spawnPosition));
        }
    }
}
