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
        uint lineWeight;

        public GameObject realParent;
        Ball lineCap;

        public LineSegment(GameObject newRealParent, float startX, float startY, float endX, float endY, uint newColor = 0xfffffff, uint newLineWeight = 1, bool newGlobal = false, bool newArrow = false, bool side = true):base()
        {
            MyGame myGame = ((MyGame)game);

            realParent = newRealParent;
            global = newGlobal;
            arrow = newArrow;
            color = newColor;
            lineWeight = newLineWeight;

            startPoint.SetXY(startX, startY);
            endPoint.SetXY(endX, endY);

            lineCap = new Ball(realParent, startPoint, 0);
            Console.WriteLine(realParent);
            if (realParent != myGame) realParent.AddChild(lineCap);
            myGame.balls.Add(lineCap);

            lineCap = new Ball(realParent, endPoint, 0);
            if (realParent != myGame) realParent.AddChild(lineCap);
            myGame.balls.Add(lineCap);

            if (!side) myGame.lines.Add(new LineSegment(newRealParent ,endPoint.x, endPoint.y, startPoint.x, startPoint.y, color, 0, global, false, true));
        }

        protected override void RenderSelf(GXPEngine.Core.GLContext glContext)
        {
            Gizmos.RenderLine(startPoint.x, startPoint.y, endPoint.x, endPoint.y, color, lineWeight, global);

            if (arrow)
            {
                Vec2 smallVec = (startPoint + endPoint).Normalized() * -10;
                Vec2 left = new Vec2(-smallVec.y, smallVec.x) + smallVec + endPoint;
                Vec2 right = new Vec2(smallVec.y, -smallVec.x) + smallVec + endPoint;

                Gizmos.RenderLine(endPoint.x, endPoint.y, left.x, left.y, color, lineWeight, global);
                Gizmos.RenderLine(endPoint.x, endPoint.y, right.x, right.y, color, lineWeight, global);
            }
        }
    }
}
