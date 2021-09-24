using UnityEngine;
using UnityEngine.UI;
using CustomUnityEvents;

public class Slider : Widget
{
    public float Value
    {
        get => _value;
        set
        {
            _value = (int)Mathf.Clamp(value, _min, _max);

            float positionX = ((_value - _min) / (_max - _min)) * _backgroud.rect.width + _backgroud.position.x - _backgroud.rect.width / 2;
            _handle.position = new Vector3(positionX, _handle.position.y, _handle.position.z);

            _inputField.text = _value.ToString();
        }
    }

    [SerializeField] private int _value = 0;
    [SerializeField] private float _min = -1f, _max = 1f, _step = 0.1f;
    [SerializeField] private RectTransform _backgroud;
    [SerializeField] private RectTransform _handle;
    [SerializeField] private InputField _inputField;
    [SerializeField] private EventInt _initField;

    public override void OnInitImage()
    {
        _image = _handle.GetComponent<Image>();
    }

    public override void OnInit()
    {
        Value = _value;
    }

    public override void SetColor(Color color)
    {
        _image.color = color;
    }

    public override void OnHold()
    {
        Value = _min + (_max - _min) *
            ((InputEvent.MousePosition.x - (_backgroud.position.x - _backgroud.rect.width / 2)) / (_backgroud.rect.width));

        _initField?.Invoke(_value);
    }

    public void GetInputFieldValue()
    {
        Value = int.Parse(_inputField.text);
    }
}
