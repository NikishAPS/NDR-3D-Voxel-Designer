using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressPanel : Panel
{
    public ProgressBar ProgressBar { get; private set; }

    public override void OnInit()
    {
        ProgressBar = GetComponentInChildren<ProgressBar>();
    }
}
