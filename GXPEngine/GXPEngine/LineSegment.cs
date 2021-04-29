using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    public class LineSegment : GameObject
    {
        public Vec2 startPoint;
        public Vec2 endPoint;
        bool global;
        bool arrow;
        uint color;
        byte lineWeight;
        public bool floor;
        bool moving;

        public GameObject realParent;
        GameObject space;
        Ball lineCap;
        Vec2 offset1, offset2;
        
        public LineSegment(GameObject newRealParent, float startX, float startY, float endX, float endY, uint newColor = 0xffffffff, byte newLineWeight = 1,
            bool newGlobal = false, bool newArrow = false, bool side = true, bool newFloor = false, bool newMoving = false):base()
        {
            MyGame myGame = ((MyGame)game);

            realParent = newRealParent;
            global = newGlobal;
            arrow = newArrow;
            color = newColor;
            lineWeight = newLineWeight;
            floor = newFloor;
            moving = newMoving;

            startPoint.SetXY(startX, startY);
            endPoint.SetXY(endX, endY);

            if (!global) space = realParent;
            else space = myGame;

            if (!moving)
            {
                lineCap = new Ball(realParent, startPoint, 0);
                if (realParent != myGame) realParent.AddChild(lineCap);
                myGame.balls.Add(lineCap);

                lineCap = new Ball(realParent, endPoint, 0);
                if (realParent != myGame) realParent.AddChild(lineCap);
                myGame.balls.Add(lineCap);
            }

            if (realParent is Box box)
            {
                offset1 = startPoint;
                offset2 = endPoint;

                //offset1.RotateAroundDegrees(box.rotation, box.position);
                //offset2.RotateAroundDegrees(box.rotation, box.position);
            }

            if (!side) myGame.lines.Add(new LineSegment(newRealParent ,endPoint.x, endPoint.y, startPoint.x, startPoint.y, color, 0, global, false, true));
        }

        protected override void RenderSelf(GXPEngine.Core.GLContext glContext)
        {
            //Gizmos.DrawLine(startPoint.x, startPoint.y, endPoint.x, endPoint.y, space, color, lineWeight);
            Gizmos.RenderLine(startPoint.x, startPoint.y, endPoint.x, endPoint.y, color, lineWeight, global);

            if (arrow)
            {
                Vec2 smallVec = (startPoint + endPoint).Normalized() * -10;
                Vec2 left = new Vec2(-smallVec.y, smallVec.x) + smallVec + endPoint;
                Vec2 right = new Vec2(smallVec.y, -smallVec.x) + smallVec + endPoint;

                Gizmos.DrawLine(endPoint.x, endPoint.y, left.x, left.y, space, color, lineWeight);
                Gizmos.DrawLine(endPoint.x, endPoint.y, right.x, right.y, space, color, lineWeight);
            }
        }

        void Update()
        {
            if (moving)
            {
                if (realParent is Box box)
                {
                    //startPoint = offset1 + box.position;
                    //endPoint = offset2 + box.position;

                    //startPoint.SetAngleAroundDegrees(box.rotation, box.max);
                    //endPoint.SetAngleAroundDegrees(box.rotation, box.min);
                    offset1.SetAngleDegrees(box.rotation);
                    offset2.SetAngleDegrees(box.rotation);

                    //startPoint.SetAngleDegrees(box.rotation);
                }
            }
        }
    }
}
