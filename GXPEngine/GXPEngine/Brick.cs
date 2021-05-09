﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    class Brick : PhysicsBody
    {
        private Sprite _graphics;
        /// <summary>
        /// Creates a Verlet physics brick with 4 corners based on the give parameters. (Corners are created from the top left, clockwise to the bottom left)
        /// </summary>
        /// <param name="spawnPosition">The center of the brick</param>
        /// <param name="newWidth">The width of the brick</param>
        /// <param name="newHeight">The height of the brick</param>
        /// <param name="startRotation">The rotation of the brick</param>
        /// <param name="isSolid">Whether the brick should move or not</param>
        /// <param name="mass">The mass of the brick</param>
        public Brick(Vec2 spawnPosition, int newWidth, int newHeight, string sprite, int startRotation = 0, bool isSolid = false, int mass = 1, bool _isFloor = false) : base(mass, isFloor: _isFloor)
        {
            CreateBody(spawnPosition, newWidth, newHeight, startRotation, isSolid);

            _graphics = new Sprite(sprite, addCollider: false);
            _graphics.SetOrigin(0, 0);
            _graphics.width = newWidth;
            _graphics.height = newHeight;
            AddChild(_graphics);
        }

        void Update()
        {
            _graphics.x = points[0].position.x;
            _graphics.y = points[0].position.y;

            _graphics.rotation = (points[1].position - points[0].position).GetAngleDegrees();
        }

        private void CreateBody(Vec2 spawnPosition, int newWidth, int newHeight, int startRotation, bool isSolid)
        {
            Vec2 topLeft = new Vec2(spawnPosition.x - newWidth / 2f, spawnPosition.y - newHeight / 2f);
            Vec2 topRight = new Vec2(spawnPosition.x + newWidth / 2f, spawnPosition.y - newHeight / 2f);
            Vec2 bottomRight = new Vec2(spawnPosition.x + newWidth / 2f, spawnPosition.y + newHeight / 2f);
            Vec2 bottomLeft = new Vec2(spawnPosition.x - newWidth / 2f, spawnPosition.y + newHeight / 2f);

            topLeft.RotateAroundDegrees(startRotation, spawnPosition);
            topRight.RotateAroundDegrees(startRotation, spawnPosition);
            bottomRight.RotateAroundDegrees(startRotation, spawnPosition);
            bottomLeft.RotateAroundDegrees(startRotation, spawnPosition);

            AddPoint(topLeft, isSolid);
            AddPoint(topRight, isSolid);
            AddPoint(bottomRight, isSolid);
            AddPoint(bottomLeft, isSolid);
        }
    }
}
