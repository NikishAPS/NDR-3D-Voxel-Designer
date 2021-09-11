using UnityEngine;
using UnityEngine.UI;

public class CoordinateDisplay : MonoBehaviour
{
    [SerializeField] private Text _title, _x, _y, _z;
    private RectTransform _rectTransform;

    public bool Active
    {
        get => gameObject.activeSelf;
        set => gameObject.SetActive(value);
    }

    public Vector3 Position
    {
        set => _rectTransform.position = Camera.main.WorldToScreenPoint(value);
    }

    public Vector3 XPosition
    {
        set => _x.transform.position = Camera.main.WorldToScreenPoint(value);
    }

    public Vector3 YPosition
    {
        set => _y.transform.position = Camera.main.WorldToScreenPoint(value);
    }

    public Vector3 ZPosition
    {
        set => _z.transform.position = Camera.main.WorldToScreenPoint(value);
    }

    public string Title
    {
        set => _title.text = value;
    }

    public Vector3? Value
    {
        set
        {
            if (value == null)
            {
                _x.text = "---";
                _y.text = "---";
                _z.text = "---";
            }
            else
            {
                Vector3 v = (Vector3)value;
                _x.text = "X = " + v.x.ToString();
                _y.text = "Y = " + v.y.ToString();
                _z.text = "Z = " + v.z.ToString();
            }
        }
    }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
}
