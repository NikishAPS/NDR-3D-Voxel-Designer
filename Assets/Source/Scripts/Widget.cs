using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Widget : MonoBehaviour
{
    public abstract void Init();
    public abstract void Tick();
}
