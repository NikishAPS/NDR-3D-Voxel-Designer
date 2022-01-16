using UnityEngine;
using System.Collections.Generic;

public class PanelManager : MonoBehaviour, IMouseMove, ILMouseDown, ILMouseHold, ILMouseUp, IRMouseDown, IRMouseUp
{
    public static Panel[] Panels { get; private set; }

    private static Panel _pinPanel;
    private static Curtain _curtain;
    [SerializeField] private Panel _cursorPanel;
    [SerializeField] private Widget _cursorWidget;

    private bool _isLMouseDowned = false;
    
    public static T GetPanel<T>() where T : Panel
    {
        foreach (Panel panel in Panels)
        {
            if (panel.GetType() == typeof(T))
                return (T)panel;
        }

        return null;
    }

    public static void PinThePanel(Panel panel)
    {
        if (panel != null)
        {
            _curtain.transform.SetSiblingIndex(panel.transform.GetSiblingIndex() - 1);
            _curtain.Active = true;
        }
        else
        {
            _curtain.transform.SetSiblingIndex(0);
            _curtain.Active = false;
        }

        _pinPanel = panel;
    }

    public static void CloseAll()
    {
        foreach (Panel currentPanel in Panels)
            if (currentPanel.Active)
                currentPanel.Close();
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
            _isLMouseDowned = true;
        }
    }

    public void OnLMouseHold()
    {
        _cursorWidget?.OnHold();
    }

    public void OnLMouseUp()
    {
        _cursorPanel?.OnLMouseUp();
        _cursorWidget?.OnLMouseUp();

        if (_cursorWidget != null && _isLMouseDowned)
        {
            Widget widget = GetCursorWidget();
            if (widget == _cursorWidget)
            {
                _cursorWidget.OnClick();
            }
            OnMouseMove();
            InputEvent.MouseMove += OnMouseMove;
            _isLMouseDowned = false;
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
        _curtain = FindObjectOfType<Curtain>();

        InputEvent.MouseMove += OnMouseMove;
        InputEvent.LMouseDown += OnLMouseDown;
        InputEvent.LMouseHold += OnLMouseHold;
        InputEvent.LMouseUp += OnLMouseUp;
        InputEvent.RMouseDown += OnRMouseDown;
        InputEvent.RMouseUp += OnRMouseUp;
    }

    private Panel GetCursorPanel()
    {
        foreach (Panel panel in Panels)
        {
            if (!panel.Active) continue;

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
                if(widget.Active && widget.Inside())
                {
                    return widget;
                }
            }
        }

        return null;
    }

}
