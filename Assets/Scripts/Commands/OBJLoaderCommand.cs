using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OBJLoaderCommand : ICommand
{
    public void Execute()
    {
        OBJControl.Import();
    }

    public void Undo()
    {

    }
}
