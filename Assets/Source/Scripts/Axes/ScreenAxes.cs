using UnityEngine;

public class ScreenAxes : MonoBehaviour, IMouseMove, ILMouseDown
{
    private static ScreenAxes _this;

    [SerializeField] private float _size = 0.015f;
    [SerializeField] private RectTransform _screenPoint;
    private ScreenAxis _curAxis;


    public static bool Active
    {
        get => _this.gameObject.activeSelf;
        set => _this.gameObject.SetActive(value);
    }


    public void OnCameraMove()
    {
        transform.position = Camera.main.ScreenToWorldPoint(_screenPoint.position);
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one * _size;
    }

    public void OnMouseMove()
    {
        RaycastHit hit;
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, LayerMask.GetMask("ScreenAxis"));
        SetCurrentAxis(hit.transform?.GetComponent<ScreenAxis>());
    }

    public void OnLMouseDown()
    {
        if(_curAxis != null)
        {

        }
    }



    private void Awake()
    {
        _this = FindObjectOfType<ScreenAxes>();

        InputEvent.MouseMove += OnMouseMove;
        InputEvent.LMouseDown += OnLMouseDown;
        CameraControl.MoveEvent += OnCameraMove;

        gameObject.SetActive(false);
    }

    private void SetCurrentAxis(ScreenAxis axis)
    {
        if (_curAxis != null) _curAxis.Highlighted = false;
        _curAxis = axis;
        if (_curAxis != null) _curAxis.Highlighted = true;
    }
}
