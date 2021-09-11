using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Widget : MonoBehaviour
{
    public Vector2 Position
    {
        get => _rectTransform != null ? (Vector2)_rectTransform.position : Vector2.zero;
        set => _rectTransform.position = value;
    }
    public Vector2 Size
    {
        get => new Vector2(_rectTransform.rect.width, _rectTransform.rect.height);
        set => _rectTransform.sizeDelta = value;
    }
    public Image Image
    {
        get => _image;
    }
    public Color DefaultColor
    {
        get => _defaultColor;
        set => _defaultColor = value;
    }
    public Color HoverColor
    {
        get => _hoverColor;
        set => _hoverColor = value;
    }
    public Color SelectedColor
    {
        get => _selectedColor;
        set => _selectedColor = value;
    }

    private RectTransform _rectTransform;
    protected Image _image;
    [SerializeField] protected Color
        _defaultColor = new Color(91f/255f, 91f / 255f, 91f / 255f),
        _hoverColor = new Color(178f / 255f, 178f / 255f, 178f / 255f),
        _selectedColor = new Color(231f / 255f, 231f / 255f, 231f / 255f);


    public virtual bool Inside()
    {
        return _rectTransform.Inside(Input.mousePosition);
    }
            
    public virtual void OnInit()
    {

    }

    public virtual void OnHover()
    {
        if(_image != null)
        _image.color = _hoverColor;
    }

    public virtual void OnSelect()
    {
        if (_image != null)
            _image.color = _selectedColor;
    }

    public virtual void OnDown()
    {

    }

    public virtual void OnLeave()
    {
        if (_image != null)
            _image.color = _defaultColor;
    }



    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();

        if (_image != null) _image.color = _defaultColor;


    }

    private void Start()
    {
        OnInit();
    }
}
