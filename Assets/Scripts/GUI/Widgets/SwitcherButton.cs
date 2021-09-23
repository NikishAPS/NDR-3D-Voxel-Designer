using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitcherButton : Widget
{
    public int Value => _value;
    [SerializeField] private int _value;
    private Switcher _switcher;

    public override void OnInit()
    {
        _switcher = GetComponentInParent<Switcher>();
    }

    public override void OnClick()
    {
        _switcher.Switch(this);
    }
}
