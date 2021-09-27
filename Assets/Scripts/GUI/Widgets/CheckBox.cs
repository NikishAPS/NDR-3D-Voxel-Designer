using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomUnityEvents;

public class CheckBox : Widget
{
    public bool Value { get => _mark.enabled; set => _mark.enabled = value; }
    [SerializeField] private Image _mark;
    [SerializeField] private EventBool _initField;

    public override void OnInit()
    {
        _mark.enabled = false;
    }

    public override void OnClick()
    {
        _mark.enabled = !_mark.enabled;
        _initField?.Invoke(_mark.enabled);
    }

}
