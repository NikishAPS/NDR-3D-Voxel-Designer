using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxelator;

public class SelectMode : Mode
{
    public List<GameObject> selectedVoxels;


    public SelectMode(VoxelsControl voxelsControl) : base(voxelsControl) { }


    public override void UpdateMode()
    {

    }
}
