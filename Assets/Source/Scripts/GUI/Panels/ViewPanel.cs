using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPanel : Panel
{
    public Mode Mode => _modes[_mode];

    private Mode[] _modes;
    [SerializeField] private int _mode;

    public override void OnInit()
    {
        _modes = new Mode[]
        {
            new BuildMode(),
            new SelectMode(),
            new EditMode(),
            new OBJMode()
        };

        _mode = 0;
        _modes[_mode].OnEnable();
    }

    public override void OnMouseMove()
    {
        Mode.OnMouseMove();
    }

    public override void OnLMouseDown()
    {
        Mode.OnLMouseDown();
    }

    public override void OnLMouseUp()
    {
        Mode.OnLMouseUp();
    }

    public override void OnRMouseDown()
    {
        Mode.OnRMouseDown();
    }

    public override void OnRMouseUp()
    {
        Mode.OnRMouseUp();
    }

    public void SwitchMode(int mode)
    {
        if (_mode < 0 || _mode >= _modes.Length) return;

        Mode.OnDisable();
        _mode = mode;
        Mode.OnEnable();
    }


    private Mode GetCurMode()
    {
        return _modes[_mode];
    }

}
