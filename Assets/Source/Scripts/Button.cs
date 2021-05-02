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

    public bool press;

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

            if (!press)
                _image.color = _hover;
            if (Input.GetMouseButtonDown(0))
            {
                _image.color = _selected;
                press = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (press)
                {
                    eventsButton.Invoke();
                }
                press = false;
            }
        }
        else
        {
            //descriptionWidgets.Hide(transform);
            _image.color = _selected;
            if (!Input.GetMouseButton(0))
            {
                _image.color = _default;
                press = false;
            }
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

}

