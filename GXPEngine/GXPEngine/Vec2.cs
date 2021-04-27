using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

public struct Vec2
{
    public float x;
    public float y;

    public Vec2(float newX = 0, float newY = 0)
    {
        x = newX;
        y = newY;
    }

    public static Vec2 operator +(Vec2 a, Vec2 b)
    {
        return new Vec2(a.x + b.x, a.y + b.y);
    }

    public static Vec2 operator -(Vec2 a, Vec2 b)
    {
        return new Vec2(a.x - b.x, a.y - b.y);
    }
    
    public static Vec2 operator *(Vec2 a, float scale)
    {
        return new Vec2(a.x * scale, a.y * scale);
    }

    public static Vec2 operator *(float scale, Vec2 a)
    {
        return new Vec2(a.x * scale, a.y * scale);
    }

    public static Vec2 operator *(Vec2 a, Vec2 b)
    {
        return new Vec2(a.x * b.x, a.y * b.y);
    }

    public static Vec2 operator /(Vec2 a, float scale)
    {
        return new Vec2(a.x / scale, a.y / scale);
    }

    public static Vec2 operator /(Vec2 a, Vec2 b)
    {
        return new Vec2(a.x / b.x, a.y / b.y);
    }

    public override string ToString()
    {
        return String.Format("({0} | {1})", x, y);
    }

    public float Length()
    {
        return Mathf.Sqrt(x * x + y * y);
    }

    public void SetXY(float newX, float newY)
    {
        x = newX;
        y = newY;
    }

    public static float Deg2Rad(float degrees)
    {
        return degrees * (Mathf.PI / 180);
    }

    public static float Rad2Deg(float radians)
    {
        return radians * (180 / Mathf.PI);
    }

    public void SetAngleDegrees(float degrees)
    {
        degrees = Deg2Rad(degrees);
        float r = Length();

        float sin = Mathf.Sin(degrees);
        float cos = Mathf.Cos(degrees);
        if (Mathf.Abs(sin) < 0.00001f) sin = 0;
        if (Mathf.Abs(cos) < 0.00001f) cos = 0;

        SetXY(r * cos, r * sin);
    }

    public void SetAngleRadians(float radians)
    {
        float r = Length();

        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
        if (Mathf.Abs(sin) < 0.00001f) sin = 0;
        if (Mathf.Abs(cos) < 0.00001f) cos = 0;

        SetXY(r * cos, r * sin);
    }

    public void RotateDegrees(float degrees)
    {
        degrees = Deg2Rad(degrees);

        float sin = Mathf.Sin(degrees);
        float cos = Mathf.Cos(degrees);
        if (Mathf.Abs(sin) < 0.00001f) sin = 0;
        if (Mathf.Abs(cos) < 0.00001f) cos = 0;

        SetXY(cos * x - sin * y, sin * x + cos * y);
    }

    public void RotateRadians(float radians)
    {
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
        if (Mathf.Abs(sin) < 0.00001f) sin = 0;
        if (Mathf.Abs(cos) < 0.00001f) cos = 0;
        SetXY(cos * x - sin * y, sin * x + cos * y);
    }

    public void RotateAroundDegrees(float degrees, Vec2 rotationPoint)
    {
        this -= rotationPoint;

        RotateDegrees(degrees);

        this += rotationPoint;
    }

    public void RotateAroundRadians(float radians, Vec2 rotationPoint)
    {
        this -= rotationPoint;

        RotateRadians(radians);

        this += rotationPoint;
    }

    public float GetAngleRadians()
    {
        float angle = Mathf.Atan2(y, x);
        return angle;
    }

    public float GetAngleDegrees()
    {
        float angle = Mathf.Atan2(y, x);
        return Rad2Deg(angle);
    }

    public static Vec2 GetUnitVectorDeg(float degrees, float length)
    {
        Vec2 newVec2 = new Vec2(length, 0);
        newVec2.SetAngleDegrees(degrees);
        return newVec2;
    }

    public static Vec2 GetUnitVectorRad(float radians, float length)
    {
        Vec2 newVec2 = new Vec2(length, 0);
        newVec2.SetAngleRadians(radians);
        return newVec2;
    }

    public static Vec2 RandomUnitVector(float length)
    {
        float degrees = Utils.Random(0, 360);
        Vec2 newVec2 = new Vec2(length, 0);
        newVec2.RotateDegrees(degrees);
        return newVec2;
    }

    public Vec2 Normal()
    {
        return new Vec2(-y, x).Normalized();
    }

    public void Normalize()
    {
        if (x == 0 && y == 0)
        {
            SetXY(0, 0);
        }
        else SetXY(x / Length(), y / Length());
    }

    public Vec2 Normalized()
    {
        if (x == 0 && y == 0)
        {
            return new Vec2(0, 0);
        }
        else return new Vec2(x / this.Length(), y / this.Length());
    }

    public static Vec2 Projection(Vec2 a, Vec2 b)
    {
        float ProjectionLength = a.Dot(b);
        return b.Normalized() * ProjectionLength;
    }

    public float Dot(Vec2 other)
    {
        return this.x * other.x + this.y * other.y;
    }

    public void Reflect(Vec2 pNormal, float pBounciness = 1)
    {
        this = this - (1 + pBounciness) * (this.Dot(pNormal)) * pNormal;
    }

    public Vec2 Reflected(float bounciness, Vec2 reflector)
    {
        Vec2 reflected = this - (1 + bounciness) * (Dot(reflector)) * reflector;
        return reflected;

    }

    public Vec2 VectorTo(Vec2 destination)
    {
        return (destination - this);
    }
}
