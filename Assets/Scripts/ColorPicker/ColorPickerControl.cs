using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPickerControl : MonoBehaviour
{
    [SerializeField] private InputField hexColor;

    private Image _image;
    private RectTransform _rectTransform;


    private void Start()
    {
        Disable();
    }

    private void Update()
    {
        Color color = new Color();
        ColorUtility.TryParseHtmlString(hexColor.text, out color);

        _image.color = color;

    }

    public void SetTarget(Image image)
    {
        _image = image;
        hexColor.text = "#" + ColorUtility.ToHtmlStringRGB(_image.color);

        gameObject.SetActive(true);
    }

    public void SetPosition(Vector2 pos)
    {
        _rectTransform.position = pos;
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
