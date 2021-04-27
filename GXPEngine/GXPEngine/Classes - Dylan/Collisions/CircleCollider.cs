using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class CircleCollider : CustomCollider
{
    private Vec2 _relativePosition; //The (relative) position of the collider
    public Vec2 position //The position of the collider
    {
        get
        {
            if(parentObj == null)
            {
                return _relativePosition;
            }
            else
            {
                Vec2 returnVec = new Vec2(parentObj.position.x + _relativePosition.x, parentObj.position.y + _relativePosition.y);
                returnVec.RotateAroundDegrees(parentObj.rotation, parentObj.position);
                return returnVec;
            }
        }
        set
        {
            if(parentObj == null)
            {
                _relativePosition = value;
            }
            else
            {
                parentObj.position = value;
            }
        }
    }

    private Vec2 _oldPosition; //The old relative position of the collider

    public Vec2 oldPosition //The old position of the collider
    {
        get
        {
            if(parentObj == null)
            {
                return _oldPosition;
            }
            else
            {
                Vec2 returnVec = new Vec2(parentObj.oldPosition.x + _relativePosition.x, parentObj.oldPosition.y + _relativePosition.y);
                returnVec.RotateAroundDegrees(parentObj.oldRotation, parentObj.oldPosition);
                return returnVec;
            }
        }
        set
        {
            if(parentObj == null)
            {
                _oldPosition = value;
            }
            else
            {
                parentObj.oldPosition = value;
            }
        }
    }

    public int radius; //The radius of the collider
    private int _density = 1; //The densite of the collider
    public int mass //The mass of the collider (density * radius)
    {
        get
        {
            return radius * _density;
        }
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="relativePosition">The position relative to its parent</param>
    /// <param name="newRadius">The radius of the collider</param>
    /// <param name="addToColliderManager">Whether the collider should be added to the manager</param>
    /// <param name="newParent">The parent of the collider (Leave empty if the collider is independent)</param>
    public CircleCollider(Vec2 relativePosition, int newRadius = 0, bool addToColliderManager = true, CustomSprite newParent = null) : base(addToColliderManager, newParent)
    {
        radius = newRadius;
        _relativePosition = relativePosition;
    }

    /// <summary>
    /// Draws a circle for the collider
    /// </summary>
    public override void DrawMe()
    {
        Gizmos.DrawLine(position.x - radius, position.y, position.x + radius, position.y);
        Gizmos.DrawLine(position.x, position.y - radius, position.x, position.y + radius);
    }
}
