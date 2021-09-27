using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct Vector3Bool
{
    public bool X { get; set; }
    public bool Y { get; set; }
    public bool Z { get; set; }

    public Vector3Bool(bool x, bool y, bool z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public static Vector3Bool False => new Vector3Bool(false, false, false);

    public static Vector3Bool True => new Vector3Bool(true, true, true);

    public bool IsTrue => X || Y || Z;

    public bool IsPureTrue => X && Y && Z;

}
