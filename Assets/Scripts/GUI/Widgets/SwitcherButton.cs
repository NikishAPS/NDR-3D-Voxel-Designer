using UnityEngine;
using UnityEngine.UI;

public class SwitcherButton : MonoBehaviour
{
    public int Value
    {
        get => _value;
        set => _value = value;
    }

    [SerializeField] private int _value;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Image _image;

    public void Init()
    {
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
    }

    public bool Inside(Vector3 point)
    {
        return _rectTransform.Inside(point);
    }

    public void SetColor(Color color)
    {
        _image.color = color;
    }

}
