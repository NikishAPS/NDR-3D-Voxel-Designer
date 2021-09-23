using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Button : Widget
{
    [SerializeField] private UnityEvent Click;



    public override void OnClick()
    {
        Click?.Invoke();
    }

}
