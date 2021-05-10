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
        private int _maxMovespeed = 10;
        int jumpSpeed = 20;
        private float aimSensitivity = 10;

        private Vec2 _startAimPosition;
        private bool _isAiming;
        private int _maxShootSpeed = 10;
        private List<Sprite> aimDots;

        public bool grounded = false;

        private AnimationSprite _graphics;
        private int spriteWidth = 75;
        private int spriteHeight = 150;

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
            _graphics = new AnimationSprite("Kasa_spritesheet.png", 6, 5);
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
                position = new Vec2(50, 50);
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

            movement = movement.Normalized() * moveSpeed;

            if(Mathf.Abs((points[0].position - points[0].oldPosition).Length()) > _maxMovespeed)
            {
                movement.x = 0;
            }
            else if(movement.x == 0 && grounded)
            {
                movement.x = (points[0].oldPosition.x - points[0].position.x) * 0.1f;
            }

            if ((Input.GetKey(Key.W) || Input.GetKey(Key.SPACE)) && grounded)
            {
                movement += new Vec2(0, -jumpSpeed);
            }

            if(movement.x == 0 && grounded)
            {
                movement.x = (points[0].oldPosition.x - points[0].position.x) * 0.2f;
            }

            foreach (Point point in points)
            {
                point.position += movement;
            }

            if (points[0].position.x - points[0].oldPosition.x < -0.05f)
            {
                _graphics.Mirror(true, false);
            }
            else if (points[0].position.x - points[0].oldPosition.x > 0.05f)
            {
                _graphics.Mirror(false, false);
            }

            SetAnimation();
        }

        private void SetAnimation()
        {
            if(Mathf.Abs(points[0].position.x - points[0].oldPosition.x) < 0.05f)
            {
                _graphics.SetCycle(0, 1, 3);
            }
            else
            {
                _graphics.SetCycle(1, 26, 3);
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

            float numberOfDots = relativeMousePosition.Length() / dotDistance;
            numberOfDots = Mathf.Round(numberOfDots);
            float trueDotDistance = (relativeMousePosition.Length() / aimSensitivity) / numberOfDots;

            Vec2 dotPosition = center + relativeMousePosition.Normalized() * 100;

            Vec2 velocity = relativeMousePosition / aimSensitivity;
            if(velocity.Length() > _maxShootSpeed)
            {
                velocity = velocity.Normalized() * _maxShootSpeed;
            }

            for (int i = 0; i < numberOfDots; i++)
            {
                Sprite newDot = new Sprite("Dot.png", false, false);
                newDot.SetOrigin(newDot.width / 2f, newDot.height / 2f);
                newDot.width = 5;
                newDot.height = 5;

                for (int j = 0; j < dotDistance; j++)
                {
                    velocity += myGame.gravity;
                    dotPosition += velocity;
                }

                newDot.SetXY(dotPosition.x, dotPosition.y);
                aimDots.Add(newDot);
                AddChild(newDot);
            }

            //Gizmos.DrawLine(_startAimPosition.x, _startAimPosition.y, mousePosition.x, mousePosition.y, color: 0xffffffff, width: 5);
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

            myGame.AddChild(new Projectile(myGame, startVelocity, spawnPosition));
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
