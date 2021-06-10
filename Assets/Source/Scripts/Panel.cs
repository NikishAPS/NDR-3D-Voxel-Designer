using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : MonoBehaviour
{
    [SerializeField]
    private RectTransform _rectTransform;

    [SerializeField]
    private Widget[] widgets;

    [SerializeField]
    private bool isActive = true;
    [SerializeField]
    private bool openStart = false;

    private bool _isPanel = false;
    public bool IsPanel { get; private set; }


    public void Awake()
    {
        widgets = GetComponentsInChildren<Widget>();
        foreach (Widget widget in widgets)
        {
            widget.Init();
        }
    }

    public void Start()
    {
        if (!isActive)
        {
            transform.localScale = Vector3.zero;
            gameObject.SetActive(false);
            return;
        }

        if (openStart)
        {
            transform.localScale = Vector3.zero;

            isActive = true;
        }
    }


    public void Tick()
    {
        if(!gameObject.activeSelf) { IsPanel = false; return; }

        IsPanel = InRect();

        if(isActive)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.one, Time.deltaTime * 10f);

            if (IsPanel)
            {
                foreach (Widget widget in widgets)
                {
                    widget.Tick();
                }
            }
        }
        else
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, Time.deltaTime * 10f);

            if (transform.localScale == Vector3.zero)
                gameObject.SetActive(false);
        }

    }

    private bool InRect()
    {
        Vector2 mousePos = Input.mousePosition;


        return mousePos.x > _rectTransform.position.x - _rectTransform.rect.width * 0.5f &&
            mousePos.x < _rectTransform.position.x + _rectTransform.rect.width * 0.5f &&
            mousePos.y > _rectTransform.position.y - _rectTransform.rect.height * 0.5f &&
            mousePos.y < _rectTransform.position.y + _rectTransform.rect.height * 0.5f;
    }


    public void Close()
    {
        isActive = false;
        //StartCoroutine(Resize(Vector3.zero));
    }

    public void Open()
    {
        isActive = true;
        gameObject.SetActive(true);
        //StartCoroutine(Resize(Vector3.one));
    }

    private IEnumerator Resize(Vector3 size)
    {
        while (transform.localScale != size)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, size, Time.deltaTime * 0.1f);
            yield return null;
        }

        if (size == Vector3.zero)
            gameObject.SetActive(false);
    }
}
