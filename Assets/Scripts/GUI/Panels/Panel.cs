using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Panel : MonoBehaviour, IMouseMove, ILMouseDown, ILMouseHold, ILMouseUp, IRMouseDown, IRMouseUp
{
    public bool Active => gameObject.activeSelf;

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
    public PanelTitle Title { get; private set; }
    public bool Inside => _rectTransform.Inside(Input.mousePosition);
    public Widget[] Widgets { get; private set; }
    private RectTransform _rectTransform;

    public void Open()
    {
        gameObject.SetActive(true);
        StartCoroutine(Opening());
        OnOpen();
    }

    public void Close()
    {
        StartCoroutine(Closing());
        OnClose();
    }

    public virtual void OnInit() { }
    public virtual void OnOpen() { }
    public virtual void OnClose() { }
    public virtual void OnMouseMove() { }
    public virtual void OnLMouseDown() { }
    public virtual void OnLMouseHold() { }
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
        Title = GetComponentInChildren<PanelTitle>();
        transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
        OnInit();
    }

    private IEnumerator Opening()
    {
        StopCoroutine(Closing());

        while (transform.localScale != Vector3.one)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.one, Time.deltaTime * 10f);
            yield return null;
        }
    }

    private IEnumerator Closing()
    {
        StopCoroutine(Opening());

        while (transform.localScale != Vector3.zero)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, Time.deltaTime * 10f);
            yield return null;
        }

        gameObject.SetActive(false);
    }

}
