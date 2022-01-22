using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchModeCommand : ICommand
{
    private int _prevMode = 0;
    private int _currentMode = 0;

    public SwitchModeCommand(int mode)
    {
        _prevMode = _currentMode;
        _currentMode = mode;
    }

    public void Execute()
    {
        Presenter.SwitchMode(_currentMode);
    }

    public void Undo()
    {
        Presenter.SwitchMode(_prevMode);
    }

}
