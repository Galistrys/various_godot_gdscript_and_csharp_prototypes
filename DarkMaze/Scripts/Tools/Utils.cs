using Godot;
using System;

static class Utils
{
    public static uint CombinePhysicsLayers(params PhysicsLayer[] layers)
    {
        PhysicsLayer result = 0;

        foreach (PhysicsLayer layer in layers)
        {
            result = result | layer;
        }

        return (uint)result;
    }

    public static float AngleBetweenVector3(Vector3 a, Vector3 b)
    {
        Vector2 tempA = new Vector2(a.z, a.x);
        Vector2 tempB = new Vector2(b.z, b.x);
        return tempA.AngleTo(tempB);
    }

    public static float Convert3DAngleY(Vector3 a, Vector3 b)
    {
        Vector2 tempA = new Vector2(Mathf.Cos(a.y), Mathf.Sin(a.y));
        Vector2 tempB = new Vector2(Mathf.Cos(b.y), Mathf.Sin(b.y));
        return tempA.AngleTo(tempB);
    }
}
