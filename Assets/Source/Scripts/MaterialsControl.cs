using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyGUI;

namespace Voxelator
{
    public class MaterialsControl : MonoBehaviour
    {
        public MyColor myColor;

        public Material standard;

        [SerializeField]
        private Material[] materials;

        private VoxelsControl voxelsControl;
        private VoxelMaterials voxelMaterials;
        private CursorPanelsGUI cursorPanelsGUI;

        [HideInInspector]
        public bool colorActive;

        void Start()
        {
            voxelsControl = GetComponent<VoxelsControl>();
            voxelMaterials = GetComponent<VoxelMaterials>();
            cursorPanelsGUI = GetComponent<CursorPanelsGUI>();
        }


        void Update()
        {

        }

        public void ColorActive(bool active)
        {
            cursorPanelsGUI.windowGUI[1].panel.gameObject.SetActive(active);
            colorActive = active;
        }

        public void SetMaterial(string hex, int index)
        {
            Color color;
            ColorUtility.TryParseHtmlString(hex, out color);
            materials[index].color = color;

            for (int i = 0; i < voxelsControl.selectedVoxels.Count; i++)
            {
                voxelsControl.selectedVoxels[i].GetComponent<VoxelMaterials>().standard = materials[index];
                voxelsControl.selectedVoxels[i].GetComponent<VoxelMaterials>().Standard();
            }
        }

        public Material GetMaterial(int index)
        {
            if (index == -1) return standard;
            return materials[index];
        }

        public int GetMaterialIndex()
        {
            if (voxelsControl.selectedVoxels.Count > 0)
            {
                for (int i = 0; i < materials.Length; i++)
                {
                    if (voxelsControl.selectedVoxels[voxelsControl.selectedVoxels.Count - 1].GetComponent<VoxelMaterials>().standard == materials[i])
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public void SetColor(int index, Color color)
        {
            materials[index].color = color;
        }

        public void SetMaterialColor(int index, Color color)
        {
            materials[index].color = color;
        }

        public void ResetColor()
        {
            for (int i = 0; i < voxelsControl.selectedVoxels.Count; i++)
            {
                voxelsControl.selectedVoxels[i].GetComponent<VoxelMaterials>().standard = standard;
                voxelsControl.selectedVoxels[i].GetComponent<VoxelMaterials>().Standard();
            }
        }

        public Material GetMaterialByCurIndex()
        {
            return GetMaterial(myColor.index);
        }
    }
}
