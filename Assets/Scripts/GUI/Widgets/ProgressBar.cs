using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : Widget
{
    public float Progress
    {
        get => _progress;
        set
        {
            _progress = value;
            _progressImage.transform.localScale = new Vector3(value, _progressImage.transform.localScale.y, _progressImage.transform.localScale.z);
            _title.text = ((int)(value * 100f)).ToString() + "%";
        }
    }

    [SerializeField] private float _progress;
    [SerializeField] private Text _title;
    [SerializeField] private Image _progressImage;

    public override void OnInit()
    {
        _image.color = _selectedColor;
        _progressImage.color = _defaultColor;
        Progress = 0;
    }

}
