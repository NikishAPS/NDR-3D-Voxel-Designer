using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Curtain : MonoBehaviour
{
    public bool Active
    {
        get => _active;
        set
        {
            _active = value;
            if (_active) StartCoroutine(Activating());
            else StartCoroutine(Deactivating());
        }
    }
    public float Alpha { get => _image.color.a; set => _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, value); }

    [SerializeField] private bool _active;
    [SerializeField] private float _alphaTarget = 0.5f;
    [SerializeField] private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _image.rectTransform.sizeDelta = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
    }

    public int Index;
    public Transform tt;
    public void Update()
    {
        Index = tt.GetSiblingIndex();
    }

    private IEnumerator Activating()
    {
        StopCoroutine(Deactivating());

        while (Alpha != _alphaTarget)
        {
            Alpha = Mathf.MoveTowards(Alpha, _alphaTarget, Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator Deactivating()
    {
        StopCoroutine(Activating());

        while (Alpha != 0)
        {
            Alpha = Mathf.MoveTowards(Alpha, 0, Time.deltaTime);
            yield return null;
        }
    }
}
