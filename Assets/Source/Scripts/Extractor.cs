using UnityEngine;

public class Extractor : MonoBehaviour
{
    private static Extractor _this;
    [SerializeField] private CoordinateDisplay _coordinates;

    public static bool Active
    {
        get => _this.gameObject.activeSelf;
        set
        {
            _this.gameObject.SetActive(value);
            _this._coordinates.Active = value;
        }
    }

    public static void SetPosition(Vector3 position)
    {
        _this.transform.position = position;

        _this._coordinates.Position = _this.transform.position;
        _this._coordinates.Title = "Position";
        _this._coordinates.Value = _this.transform.position;
    }

    public static void OnMouseMove()
    {
        _this._coordinates.Position = _this.transform.position;
    }

    public static Vector3 GetPosition()
    {
        return _this.transform.position;
    }

    public static void SetRotation(Quaternion rot)
    {
        Vector3 euler = rot.eulerAngles;
        //transform.eulerAngles = new Vector3(Mathf.Abs(euler.x), Mathf.Abs(euler.y), Mathf.Abs(euler.z));
        _this.transform.eulerAngles = euler;
    }

    public static void SetScale(Vector3 scale)
    {
        _this.transform.localScale = new Vector3().Set(scale);

        _this._coordinates.Position = _this.transform.position;
        _this._coordinates.Title = "Scale";
        _this._coordinates.Value = _this.transform.localScale;
    }

    public static Vector3 GetScale()
    {
        return _this.transform.localScale;
    }

    private void Awake()
    {
        _this = FindObjectOfType<Extractor>();
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
