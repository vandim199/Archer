using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

abstract class CustomCollider : GameObject
{
    public CustomSprite parentObj; //The parent object of the collider (leave empty for an independent collider)
    public static event Action<CustomCollider> onCreated; //Event for when the collider is created
    public static event Action<CustomCollider> onDestroyed; //Event for when the collider is destroyed
    public static event Action<CustomCollisionInfo> onCollided; //event for when the collider collides
    public bool shouldAddToColliderManager { get; private set; } //Whether the collider should be added to the collider manager

    private Vec2 _independentVelocity; //The velocity of the collider IF the collider is independent
    public Vec2 velocity //The velocity of the collider, or of its parent object depending on independance
    {
        get
        {
            if (parentObj == null)
            {
                return _independentVelocity;
            }
            else
            {
                return parentObj.velocity;
            }
        }
        set
        {
            if (parentObj == null)
            {
                _independentVelocity = value;
            }
            else
            {
                parentObj.velocity = value;
            }
        }
    }

    private float _bounciness; //The bounciness of the collider IF the collider is independant
    public float bounciness //The bounciness of the collider or its parent depending on independance
    {
        get
        {
            if(parentObj == null)
            {
                return _bounciness;
            }
            else
            {
                return parentObj.bounciness;
            }
        }
        set
        {
            _bounciness = value;
        }
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="addtoManager">Whether the collider should be added to the collider manager</param>
    /// <param name="newParent">The parent of the collider (leave empty for independent collider)</param>
    public CustomCollider(bool addtoManager = false, CustomSprite newParent = null)
    {
        shouldAddToColliderManager = addtoManager;
        parentObj = newParent;

        if(onCreated != null)
        {
            onCreated.Invoke(this);
        }
    }

    public void Update()
    {
        DrawMe();
    }

    /// <summary>
    /// Make the child colliders implement a DrawMe method
    /// </summary>
    public abstract void DrawMe();

    /// <summary>
    /// When the collider collides
    /// </summary>
    /// <param name="colInfo">The CollisionInfo of the collision</param>
    public void Collided(CustomCollisionInfo colInfo)
    {
        onCollided.Invoke(colInfo);
    }

    protected override void OnDestroy()
    {
        if(onDestroyed != null)
        {
            onDestroyed.Invoke(this);
        }

        base.OnDestroy();
    }
}