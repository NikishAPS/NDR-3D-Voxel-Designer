using System.Runtime.CompilerServices;

namespace Source
{
    using UnityEngine;

    public static class Math
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEqual1D(float a, float b) => Mathf.Approximately(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEqual2D(Vector2 a, Vector2 b) => IsEqual1D(a.x + a.y, b.x + b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEqual3D(Vector3 a, Vector3 b) => IsEqual1D(a.x + a.y + a.z, b.x + b.y + b.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetGridCoord1D(float value, float size) => Mathf.FloorToInt(value * size) / size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetGridCoord2D(Vector2 value, float size) => new Vector2(GetGridCoord1D(value.x, size), GetGridCoord1D(value.y, size));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetGridCoord3D(Vector3 value, float size) => new Vector3(GetGridCoord1D(value.x, size), GetGridCoord1D(value.y, size), GetGridCoord1D(value.z, size));
    }
}