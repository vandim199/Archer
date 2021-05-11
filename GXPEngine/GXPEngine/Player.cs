using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    public class Player : PhysicsBody
    {
        MyGame myGame;
        int moveSpeed = 1;
        private int _maxMovespeed = 10;
        int jumpSpeed = 20;
        private float aimSensitivity = 10;

        private Vec2 _startAimPosition;
        private bool _isAiming;
        private int _maxShootSpeed = 15;
        private List<Sprite> aimDots;

        public bool grounded = false;

        private AnimationSprite _graphics;
        private int spriteWidth = 75;
        private int spriteHeight = 150;

        public Vec2 currentCheckpoint = new Vec2(-1100, 200);
        //-1100, 200
        //Gets and sets the center of the player
        public Vec2 position
        {
            get
            {
                return center;
            }

            set
            {
                Vec2 offset = (value - center);

                foreach(Point point in points)
                {
                    point.position += offset;
                    point.oldPosition = point.position;
                }
            }
        }

        public Player(Vec2 spawnPosition):base(newMass:0.1f, isPlayer: true, bounceArrow:false)
        {
            myGame = ((MyGame)game);
            spawnPosition = currentCheckpoint;
            _graphics = new AnimationSprite("complete_sprisheet_kasaX.png", 4, 8);
            _graphics.SetOrigin(0, 0);
            CreatePhysicsBody(spawnPosition);
            AddChild(_graphics);
            friction = 2;
            aimDots = new List<Sprite>();
        }

        public void Step()
        {
            _graphics.Animate();
            Movement();

            if (Input.GetKeyDown(Key.R))
            {
                position = currentCheckpoint;
            }

            if (Input.GetMouseButtonDown(0))
            {
                StartAiming();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Shoot();
            }

            if (_isAiming)
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
            foreach (Point point in points)
            {
                Vec2 currentPosition = point.position;
                currentPosition.RotateAroundDegrees(-physicsAngle, center);
                point.position = currentPosition;
            }

            Vec2 movement = new Vec2(0, 0);

            if (Input.GetKey(Key.D))
            {
                movement += new Vec2(0.5f, 0);
            }
            if (Input.GetKey(Key.A))
            {
                movement += new Vec2(-0.5f, 0);
            }

            if (movement.x < -0.05f)
            {
                _graphics.Mirror(true, false);
            }
            else if (movement.x > 0.05f)
            {
                _graphics.Mirror(false, false);
            }

            SetAnimation(movement);

            movement = movement.Normalized() * moveSpeed;

            if(Mathf.Abs((points[0].position - points[0].oldPosition).Length()) > _maxMovespeed)
            {
                movement.x = 0;
            }
            else if(movement.x == 0 && grounded)
            {
                movement.x = (points[0].oldPosition.x - points[0].position.x) * 0.1f;
            }

            if ((Input.GetKeyDown(Key.W) || Input.GetKeyDown(Key.SPACE)) && grounded)
            {
                movement += new Vec2(0, -jumpSpeed);
            }

            foreach (Point point in points)
            {
                point.position += movement;
            }
        }

        private void SetAnimation(Vec2 movement)
        {
            if (_isAiming)
            {
                _graphics.SetCycle(0, 4, 5);
                if (_graphics.currentFrame == 3)
                {
                    _graphics.SetCycle(3, 1);
                }
            }
            else if(Mathf.Abs(movement.x) < 0.05f)
            {
                _graphics.SetCycle(4, 1, 3);
            }
            else
            {
                _graphics.SetCycle(5, 26, 3);
            }
        }

        private void StartAiming()
        {
            _startAimPosition = new Vec2(Input.mouseX, Input.mouseY);
            _isAiming = true;
        }

        private void Aim()
        {
            ClearAimDots();

            int dotDistance = 3;
            Vec2 mousePosition = new Vec2(Input.mouseX, Input.mouseY);

            Vec2 relativeMousePosition = _startAimPosition - mousePosition;

            Vec2 dotPosition = center + relativeMousePosition.Normalized() * 100;

            Vec2 velocity = relativeMousePosition / aimSensitivity;
            if(velocity.Length() > _maxShootSpeed)
            {
                velocity = velocity.Normalized() * _maxShootSpeed;
            }

            velocity *= 1.5f;

            if(velocity.x < -0.05f)
            {
                _graphics.Mirror(true, false);
            }
            else if(velocity.x > 0.05f)
            {
                _graphics.Mirror(false, false);
            }

            for (int i = 0; i < 10; i++)
            {
                Sprite newDot = new Sprite("Dot.png", false, false);
                newDot.SetOrigin(newDot.width / 2f, newDot.height / 2f);
                newDot.width = 5;
                newDot.height = 5;

                for (int j = 0; j < dotDistance; j++)
                {
                    velocity += myGame.gravity * 1.5f;
                    dotPosition += velocity;
                }

                newDot.SetXY(dotPosition.x, dotPosition.y);
                aimDots.Add(newDot);
                AddChild(newDot);
            }
        }

        private void Shoot()
        {
            ClearAimDots();

            _isAiming = false;
            Vec2 mousePosition = new Vec2(Input.mouseX, Input.mouseY);

            Vec2 relativeMousePosition = _startAimPosition - mousePosition;

            Vec2 spawnPosition = center + (relativeMousePosition.Normalized() * 100);

            myGame.soundArrow.Play();

            Vec2 startVelocity = relativeMousePosition / aimSensitivity;

            if(startVelocity.Length() > _maxShootSpeed)
            {
                startVelocity = startVelocity.Normalized() * _maxShootSpeed;
            }

            myGame.AddChild(new Projectile(myGame, startVelocity * 1.5f, spawnPosition));
        }

        private void ClearAimDots()
        {
            foreach (Sprite sprite in aimDots)
            {
                sprite.LateDestroy();
            }

            aimDots.Clear();
        }
    }
}
