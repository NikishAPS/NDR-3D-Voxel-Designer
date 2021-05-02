using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxelator;

public abstract class Mode
{
    public abstract void Invoke();
    public abstract void Disable();
    public abstract void Enable();
}

