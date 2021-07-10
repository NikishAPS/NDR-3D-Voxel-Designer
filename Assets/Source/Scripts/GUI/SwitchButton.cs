using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchButton : MonoBehaviour
{
    [SerializeField]
    private int _value;

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

    public int GetValue()
    {
        return _value;
    }

    public void SetColor(Color color)
    {
        _image.color = color;
    }

}
