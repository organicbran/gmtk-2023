using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static float QuadraticBezier(float p1, float p2, float p3, float t)
    {
        return Mathf.Lerp(Mathf.Lerp(p1, p2, t), Mathf.Lerp(p2, p3, t), t);
    }

    public static float CubicBezier(float p1, float p2, float p3, float p4, float t)
    {
        return Mathf.Lerp(QuadraticBezier(p1, p2, p3, t), QuadraticBezier(p2, p3, p4, t), t);
    }

    public static Vector3 QuadraticBezier(Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        return Vector3.Lerp(Vector3.Lerp(p1, p2, t), Vector3.Lerp(p2, p3, t), t);
    }

    public static Vector3 CubicBezier(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t)
    {
        return Vector3.Lerp(QuadraticBezier(p1, p2, p3, t), QuadraticBezier(p2, p3, p4, t), t);
    }
}