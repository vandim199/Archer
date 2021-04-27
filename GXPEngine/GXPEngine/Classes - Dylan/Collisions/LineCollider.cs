using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class LineCollider : CustomCollider
{
    private Vec2 _relativeStart; //The start of the line, relative to its parent
    public Vec2 lineStart
    {
        get
        {
            if(parentObj != null)
            {
                Vec2 returnVec = new Vec2(parentObj.position.x + _relativeStart.x, parentObj.position.y + _relativeStart.y);
                returnVec.RotateAroundDegrees(parentObj.rotation, parentObj.position);
                return returnVec;
            }
            else
            {
                return _relativeStart;
            }
        }
    } //The true start of the line

    private Vec2 _relativeEnd; //The end of the line, relative to its parent
    public Vec2 lineEnd
    {
        get
        {
            if(parentObj != null)
            {
                Vec2 returnVec = new Vec2(parentObj.position.x + _relativeEnd.x, parentObj.position.y + _relativeEnd.y);
                returnVec.RotateAroundDegrees(parentObj.rotation, parentObj.position);
                return returnVec;
            }
            else
            {
                return _relativeEnd;
            }
        }
    } //The true end of the line

    public int lineWidth; //The width of the line
    public Vec2 normal
    {
        get
        {
            return lineStart.VectorTo(lineEnd).Normal();
        }
    } //The normal of the line

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="relativeStart">The relative start of the line</param>
    /// <param name="relativeEnd">The relative end of the line</param>
    /// <param name="newLineWidth">The width of the line</param>
    /// <param name="addToColliderManager">Whether the collider should be added to the collider manager</param>
    /// <param name="newParent">The parent of the collider (leave empty for independant collider)</param>
    public LineCollider(Vec2 relativeStart, Vec2 relativeEnd, int newLineWidth = 0, bool addToColliderManager = true, CustomSprite newParent = null) : base(addToColliderManager, newParent)
    {
        _relativeStart = relativeStart;
        _relativeEnd = relativeEnd;
        lineWidth = newLineWidth;
    }

    /// <summary>
    /// Draws a line for the collider
    /// </summary>
    public override void DrawMe()
    {
        Gizmos.DrawLine(lineStart.x, lineStart.y, lineEnd.x, lineEnd.y, color: 4294902015);
    }
}
