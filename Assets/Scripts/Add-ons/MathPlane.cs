using UnityEngine;
using System.Collections;

public class MathPlane
{
    public float A { get; private set; }
    public float B { get; private set; }
    public float C { get; private set; }
    public float D { get; private set; }
    public Vector3 Normal
    {
        get => new Vector3(A, B, C);
        set
        {
            A = value.x;
            B = value.y;
            C = value.z;
        }
    }

    public MathPlane(Vector3 v1, Vector3 v2)
    {
        Init(v1, v2, v1 + v2);
    }

    public MathPlane(Vector3 point1, Vector3 point2, Vector3 point3)
    {
        Init(point1, point2, point3);
    }

    public Vector3 GetIntersection(Vector3 point, Vector3 direction)
    {
        direction.Normalize();

        Vector3 coef = new Vector3(A, B, C);
        float t = -(Vector3.Dot(coef, point) + D) / Vector3.Dot(coef, direction);

        return point + direction * t;
    }

    private void Init(Vector3 point1, Vector3 point2, Vector3 point3)
    {
        //A = v1.y * v2.z - v2.y * v1.z;
        //B = -v1.x * v2.z + v2.x * v1.z;
        //C = v1.x * v2.y - v2.x * v1.y;

        A = (point2.y - point1.y) * (point3.z - point1.z) - (point3.y - point1.y) * (point2.z - point1.z);
        B = -(point2.x - point1.x) * (point3.z - point1.z) + (point3.x - point1.x) * (point2.z - point1.z);
        C = (point2.x - point1.x) * (point3.y - point1.y) - (point3.x - point1.x) * (point2.y - point1.y);

        D = -point1.x * A - point1.y * B - point1.z * C;
    }
}
