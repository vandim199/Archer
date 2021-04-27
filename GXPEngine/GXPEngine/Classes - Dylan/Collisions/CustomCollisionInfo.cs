using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class CustomCollisionInfo
{
    public float timeOfImpact; //The time of impact of the collision
    public CustomCollider col; //The collider to initiate the collision
    public CustomCollider other; //The collider that was collided with

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="newCol">The collider to initiate the collision</param>
    /// <param name="newOther">The collider that was collided with</param>
    /// <param name="newToI">The time of impact of the collision</param>
    public CustomCollisionInfo(CustomCollider newCol, CustomCollider newOther, float newToI)
    {
        timeOfImpact = newToI;
        col = newCol;
        other = newOther;
    }
}
