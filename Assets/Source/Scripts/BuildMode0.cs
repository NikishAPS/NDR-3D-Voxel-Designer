using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxelator;

public class BuildMode : Mode
{
    private bool voxelAreaXZ;
    private float voxelAreaOffsetY;

    public BuildMode(VoxelsControl voxelsControl) : base(voxelsControl) { }

    public override void UpdateMode()
    {
        Raycasthit();

        if (hit)
        {
            //extractor
            ExtractorPos();
            ExtractorRot();
            extractor.SetActive(true);


            //building area
            if (MouseButton(0, true))
            {
                if (!voxelAreaXZ)
                {
                    voxelAreaOffsetY = 0;

                    VoxelArea(voxelArea);
                }
            }
            else
            {
                if (voxelArea.gameObject.activeSelf)
                {
                    voxelAreaXZ = true;
                }
                else
                    voxelAreaXZ = false;
            }

            if (MouseButton(1, false) && !voxelAreaXZ)
            {
                //Instantiate(voxel, extractor.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);

                // Vector3 voxelPos = extractor.transform.position + new Vector3(Mathf.Sin(extractor.transform.eulerAngles.x * Mathf.Deg2Rad), 0.5f, 0);
                /*  if (mySwitchVoxelSettings.mode != 6)
                  {
                      BlockSwitch();
                  }
                  */
                Vector3 voxelPos = extractor.transform.position + extractor.transform.up * 0.5f;

                //Устранение погрешности путем округления (0.4999 -> 0.5)
                voxelPos.x = (int)((voxelPos.x + 0.01f) * 10) * 0.1f;
                voxelPos.y = (int)((voxelPos.y + 0.01f) * 10) * 0.1f;
                voxelPos.z = (int)((voxelPos.z + 0.01f) * 10) * 0.1f;

                if (gridControl.InGrid(voxelPos))
                {
                    // Instantiate(voxel, voxelPos, Quaternion.identity);

                    if (autoFit.active)
                    {

                        Voxelator.VoxelMeshGenerator voxelMeshGenerator = createInstanceVoxel.Create(voxelPos);

                       // AutoFitVertices(voxelMeshGenerator, false);

                        //voxelMeshGenerator.GetComponent<VoxelMaterials>().SetStandard(materialsControl.GetMaterialByCurIndex());
                    }
                    else
                    {
                        createInstanceVoxel.Create(voxelPos);
                    }
                }
            }
        }
    }

    private void ExtractorPos()
    {
        Vector3 point = raycastHit.point;

        if (raycastHit.transform.tag == "Voxel")
        {
            point = raycastHit.transform.position + extractor.transform.up * 0.5f;
        }
        else
        {
            point.x = (point.x - (int)point.x < 0.5f) ? (int)point.x : (int)point.x + 1;
            point.y = (point.y - (int)point.y < 0.5f) ? (int)point.y : (int)point.y + 1;
            point.z = (point.z - (int)point.z < 0.5f) ? (int)point.z : (int)point.z + 1;
        }

        extractor.transform.position = point;
    }

    private void ExtractorRot()
    {
        if (raycastHit.transform.tag != "Voxel")
            extractor.transform.rotation = Quaternion.identity;
        else
        {
            extractor.transform.rotation = Quaternion.FromToRotation(Vector3.up, raycastHit.normal);
        }
    }
}
