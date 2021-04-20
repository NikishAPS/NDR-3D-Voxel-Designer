using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    private void Update()
    {
        if (!SceneData.controlGUI.IsPanel)
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 10000))
            {
                Vector3 pos = Vector3.zero;

                pos.x = (hit.normal.x == -1) ? (int)(hit.point.x - 0.5f) : (int)(hit.point.x + 0.5f);
                pos.y = (hit.normal.y == -1) ? (int)(hit.point.y - 0.5f) : (int)(hit.point.y + 0.5f);
                pos.z = (hit.normal.z == -1) ? (int)(hit.point.z - 0.5f) : (int)(hit.point.z + 0.5f);

                SceneData.extractor.transform.position = pos;
                SceneData.extractor.SetRotation(Quaternion.FromToRotation(Vector3.up, hit.normal));


                if (Input.GetMouseButtonDown(0))
                {
                    if (SceneData.chunk.InChunk(SceneData.Vector3FloatToInt(pos)))
                    {
                        SceneData.chunk.CreateVoxel(pos);
                    }
                }
            }
        }
    }
}
