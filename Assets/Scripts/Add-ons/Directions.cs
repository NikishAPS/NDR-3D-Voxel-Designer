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

    public static readonly Vector3Int[] Directions =
    {
        new Vector3Int(-1, 0, 0),
        new Vector3Int(+1, 0, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(0, +1, 0),
        new Vector3Int(0, 0, -1),
        new Vector3Int(0, 0, +1),
    };

}

//public enum Direction
//{
//    Left, Right, Down, Up, Back, Forward
//}


