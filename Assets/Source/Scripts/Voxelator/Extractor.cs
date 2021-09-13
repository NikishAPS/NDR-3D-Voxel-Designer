using UnityEngine;

public class Extractor : MonoBehaviour
{
    private static Extractor _this;
    [SerializeField] private CoordinateDisplay _coordinates;
    private static bool _activeCoordinates;

    public static bool Active
    {
        get => _this.gameObject.activeSelf;
        set
        {
            _this.gameObject.SetActive(value);
            _this._coordinates.Active = value && _activeCoordinates;
        }
    }

    public static Vector3 Position
    {
        get
        {
            return _this.transform.position;
        }
        set
        {
            _this.transform.position = value;

            _this._coordinates.Position = _this.transform.position;
            _this._coordinates.Title = "Position";
            _this._coordinates.Value = _this.transform.position;
        }
    }

    public static Vector3 Scale
    {
        get
        {
            return _this.transform.localScale;
        }
        set
        {
            _this.transform.localScale = new Vector3().Set(value);

            _this._coordinates.Position = _this.transform.position;
            _this._coordinates.Title = "Scale";
            _this._coordinates.Value = _this.transform.localScale;
        }
    }

    public static void OnMouseMove()
    {
        _this._coordinates.Position = _this.transform.position;
    }

    //public static void SetRotation(Quaternion rot)
    //{
    //    Vector3 euler = rot.eulerAngles;
    //    //transform.eulerAngles = new Vector3(Mathf.Abs(euler.x), Mathf.Abs(euler.y), Mathf.Abs(euler.z));
    //    _this.transform.eulerAngles = euler;
    //}

    public static bool ActiveCoordinates
    {
        set
        {
            _activeCoordinates = value;
            _this._coordinates.Active = value;
        }
    }

    private void Awake()
    {
        _this = FindObjectOfType<Extractor>();
        ActiveCoordinates = true;
        Active = false;
    }

    private void OnEnable()
    {
        CameraControl.MoveEvent += OnMouseMove;
    }

    private void OnDisable()
    {
        CameraControl.MoveEvent -= OnMouseMove;
    }
}
