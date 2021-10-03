using System.Collections.Generic;
using UnityEngine;

public class Reflector
{
    public readonly Vector3Int FieldSize;
    public Vector3Bool Mirror { set
        {
            //int k = ((value.X) ? 1 : 0) + ((value.Y) ? 1 : 0) + ((value.Z) ? 1 : 0);
            //_reflectedPositions = new Vector3[k * (k - 1) + 1];

            List<Vector3> reflectionSides = new List<Vector3>();
            if (value.X) reflectionSides.Add(new Vector3(1, 0, 0));
            if (value.Y) reflectionSides.Add(new Vector3(0, 1, 0));
            if (value.Z) reflectionSides.Add(new Vector3(0, 0, 1));
            if (value.X && value.Y) reflectionSides.Add(new Vector3(1, 1, 0));
            if (value.X && value.Z) reflectionSides.Add(new Vector3(1, 0, 1));
            if (value.Y && value.Z) reflectionSides.Add(new Vector3(0, 1, 1));
            if (value.X && value.Y && value.Z) reflectionSides.Add(new Vector3(1, 1, 1));

            _reflectionSides = reflectionSides.ToArray();
        }
    }
    private Vector3[] _reflectionSides;

    public Reflector(Vector3Int fieldSize)
    {
        FieldSize = fieldSize;
        Mirror = Vector3Bool.False;
    }

    public Vector3 GetReflectedPositionByX(Vector3 position)
    {
        return new Vector3(FieldSize.x - position.x - 1, position.y, position.z);
    }

    public Vector3 GetReflectedPositionByY(Vector3 position)
    {
        return new Vector3(position.x, FieldSize.y - position.y - 1, position.z);
    }

    public Vector3 GetReflectedPositionByZ(Vector3 position)
    {
        return new Vector3(position.x, position.y, FieldSize.z - position.z - 1);
    }

    public Vector3[] GetReflectedPositions(Vector3 position)
    {
        Vector3[] reflectedPositions = new Vector3[_reflectionSides.Length];
        for (int i = 0; i < _reflectionSides.Length; i++)
            reflectedPositions[i] = position + GetReflectedPositionByXYZ(position).Mul(_reflectionSides[i]) - position.Mul(_reflectionSides[i]);

        return reflectedPositions;
    }

    private Vector3 GetReflectedPositionByXYZ(Vector3 position)
    {
        return FieldSize - position - Vector3.one;
    }

}

