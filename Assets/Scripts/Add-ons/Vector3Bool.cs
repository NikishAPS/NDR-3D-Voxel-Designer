using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Vector3Bool
{
    public bool X, Y, Z;

    public Vector3Bool()
    {
        X = Y = Z = false;
    }

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
