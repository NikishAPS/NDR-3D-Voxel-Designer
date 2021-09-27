using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchModeCommand : Command
{
    private int _prevMode = 0;
    private int _mode = 0;

    public SwitchModeCommand(int mode)
    {
        _prevMode = _mode;
        _mode = mode;
    }

    public override void Execute()
    {
        Presenter.SwitchMode(_mode);
    }
}
