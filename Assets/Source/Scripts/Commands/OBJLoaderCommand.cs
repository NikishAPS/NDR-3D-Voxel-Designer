using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OBJLoaderCommand : Command
{
    public override void Execute()
    {
        OBJControl.Import();
    }
}
