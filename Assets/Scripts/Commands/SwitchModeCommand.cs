using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchModeCommand : Command
{
    private int _mode = 0;

    public SwitchModeCommand(int mode)
    {
        _mode = mode;
    }

    public override void Execute()
    {
        ProjectPresenter.SwitchMode(_mode);
    }
}
