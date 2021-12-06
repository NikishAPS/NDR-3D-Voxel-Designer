using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Direction
{
    public static int Left => (int)DirectionType.Left;
    public static int Right => (int)DirectionType.Right;
    public static int Down => (int)DirectionType.Down;
    public static int Up => (int)DirectionType.Up;
    public static int Back => (int)DirectionType.Back;
    public static int Forward => (int)DirectionType.Forward;

    public static readonly byte[] Masks =
    {
        0b000001,
        0b000010,
        0b000100,
        0b001000,
        0b010000,
        0b100000
    };

    public static readonly byte[] InvMasks;

    public static readonly Vector3Int[] Directions =
    {
        new Vector3Int(-1, 0, 0),
        new Vector3Int(+1, 0, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(0, +1, 0),
        new Vector3Int(0, 0, -1),
        new Vector3Int(0, 0, +1),
    };

    static Direction()
    {
        InvMasks = new byte[Masks.Length];
        for (int i = 0; i < InvMasks.Length; i++)
            InvMasks[i] = (byte)~Masks[i];
    }

    public static Vector3Int GetDirectionByType(DirectionType directionType)
    {
        return Directions[(int)directionType];
    }

    public static int GetDirectionIndex(Vector3Int direction)
    {
        for(int i = 0; i < Directions.Length; i++)
        {
            if (Directions[i] == direction) return i;
        }
            
        return -1;
    }

    public static Vector3 GetCursorOffsetAlongAxis(Vector3 worldTarget)
    {
        Vector3 center = Camera.main.WorldToScreenPoint(worldTarget);
        Vector3 mouseDirection = Input.mousePosition - center;

        float maxProjection = 0;
        int index = 0;
        Vector3 direction = Vector3.zero;
        for(int i = 0; i < Directions.Length; i++)
        {
            Vector3 curDirection = Camera.main.WorldToScreenPoint(Directions[i] + worldTarget) - center;
            float angle = Vector3.Angle(mouseDirection, curDirection);

            if (angle < 90)
            {
                float projection = Vector3.Project(mouseDirection, curDirection).magnitude;

                if (projection > maxProjection)
                {
                    index = i;
                    maxProjection = projection;
                    direction = curDirection;
                }
            }
        }

        return Directions[index].ToVector3() * (maxProjection / (direction).magnitude);
    }

    public static Vector3Int GetNormalByView()
    {
        Vector3 viewDirection = Camera.main.transform.forward;

        float minAngle = 361f;
        int minIndex = 0;
        for(int i = 0; i < Directions.Length; i++)
        {
            float angle = Vector3.Angle(viewDirection, Directions[i]);

            if(angle < minAngle)
            {
                minIndex = i;
                minAngle = angle;
            }
        }

        return Directions[minIndex].Abs();
    }

    public static Vector3Int[] GetDirectionsByCursorPlane(Vector3Int worldTarget)
    {
        Vector3 center = Camera.main.WorldToScreenPoint(worldTarget);
        Vector3 mouseDirection = Input.mousePosition - center;

        float minAngle = 361f;
        float prevMinAngle = 361f;
        int directionIndex = 0;
        int prevDirectionIndex = 0;

        for(int i = 0; i < Directions.Length; i++)
        {
            float angle = Vector3.Angle(Camera.main.WorldToScreenPoint(Directions[i] + worldTarget) - center, mouseDirection);
            //angle = Vector3.Dot(Camera.main.WorldToScreenPoint(Directions[i] + worldTarget) - center, mouseDirection) / mouseDirection.magnitude;

            if (angle < minAngle)
            {
                prevMinAngle = minAngle;
                prevDirectionIndex = directionIndex;

                minAngle = angle;
                directionIndex = i;
            }
            else if(angle < prevMinAngle)
            {
                prevMinAngle = angle;
                prevDirectionIndex = i;
            }
        }

        return new Vector3Int[2]
        {
            Directions[directionIndex],
            Directions[prevDirectionIndex]
        };

        //return Directions[directionIndex] + Directions[prevDirectionIndex];
    }

    public static Tuple<Vector3Int, Vector3Int> GetPerpendicularDirections(Vector3Int direction)
    {
        int index = GetDirectionIndex(direction);

        if (index == -1)
            throw new ArgumentException($"Incorrect direction. You should use the direction corresponding to DirectionType: {direction}");

        return GetPerpendicularDirections((DirectionType)GetDirectionIndex(direction));
    }

    public static Tuple<Vector3Int, Vector3Int> GetPerpendicularDirections(DirectionType directionType)
    {
        int index = (int)directionType;

        int index1 = index + 2 > Directions.Length ? index + 2 - Directions.Length : index + 2;
        int index2 = index + 4 > Directions.Length ? index + 4 - Directions.Length : index + 4;

        return new Tuple<Vector3Int, Vector3Int>(Directions[index1], Directions[index2]);
    }

}

//public enum Direction
//{
//    Left, Right, Down, Up, Back, Forward
//}


