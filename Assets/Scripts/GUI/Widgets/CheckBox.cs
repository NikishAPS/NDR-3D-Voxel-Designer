using UnityEngine;
using UnityEngine.UI;
using CustomUnityEvents;

public class CheckBox : Widget
{
    public bool Value { get => _active; set => _active = value; }
    [SerializeField] private bool _active = false;
    [SerializeField] private Image _mark;
    [SerializeField] private EventBool _initField;

    public override void OnInit()
    {
        _mark.enabled = _active;
    }

    public override void OnClick()
    {
        _active = !_active;
        _mark.enabled = _active;
        _initField?.Invoke(_active);
    }

}
