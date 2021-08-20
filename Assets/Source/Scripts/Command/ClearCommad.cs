using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCommad : Command
{
    public override void Execute()
    {
        ChunksManager.Release();
    }
}
