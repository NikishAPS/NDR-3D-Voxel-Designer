using UnityEngine;

public class Axes : MonoBehaviour
{
    private static Axes _this;

    public static bool Active
    {
        get
        {
            return _this.gameObject.activeSelf;
        }
        set
        {
            _this.gameObject.SetActive(value);
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
            OnResize();
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
            _this.transform.localScale = value + _this._offsetScale;
        }
    }
    public static Vector3 OffsetScale
    {
        get
        {
            return _this._offsetScale;
        }
        set
        {
            _this._offsetScale = value;
            Scale = Scale;
        }
    }
    public static bool ScaleAxesActive
    {
        set
        {
            foreach (ScaleAxis scaleAxis in _this._scaleAxes)
            {
                scaleAxis.gameObject.SetActive(value);
            }
        }
    }

    public delegate Vector3? DragEvent(Vector3 dragValue); //return drag value

    public static DragEvent DragPosition; 
    public static DragEvent DragScale;

    private Axis[] _positionAxes, _scaleAxes;
    private Vector3 _offsetScale;
    [SerializeField] private float _pivotSize = 0.1f;
    [SerializeField] private Axis _highlightedAxis;
    [SerializeField] private bool _isDrag = false;



    public static Axis GetHighlightedAxis()
    {
        return _this._highlightedAxis;
    }

    public static bool IsHighlightedAxis()
    {
        return _this._highlightedAxis != null;
    }

    public static void OnResize()
    {
        Scale = Vector3.one * _this._pivotSize * Vector3.Distance(Position, Camera.main.transform.position);
    }



    private void Awake()
    {
        _positionAxes = FindObjectsOfType<PositionAxis>();
        _scaleAxes = FindObjectsOfType<ScaleAxis>();
    }

    private void Start()
    {
        Active = false;
        ScaleAxesActive = false;
    }

    private void OnEnable()
    {
        InputEvent.MouseMove += OnMouseMove;
        InputEvent.LMouseDown += OnLMouseDown;
        InputEvent.LMouseHold += OnLMouseHold;
        InputEvent.LMouseUp += OnLMouseUp;
        CameraControl.MoveEvent += OnResize;

        _this = FindObjectOfType<Axes>();

    }

    private void OnDisable()
    {
        InputEvent.MouseMove -= OnMouseMove;
        InputEvent.LMouseDown -= OnLMouseDown;
        InputEvent.LMouseHold -= OnLMouseHold;
        InputEvent.LMouseUp -= OnLMouseUp;
        CameraControl.MoveEvent -= OnResize;
    }

    private void OnMouseMove()
    {
        if (!_isDrag && !InputEvent.GetLMouseHold)
        {
            RaycastHit hit;
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, LayerMask.GetMask("Axis"));
            SetSelectedAxis(hit.transform?.GetComponent<Axis>());
        }
    }

    private void OnLMouseDown()
    {
        _highlightedAxis?.OnStartDrag();
    }

    private void OnLMouseHold()
    {
        if (_highlightedAxis != null)
        {
            _isDrag = true;

            _highlightedAxis.OnDrag();
        }
    }

    private void OnLMouseUp()
    {
        if (_isDrag)
        {
            _highlightedAxis.OnEndDrag();
            SetSelectedAxis(null);
            _isDrag = false;
            OnMouseMove();
        }
    }

    private void SetSelectedAxis(Axis axis)
    {
        _highlightedAxis?.SetHighlight(false);
        _highlightedAxis = axis;
        _highlightedAxis?.SetHighlight(true);
    }

}