using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    class Projectile : Sprite
    {
        private Ball _collider;
        private MyGame _myGame;
        private Vec2 _velocity;

        private Vec2 _position
        {
            get
            {
                return new Vec2(x, y);
            }
            set
            {
                x = value.x;
                y = value.y;
            }
        }

        public Projectile(MyGame myGame, Vec2 startingVelocity, Vec2 startingPosition) : base("ball.png", false)
        {
            _position = startingPosition;

            _myGame = myGame;

            _collider = new Ball(this, startingPosition, width / 2f, 0.5f, true);
            _collider.velocity = startingVelocity;

            _myGame.AddChild(_collider);
            _velocity = startingVelocity;

            SetOrigin(width / 2f, height / 2f);
        }

        void Update()
        {
            Move();
        }

        private void Move()
        {
            _collider.velocity += _myGame.gravity;
            _velocity = _collider.velocity;
            _position = _collider.position;
        }
    }
}
