using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Button : Widget
{
    public string description;
    [SerializeField]
    private Color _default, _hover, _selected;

    public UnityEvent eventsButton;

    private RectTransform _rectTransform;
    private Image _image;

    [SerializeField]
    private bool _press;

    // private DescriptionWidgets descriptionWidgets;

    public override void Init()
    {
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();

        _image.color = _default;

        //descriptionWidgets = GameObject.Find("Canvas").transform.Find("DescriptionWidgets").GetComponent<DescriptionWidgets>();
    }

    public override void Tick()
    {
        if (InRect())
        {
            //descriptionWidgets.SetTarget(description, transform);
            if (!_press)
                _image.color = _hover;
            if (Input.GetMouseButtonDown(0))
            {
                _image.color = _selected;
                _press = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (_press)
                {
                    eventsButton.Invoke();
                }
                _press = false;
            }
        }
        else
        {
            //descriptionWidgets.Hide(transform);

            if (!Input.GetMouseButton(0)) _press = false;


            _image.color = _press ? _selected : _default;

            //_image.color = _selected;
            //if (!Input.GetMouseButton(0))
            //{
            //    _image.color = _default;
            //    _press = false;
            //}
        }
    }

    private bool InRect()
    {
        Vector2 mousePos = Input.mousePosition;

        return mousePos.x > transform.position.x - _rectTransform.rect.width * 0.5f &&
            mousePos.x < transform.position.x + _rectTransform.rect.width * 0.5f &&
            mousePos.y > transform.position.y - _rectTransform.rect.height * 0.5f &&
            mousePos.y < transform.position.y + _rectTransform.rect.height * 0.5f;
    }

    public void SetDefaultColor(Color color)
    {
        _default = color;
    }

}

