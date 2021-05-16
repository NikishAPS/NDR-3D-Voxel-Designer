using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : Mode
{
    public override void Tick()
    {
        SceneData.extractor.SetActive(false);

        if (!SceneData.controlGUI.IsPanel)
        {
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //RaycastHit hit;

            CastResult castResult = VoxelRayCast.CastByMouse(SceneData.rayLength);
            if (castResult != null)
            {

                if (SceneData.chunk.InChunk(SceneData.Vector3FloatToInt(castResult.lastPoint)))
                {
                    SceneData.extractor.SetActive(true);
                    SceneData.extractor.transform.position = castResult.lastPoint;

                    if (SceneData.eventInput.MouseDown0)
                    {
                        int id = SceneData.colorTest.id;
                        SceneData.chunk.CreateVoxel(SceneData.colorTest.id, castResult.lastPoint);
                    }
                }
            }
        }
    }

    public override void Disable()
    {
        SceneData.extractor.SetActive(false);
    }

    public override void Enable()
    {

    }


    //public void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawLine(posRay, posRay + dirRay * l);
    //}

}
