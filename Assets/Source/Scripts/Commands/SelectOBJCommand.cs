using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NDR.UI;

public class SelectOBJCommand : Command
{
    public ImportedModel ImportedModel { get; set; }
    [SerializeField] private InputField _positionX, _positionY, _positionZ;
    [SerializeField] private InputField _scaleX, _scaleY, _scaleZ;

    public override void Execute()
    {
        
    }
}
