using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMirrorCommand : ICommand
{
    private Vector3Bool _previousValue;
    private Vector3Bool _value;

    public SetMirrorCommand(Vector3Bool value)
    {
        _previousValue = _value;
        _value = value;
    }

    public void Execute()
    {
        //ChunkManager.Mirror = _value;
        PanelManager.GetPanel<ProjectPanel>().Mirror = _value;
    }

    public void Undo()
    {

    }

}
