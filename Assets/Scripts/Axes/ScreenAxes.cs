using UnityEngine;

public class ScreenAxes : MonoBehaviour, IMouseMove, ILMouseDown
{
    public static ScreenAxis CurAxis { get; private set; }
    private static ScreenAxes _this;

    [SerializeField] private Camera _axesCamera;
    [SerializeField] private float _size = 0.015f;
    [SerializeField] private RectTransform _screenPoint;

    public static bool Active
    {
        get => _this.gameObject.activeSelf;
        set => _this.gameObject.SetActive(value);
    }

    public void UpdateAxes()
    {
        transform.position = CameraController.MainCamera.ScreenToWorldPoint(_screenPoint.position);
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one * _size * CameraController.Distance;

        _axesCamera.orthographicSize = CameraController.MainCamera.orthographicSize;
        _axesCamera.nearClipPlane = CameraController.MainCamera.nearClipPlane;
    }

    public void OnMouseMove()
    {
        RaycastHit hit;
        Physics.Raycast(CameraController.MainCamera.ScreenPointToRay(Input.mousePosition), out hit, LayerMask.GetMask("ScreenAxis"));
        SetCurrentAxis(hit.transform?.GetComponent<ScreenAxis>());
    }

    public void OnLMouseDown()
    {
        if(CurAxis != null)
        {
            CameraController.SetViewDirectino(CurAxis.Direction);
        }
    }



    private void Awake()
    {
        _this = FindObjectOfType<ScreenAxes>();

        InputEvent.MouseMove += OnMouseMove;
        InputEvent.WindowResize += UpdateAxes;
        CameraController.MoveEvent += UpdateAxes;

        gameObject.SetActive(false);
    }

    private void Start()
    {
        UpdateAxes();
    }

    private void SetCurrentAxis(ScreenAxis axis)
    {
        if (CurAxis != null) CurAxis.Highlighted = false;
        CurAxis = axis;
        if (CurAxis != null) CurAxis.Highlighted = true;
    }
}
