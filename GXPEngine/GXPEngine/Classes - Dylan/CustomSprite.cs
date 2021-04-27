using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

class CustomSprite : Sprite
{
    public List<CustomCollider> colliders; //A list of colliders belonging to this object
    public Vec2 velocity; //The velocity

    public Vec2 oldPosition; //The old position
    public Vec2 position
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
    } //The position

    public float oldRotation; //The old rotation
    public float bounciness; //The bounciness

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="sprite">The sprite image to use</param>
    /// <param name="newBounciness">The bounciness of the object</param>
    public CustomSprite(string sprite, float newBounciness = 0) : base(sprite, addCollider:false)
    {
        bounciness = newBounciness;
        colliders = new List<CustomCollider>();
    }

    /// <summary>
    /// Creates the colliders for the object
    /// </summary>
    public virtual void CreateColliders()
    {
        AddColliders();
    }

    /// <summary>
    /// Adds the colliders to the list of colliders
    /// </summary>
    public void AddColliders()
    {
        if(colliders != null)
        {
            foreach(CustomCollider col in colliders)
            {
                AddChild(col);
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}
