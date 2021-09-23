using System.Collections;
using UnityEngine;
using CustomUnityEvents;

public class ColorPickerWIdget : Widget
{
    [SerializeField] private EventColor _initColor;

    [SerializeField] private RectTransform _HS;
    [SerializeField] private RectTransform _HSPoint;
    [SerializeField] private RectTransform _V;
    [SerializeField] private RectTransform _VPoint;

    [SerializeField] private Slider _rSlider;
    [SerializeField] private Slider _gSlider;
    [SerializeField] private Slider _bSlider;

    private Void _moveHSVPoint;

    private RGBColor _rgb = new RGBColor(255, 255, 255);
    private HSVColor _hsv = new HSVColor(0f, 0f, 1f);

    public override void OnInit()
    {
        
    }

    public override void OnSelect()
    {
        _moveHSVPoint = null;
        if (Vector3.Distance(_HS.position, InputEvent.MousePosition) < _HS.rect.width / 2)
            _moveHSVPoint = MoveHSPoint;
        else if (_V.Inside(InputEvent.MousePosition))
            _moveHSVPoint = MoveVPoint;
    }

    public override void OnHold()
    {
        _moveHSVPoint?.Invoke();
    }

    public override void OnLMouseUp()
    {
        _moveHSVPoint = null;
    }

    public void SetR(int r)
    {
        _rgb.R = r;
        ConvertToHSV();
        OnChangeColor();
    }

    public void SetG(int g)
    {
        _rgb.G = g;
        ConvertToHSV();
        OnChangeColor();
    }

    public void SetB(int b)
    {
        _rgb.B = b;
        ConvertToHSV();
        OnChangeColor();
    }

    private void MoveHSPoint()
    {
        _HSPoint.position = InputEvent.MousePosition;

        if (Vector3.Distance(_HS.position, _HSPoint.position) > _HS.rect.width / 2)
            _HSPoint.position = _HS.position +  (InputEvent.MousePosition - _HS.position).normalized * (_HS.rect.width / 2);

        Vector3 dir = _HSPoint.position - _HS.position;
        float angleUp = Vector3.Angle(Vector3.up, dir);
        float angleRight = Vector3.Angle(Vector3.right, dir);

        float angle = (angleUp < 90 && angleRight < 90 || angleUp > 90 && angleRight < 90) ?
            angleUp : 360 - angleUp;

        _hsv.H = angle / 360f;
        _hsv.S = dir.magnitude / (_HS.rect.width / 2);

        ConvertToRGB();

        OnChangeColor();
    }

    private void MoveVPoint()
    {
        float positionY = Mathf.Clamp(InputEvent.MousePosition.y, _V.position.y - _V.rect.height / 2, _V.position.y + _V.rect.height / 2);
        _VPoint.position = new Vector3(_VPoint.position.x, positionY, _VPoint.position.z);

        _hsv.V = (positionY - _V.position.y + _V.rect.height / 2) / (_V.rect.height);

        _HS.GetComponent<UnityEngine.UI.Image>().color = new Color(_hsv.V, _hsv.V, _hsv.V, 1);

        ConvertToRGB();

        OnChangeColor();
    }

    private void ConvertToRGB()
    {
        _rgb = ColorConverter.ConvertHSV2RGB(_hsv);

        _rSlider.Value = _rgb.R;
        _gSlider.Value = _rgb.G;
        _bSlider.Value = _rgb.B;
    }

    private void ConvertToHSV()
    {
        _hsv = ColorConverter.ConvertRGB2HSV(_rgb);

        _HSPoint.position = new Vector3(Mathf.Cos((_hsv.H * 360f + 90f) * Mathf.Deg2Rad), Mathf.Sin((_hsv.H * 360f + 90f) * Mathf.Deg2Rad)) *
            _hsv.S * _HS.rect.width / 2 + _HS.position;

        _VPoint.position = new Vector2(_VPoint.position.x, _V.position.y - _V.rect.height / 2 + _hsv.V * _V.rect.height);
        _HS.GetComponent<UnityEngine.UI.Image>().color = new Color(_hsv.V, _hsv.V, _hsv.V, 1);

    }

    private void OnChangeColor()
    {
        _initColor?.Invoke(_rgb.ToColor());
    }

}
