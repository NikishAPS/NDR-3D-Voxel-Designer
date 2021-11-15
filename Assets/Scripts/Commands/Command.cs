using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Command
{
    public abstract void Execute();
    //public abstract void Undo();
}
