using UnityEngine;

public static class MathfExtension
{
    public static float SinDeg(this Mathf mathf, float angle)
    {
        return Mathf.Sin(angle * Mathf.Deg2Rad);
    }

    public static float CosDeg(this Mathf mathf, float angle)
    {
        return Mathf.Cos(angle * Mathf.Deg2Rad);
    }

}
