using GXPEngine;
using System;
using System.Collections.Generic;

class ColliderManager
{
    public List<CustomCollider> colliders; //List of colliders

    /// <summary>
    /// Constructor
    /// </summary>
    public ColliderManager()
    {
        colliders = new List<CustomCollider>();
        CustomCollider.onCreated += AddCollider;
        CustomCollider.onDestroyed += RemoveCollider;
    }

    /// <summary>
    /// Add a collider to the list
    /// </summary>
    /// <param name="col">Collider to add</param>
    private void AddCollider(CustomCollider col)
    {
        if (!colliders.Contains(col) && col.shouldAddToColliderManager)
        {
            colliders.Add(col);
        }
    }

    /// <summary>
    /// Remove a collider from the list
    /// </summary>
    /// <param name="col">Collider to remove</param>
    private void RemoveCollider(CustomCollider col)
    {
        if (colliders.Contains(col))
        {
            colliders.Remove(col);
        }
    }

    /// <summary>
    /// Get the earliest collision for an object
    /// </summary>
    /// <param name="obj">Object to check for</param>
    /// <returns></returns>
    public CustomCollisionInfo GetEarliestCollision(CustomSprite obj)
    {
        CustomCollisionInfo earliestCollision = null;

        //Check if the object has any colliders
        if (obj.colliders != null && obj.colliders.Count > 0)
        {
            //Foreach collider belonging to the object
            foreach (CustomCollider collider in obj.colliders)
            {
                //Get the earliest collision of the collider
                CustomCollisionInfo newCollision = GetEarliestCollision(collider);

                if (newCollision != null)
                {
                    //Check if the earliest collision doesn't exist, or if the new collision happens earlier
                    if (earliestCollision == null || newCollision.timeOfImpact < earliestCollision.timeOfImpact)
                    {
                        earliestCollision = newCollision;
                    }
                }
            }
        }

        return earliestCollision;
    }

    /// <summary>
    /// Get the earliest collision for a collider
    /// </summary>
    /// <param name="col">Collider to check for</param>
    /// <param name="objectBlackList"></param>
    /// <returns></returns>
    public CustomCollisionInfo GetEarliestCollision(CustomCollider col, List<Type> objectBlackList = null)
    {
        CustomCollisionInfo earliestCollision = null;

        //If the collider is a circle collider
        if (col is CircleCollider circleCol)
        {
            //Foreach collider in the manager
            foreach (CustomCollider other in colliders)
            {
                //Check if the circle collider and the other collider are NOT part of the same parent
                if (circleCol.parentObj == null || other.parentObj == null || circleCol.parentObj != other.parentObj)
                {
                    CustomCollisionInfo newCollision = null;

                    //If the other collider is a line collider
                    if (other is LineCollider line && (objectBlackList == null || !objectBlackList.Contains(other.parentObj.GetType())))
                    {
                        newCollision = CheckCircleOnLineCollision(circleCol, line);
                    }
                    //If the other collider is a circle collider
                    else if (other is CircleCollider circle && (objectBlackList == null || !objectBlackList.Contains(other.parentObj.GetType())))
                    {
                        newCollision = CheckCircleOnCircleCollision(circleCol, circle);
                    }

                    if (newCollision != null)
                    {
                        //Check if the earliest collision doesn't exist, or if the new collision happens sooner
                        if (earliestCollision == null || newCollision.timeOfImpact < earliestCollision.timeOfImpact)
                        {
                            earliestCollision = newCollision;
                        }
                    }
                }
            }
        }

        return earliestCollision;
    }

    /// <summary>
    /// Check two circle colliders for collision
    /// </summary>
    /// <param name="col">Collider to check for</param>
    /// <param name="other">Collider to check against</param>
    /// <returns></returns>
    public static CustomCollisionInfo CheckCircleOnCircleCollision(CircleCollider col, CircleCollider other)
    {
        CustomCollisionInfo earliestCollision = null;

        //Check if the colliders are NOT part of the same parent
        if (col.parentObj == null || other.parentObj == null || col.parentObj != other.parentObj)
        {
            Vec2 u = other.position.VectorTo(col.oldPosition);
            float a = col.velocity.Length() * col.velocity.Length();

            float b = (u * 2).Dot(col.velocity);
            float c = (u.Length() * u.Length()) - ((col.radius + other.radius) * (col.radius + other.radius));

            //If there's an overlap
            if (c < 0f)
            {
                //If the colliders are moving towards each other
                if (b < 0)
                {
                    earliestCollision = new CustomCollisionInfo(col, other, 0);
                }
                else return null;
            }
            //If the collider is moving
            else if (a != 0)
            {
                float D = (b * b) - (4 * a * c);
                if (D >= 0)
                {
                    float t = (-b - Mathf.Sqrt(D)) / (2 * a);

                    //If the collision happens this frame
                    if (t >= 0 && t < 1f)
                    {
                        earliestCollision = new CustomCollisionInfo(col, other, t);
                    }
                }
            }
        }

        return earliestCollision;
    }

