using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Direction
{
    public static int Left => 0;
    public static int Right => 1;
    public static int Down => 2;
    public static int Up => 3;
    public static int Back => 4;
    public static int Forward => 5;

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


