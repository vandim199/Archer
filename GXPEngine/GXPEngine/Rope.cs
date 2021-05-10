using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    class Rope : PhysicsBody
    {

        private Vec2 _startPosition;
        private int _segmentLength;

        public Rope(Vec2 startPosition, Vec2 endPosition, int segmentLength = 3): base(1, false, true, true, false, false)
        {
            _startPosition = startPosition;
            _segmentLength = segmentLength;

            SetupCollider(Mathf.Abs((startPosition - endPosition).Length()), (endPosition - startPosition).GetAngleDegrees());
        }

        private void SetupCollider(float ropeLength, float ropeAngle)
        {
            float numberOfSegments = ropeLength / (float)_segmentLength;
            numberOfSegments = Mathf.Round(numberOfSegments);
            float trueSegmentLength = ropeLength / numberOfSegments;

            Vec2 segmentPosition = _startPosition;
            Vec2 nextPosition = Vec2.GetUnitVectorDeg(ropeAngle, trueSegmentLength);

            for (int i = 0; i <= numberOfSegments; i++)
            {
                bool isSolid = false;

                if (i == 0)
                    isSolid = true;

                segmentPosition += nextPosition;
                AddPoint(segmentPosition, isSolid);
            }
        }

        public void Break(Connection connection)
        {
            for(int i = connections.Count - 1; i >= 0; i--)
            {
                if(connections[i] != connection)
                {
                    connections[i].LateDestroy();
                    connections.RemoveAt(i);

                    points[i].LateDestroy();
                    points.RemoveAt(i);
                }
                else
                {
                    return;
                }
            }
        }
    }
}