    /// <summary>
    /// Check a circle and line collider for collision
    /// </summary>
    /// <param name="col">Collider to check for</param>
    /// <param name="other">Collider to check against</param>
    /// <returns></returns>
    public static CustomCollisionInfo CheckCircleOnLineCollision(CircleCollider col, LineCollider other)
    {
        CustomCollisionInfo earliestCollision = null;

        Vec2 line = other.lineEnd.VectorTo(other.lineStart);

        Vec2 lineNormal = line.Normal();

        Vec2 ballToLineStart = col.oldPosition.VectorTo(other.lineStart);

        float a = lineNormal.Dot(ballToLineStart) - col.radius - (other.lineWidth / 2);
        float b = lineNormal.Dot(col.velocity);

        if (b > 0)
        {
            float t;
            //If the distance between the colliders is 0 or bigger
            if (a > 0) t = a / b;
            else if (a >= -col.radius) t = 0;
            else return null;

            Vec2 POIToLineStart = (col.oldPosition + (col.velocity * t)).VectorTo(other.lineStart);

            float distanceAlongLine = POIToLineStart.Dot(line.Normalized());

            //Check if the circle collider is on the line collider
            if (distanceAlongLine <= line.Length() && distanceAlongLine > 0)
            {
                if (t <= 1 && t >= 0)
                {
                    return new CustomCollisionInfo(col, other, t);
                }
            }
        }

        return earliestCollision;
    }

    /// <summary>
    /// Resolve the collision of a CollisionInfo
    /// </summary>
    /// <param name="colInfo">The CollisionInfo the resolve</param>
    /// <param name="moveBothObjects">Whether both objects should move, or only the first one</param>
    public static void ResolveCollision(CustomCollisionInfo colInfo, bool moveBothObjects = true)
    {
        //If the collider is a circle collider
        if (colInfo.col is CircleCollider col)
        {
            Vec2 PoI = col.position + (col.velocity * colInfo.timeOfImpact);
            col.position = col.oldPosition + (col.velocity * colInfo.timeOfImpact);

            //Calculate the combined bounciness of the two colliders
            float combinedBounciness = col.bounciness + colInfo.other.bounciness;
            combinedBounciness = Mathf.Clamp(combinedBounciness - 1, 0, 1);

            //If the other collider is a line collider
            if (colInfo.other is LineCollider line)
            {
                col.velocity = col.velocity.Reflected(combinedBounciness, line.normal);
            }
            //If the other collider is a circle collider
            else if (colInfo.other is CircleCollider circle)
            {
                Vec2 collisionNormal = circle.position.VectorTo(PoI).Normalized();

                Vec2 relativePosition = circle.position.VectorTo(col.position);
                Vec2 relativeVelocity = col.velocity - circle.velocity;

                //If the colliders are moving towards each other
                if (relativePosition.Dot(relativeVelocity) < 0)
                {
                    //If the other collider is a ball & both objects should move
                    if (circle.parentObj is Ball && moveBothObjects)
                    {
                        Vec2 CoMVelocity = ((col.velocity * col.mass) + (circle.velocity * circle.mass)) / (col.mass + circle.mass);

                        //Calculate the new velocity for both colliders
                        col.velocity = col.velocity - (1 + col.bounciness) * ((col.velocity - CoMVelocity).Dot(collisionNormal)) * collisionNormal;
                        circle.velocity = circle.velocity - (1 + circle.bounciness) * ((circle.velocity - CoMVelocity).Dot(collisionNormal)) * collisionNormal;

                        circle.Collided(colInfo);
                    }
                    else
                    {
                        col.velocity = col.velocity.Reflected(combinedBounciness, collisionNormal);
                    }
                }
            }
            col.Collided(colInfo);
        }
    }
}