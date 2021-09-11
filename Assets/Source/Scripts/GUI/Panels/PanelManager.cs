using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PanelManager : MonoBehaviour, IMouseMove, ILMouseDown, ILMouseUp, IRMouseDown, IRMouseUp
{
    public static Panel[] Panels { get; private set; }

    private static Panel _pinPanel;
    private static LinkedList<Panel> _openedPanel = new LinkedList<Panel>();
    private static BoolFunction _process;
    private static LinkedList<BoolFunction> _processes = new LinkedList<BoolFunction>();
    [SerializeField] private Panel _cursorPanel;
    [SerializeField] private Widget _cursorWidget;

    
    
    public static T GetPanel<T>() where T : Panel
    {
        foreach (Panel panel in Panels)
        {
            if (panel.GetType() == typeof(T))
                return (T)panel;
        }

        return null;
    }

    public static void PinPanel(Panel panel)
    {
        _pinPanel = panel;
    }

    public static void AddPanel(Panel panel)
    {
        _openedPanel.AddLast(panel);
    }

    public static void RemovePanel(Panel panel)
    {
        _openedPanel.Remove(panel);
    }

    public static void AddProcess(BoolFunction process)
    {
        _processes.AddLast(process);
    }

    public void OnMouseMove()
    {
        _cursorPanel = GetCursorPanel();
        _cursorPanel?.OnMouseMove();

        Panel panel = GetCursorPanel();
        Widget widget = null;
        if (panel != null)
        {
            panel.OnMouseMove();
            widget = GetCursorWidget();

            widget?.OnHover();
        }

        _cursorPanel = panel;
        if (widget != _cursorWidget) _cursorWidget?.OnLeave();
        _cursorWidget = widget;
    }

    public void OnLMouseDown()
    {
        _cursorPanel?.OnLMouseDown();
        if(_cursorWidget != null)
        {
            _cursorWidget.OnSelect();
            InputEvent.MouseMove -= OnMouseMove;
        }
    }

    public void OnLMouseUp()
    {
        _cursorPanel?.OnLMouseUp();

        if (_cursorWidget != null)
        {
            Widget widget = GetCursorWidget();
            if (widget == _cursorWidget)
            {
                _cursorWidget.OnDown();
            }
            OnMouseMove();
            InputEvent.MouseMove += OnMouseMove;
        }
    }

    public void OnRMouseDown()
    {
        _cursorPanel?.OnRMouseDown();
    }

    public void OnRMouseUp()
    {
        _cursorPanel?.OnRMouseUp();
    }



    private void Awake()
    {
        Panels = FindObjectsOfType<Panel>();

        InputEvent.MouseMove += OnMouseMove;
        InputEvent.LMouseDown += OnLMouseDown;
        InputEvent.LMouseUp += OnLMouseUp;
        InputEvent.RMouseDown += OnRMouseDown;
        InputEvent.RMouseUp += OnRMouseUp;
    }

    private void Start()
    {
        foreach(Panel panel in Panels)
        {
            panel.OnInit();
        }
    }

    private void Update()
    {
        var process = _processes.First;
        while (process != null)
        {
            var next = process.Next;
            if (process.Value.Invoke())
            {
                _processes.Remove(process);
            }
            process = next;
        }
    }

    private Panel GetCursorPanel()
    {
        foreach (Panel panel in _openedPanel)
        {
            if (_pinPanel != null && panel != _pinPanel)
                continue;

            if (panel.Inside)
                return panel;
        }

        return null;
    }

    private Widget GetCursorWidget()
    {
        Widget[] widgets = _cursorPanel?.Widgets;
        if (widgets != null)
        {
            foreach (Widget widget in widgets)
            {
                if(widget.Inside())
                {
                    return widget;
                }
            }
        }

        return null;
    }

}
