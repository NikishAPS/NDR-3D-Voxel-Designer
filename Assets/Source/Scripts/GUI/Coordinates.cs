using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Coordinates : MonoBehaviour
{
    [SerializeField]
    private Text _x, _y, _z;

    private RectTransform _rectTransform;

    public bool Active
    {
        get => gameObject.activeSelf;
        set
        {
            gameObject.SetActive(value);
        }
    }

    public Vector3 Value
    {
        get
        {
            return new Vector3(float.Parse(_x.text), float.Parse(_y.text), float.Parse(_z.text));
        }
        set
        {
            _x.text = "X = " + value.x.ToString();
            _y.text = "Y = " + value.y.ToString();
            _z.text = "Z = " + value.z.ToString();
        }
    }

    public Vector3 Position
    {
        get => _rectTransform.position;
        set
        {
            _rectTransform.position = value;
        }

    }



    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
}
