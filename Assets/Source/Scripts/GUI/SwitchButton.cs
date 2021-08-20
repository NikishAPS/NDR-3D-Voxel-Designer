using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchButton : MonoBehaviour
{
    [SerializeField]
    private int _value;

    [SerializeField]
    private Color _default, _hover, _selected;

    [SerializeField]
    private RectTransform _rectTransform;
    private Image _image;


    public void Init()
    {
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
    }

    public bool InRect()
    {
        return ControlGUI.InRect(_rectTransform);
    }

    public RectTransform GetRect()
    {
        return _rectTransform;
    }

    public int GetValue()
    {
        return _value;
    }

    public void SetDefaultColor()
    {
        _image.color = _default;
    }

    public void SetSelectedColor()
    {
        _image.color = _selected;
    }

    public void SetHoverColor()
    {
        _image.color = _hover;
    }

}
