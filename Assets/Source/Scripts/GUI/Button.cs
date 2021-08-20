using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Button : Widget
{
    [SerializeField]
    private Color _default, _hover, _selected;

    public UnityEvent ClickButton;

    private Image _image;

    [SerializeField]
    private bool _press;

    public override void Init()
    {
        BaseInit();

        _image = GetComponent<Image>();

        _image.color = _default;
    }

    public override void Tick()
    {
        if (InRect())
        {
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
                    ClickButton?.Invoke();
                    ExecuteCommands();
                }
                _press = false;
            }
        }
        else
        {
            if (!Input.GetMouseButton(0)) _press = false;

            _image.color = _press ? _selected : _default;
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

