using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxelator;

public abstract class Mode
{
    protected VoxelsControl _voxelsControl;

    public Mode(VoxelsControl voxelsControl)
    {
        _voxelsControl = voxelsControl;
    }

    public Mode()
    {

    }

    //objects
    protected static GameObject extractor;
    protected static Transform voxelArea;
    protected static MyGUI.MyFlag autoFit;
    protected static Voxelator.CreateInstanceVoxel createInstanceVoxel;

    //paraparameters
    protected static RaycastHit raycastHit;
    protected static bool hit;

    //components
    protected static MyGUI.CursorPanelsGUI cursorPanelsGUI;
    protected static Voxelator.CoordinateSystem coordinateSystem;
    protected static Voxelator.GridControl gridControl;




    public abstract void UpdateMode();

    protected void Raycasthit()
    {
        if (cursorPanelsGUI.cursorInGameScene)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            hit = Physics.Raycast(ray, out raycastHit, Mathf.Infinity);
        }
        else
        {
            hit = false;
        }
    }

    protected bool MouseButton(int index, bool hold)
    {
        if (hold)
        {
            if (Input.GetMouseButton(index))
                return cursorPanelsGUI.cursorInGameScene;
        }
        else
        {
            if (Input.GetMouseButtonDown(index))
                return cursorPanelsGUI.cursorInGameScene;
        }
        return false;
    }

    protected void VoxelArea(Transform voxelAre)
    {

    }
}

