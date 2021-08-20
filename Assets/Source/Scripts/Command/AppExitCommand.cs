using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppExitCommand : Command
{
    [SerializeField]
    private Panel _exitPanel;

    public override void Execute()
    {
        if (!Project.Saved)
        {
            _exitPanel.Open();
        }
        else
        {
            Project.Ouit();
        }
    }
}
