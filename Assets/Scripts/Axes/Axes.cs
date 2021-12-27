using UnityEngine;

public class Axes : MonoBehaviour
{
    public static bool Active
    {
        get
        {
            return _this.gameObject.activeSelf;
        }
        set
        {
            _this.gameObject.SetActive(value);
            _this._coordinates.Active = value;
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
    public static bool ScaleActive
    {
        set
        {
            foreach (ScaleAxis scaleAxis in _this._scaleAxes)
            {
                scaleAxis.gameObject.SetActive(value);
            }
        }
    }
    public static IDrag DragObject
    {
        get => _dragObject;
        set
        {
            _dragObject = value;
        }
    }

    private static Axes _this;
    private static IDrag _dragObject;
    private static DragTransform _offset = new DragTransform();
    [SerializeField] private CoordinateDisplay _coordinates;

    private Axis[] _positionAxes, _scaleAxes;
    private Vector3 _offsetScale;
    [SerializeField] private float _pivotSize = 0.1f;
    [SerializeField] private Axis _highlightedAxis;
    [SerializeField] private bool _isDrag = false;
    private DragTransform _initialDragValue;

    public bool _thisB;
    public void Update()
    {
        _thisB = _this != null;
    }

    public delegate void DragResult();

    public static void SetDragObject(IDrag dragObject)
    {
        _dragObject = dragObject;
    }

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
        //float size = _this._pivotSize * Vector3.Distance(Position, Camera.main.transform.position);
        float size = _this._pivotSize * CameraController.Size;

        Scale = Vector3.one * size;

        DragTransform? dragCoordinates = _dragObject.GetDragCoordinates();
        if (dragCoordinates == null) _this._coordinates.Value = null;
        else _this._coordinates.Value = dragCoordinates?.Position;

        size *= 1.5f;

        _this._coordinates.Position = Position;
        _this._coordinates.XPosition = Position + Vector3.right * size;
        _this._coordinates.YPosition = Position + Vector3.up * size;
        _this._coordinates.ZPosition = Position + Vector3.forward * size;
    }

    private void Awake()
    {
        _positionAxes = FindObjectsOfType<PositionAxis>();
        _scaleAxes = FindObjectsOfType<ScaleAxis>();
        _this = FindObjectOfType<Axes>();
    }

    private void Start()
    {
        Active = false;
        ScaleActive = false;
    }

    private void OnEnable()
    {
        InputEvent.MouseMove += OnMouseMove;
        InputEvent.LMouseDown += OnLMouseDown;
        InputEvent.LMouseHold += OnLMouseHold;
        InputEvent.LMouseUp += OnLMouseUp;
        CameraController.MoveEvent += OnResize;
    }

    private void OnDisable()
    {
        InputEvent.MouseMove -= OnMouseMove;
        InputEvent.LMouseDown -= OnLMouseDown;
        InputEvent.LMouseHold -= OnLMouseHold;
        InputEvent.LMouseUp -= OnLMouseUp;
        CameraController.MoveEvent -= OnResize;
    }

    private void OnMouseMove()
    {
        if (!_isDrag && !InputEvent.IsLMouseHold)
        {
            RaycastHit hit;
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, LayerMask.GetMask("Axis"));
            SetSelectedAxis(hit.transform?.GetComponent<Axis>());
        }
    }

    private void OnLMouseDown()
    {
        _initialDragValue = new DragTransform(transform);
        _highlightedAxis?.OnStartDrag(_initialDragValue);
        _offset = DragTransform.Zero;
    }

    //private void OnLMouseHold()
    //{
    //    if (_highlightedAxis != null)
    //    {
    //        _isDrag = true;

    //        DragTransform dragValue =_highlightedAxis.GetDragValue();
    //        MonoBehaviour.print(_highlightedAxis.GetDragValue());
    //        Position = _initialDragValue.Position + dragValue.Position;
    //        if(_dragObject.OnDrag(dragValue))
    //        {
    //            _initialDragValue.Position += dragValue.Position;
    //            _highlightedAxis.OffsetDragPoint(dragValue);
    //            _offset += dragValue;
    //        }
    //    }
    //}

    public Vector3 val;

    private void OnLMouseHold()
    {
        if (_highlightedAxis != null)
        {
            _isDrag = true;

            DragTransform dragValue = _highlightedAxis.GetDragValue();
            DragTransform dragResult;

            val = dragValue.Position;

            Position = _initialDragValue.Position + dragValue.Position;

            _dragObject.OnDrag(dragValue, out dragResult);

            _initialDragValue.Position += dragResult.Position;
            _highlightedAxis.OffsetDragPoint(dragResult);
            _offset += dragResult;
        }
    }

    private void OnLMouseUp()
    {
        if (_isDrag)
        {
            //_highlightedAxis.OnEndDrag();
            _dragObject.OnEndDrag(_offset);
            Position = _initialDragValue.Position;
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