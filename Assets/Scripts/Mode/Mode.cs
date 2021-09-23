using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mode : IMouseMove, ILMouseDown, ILMouseUp, IRMouseDown, IRMouseUp
{
    public virtual void OnEnable() { }
    public virtual void OnDisable() { }

    public virtual void OnMouseMove() { }
    public virtual void OnLMouseDown() { }
    public virtual void OnLMouseUp() { }
    public virtual void OnRMouseDown() { }
    public virtual void OnRMouseUp() { }
}

