using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : MonoBehaviour, IMouseMove, ILMouseDown, ILMouseUp, IRMouseDown, IRMouseUp
{
    public Vector2 Position
    {
        get => _rectTransform.position;
        set => _rectTransform.position = value;
    }
    public Vector2 Size
    {
        get => new Vector2(_rectTransform.rect.width, _rectTransform.rect.height);
        //set => _rectTransform.sizeDelta = value;
    }
    public bool Inside => _rectTransform.Inside(Input.mousePosition);
    public Widget[] Widgets { get; private set; }
    private RectTransform _rectTransform;



    public void Open()
    {
        OnOpen();

        PanelManager.AddProcess(Opening);
        PanelManager.AddPanel(this);
    }

    public void Close()
    {
        OnClose();

        PanelManager.AddProcess(Closing);
        PanelManager.RemovePanel(this);
    }

    public virtual void OnInit() { }
    public virtual void OnOpen() { }
    public virtual void OnClose() { }
    public virtual void OnMouseMove() { }
    public virtual void OnLMouseDown() { }
    public virtual void OnLMouseUp() { }
    public virtual void OnRMouseDown() { }
    public virtual void OnRMouseUp() { }




    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        Widgets = GetComponentsInChildren<Widget>();
    }

    private void Start()
    {
        transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }

    private bool Closing()
    {
        transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, Time.deltaTime * 10f);

        if (transform.localScale == Vector3.zero)
        {
            gameObject.SetActive(false);
            return true;
        }

        return false;
    }

    private bool Opening()
    {
        gameObject.SetActive(true);

        transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.one, Time.deltaTime * 10f);

        if(transform.localScale == Vector3.one)
        {
            return true;
        }

        return false;
    }


}
