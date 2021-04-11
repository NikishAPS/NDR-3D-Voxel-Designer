using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Voxelator;

public class VoxelPickerMaterial: MonoBehaviour
{

    [SerializeField] private InputField colorValue;

    private VoxelsControl _voxelsControl;
    public MessageSystem messageSystem;


    private void Start()
    {
        _voxelsControl = GetComponent<VoxelsControl>();
        Disable();
    }

    private void Update()
    {
        if (_voxelsControl.selectedVoxels.Count == 0) Disable();
        Color color = new Color();
        ColorUtility.TryParseHtmlString(colorValue.text, out color);
        for (int i = 0; i < _voxelsControl.selectedVoxels.Count; i++)
        {
           
            _voxelsControl.selectedVoxels[i].GetComponent<MeshRenderer>().material.color = color;
        }
    }

    public void SetPaletteColor()
    {
        if (_voxelsControl.selectedVoxels.Count == 0)
        {
            messageSystem.AddMessage("Воксель не выделен");
            return;
        }

        colorValue.transform.parent.gameObject.SetActive(true);
        enabled = true;

        foreach (GameObject voxel in _voxelsControl.selectedVoxels)
        {
            voxel.GetComponent<VoxelMaterials>().Standard();
        }


        colorValue.text = "#" + ColorUtility.ToHtmlStringRGBA(_voxelsControl.selectedVoxels[_voxelsControl.selectedVoxels.Count - 1].
            GetComponent<MeshRenderer>().material.color);
    }

    public void Disable()
    {
        colorValue.transform.parent.gameObject.SetActive(false);
        enabled = false;
    }
}
