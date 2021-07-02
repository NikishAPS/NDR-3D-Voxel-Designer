using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : Mode
{
    public override void Tick()
    {
        SceneData.Extractor.SetActive(false);

        if (!SceneData.ControlGUI.IsPanel)
        {
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //RaycastHit hit;

            CastResult castResult = VoxelRayCast.CastByMouse(SceneData.RayLength);
            if (castResult != null)
            {

                if (SceneData.Chunk.InChunk(SceneData.Vector3FloatToInt(castResult.lastPoint)))
                {
                    SceneData.Extractor.SetActive(true);
                    SceneData.Extractor.transform.position = castResult.lastPoint;

                    if (SceneData.EventInput.MouseDown0)
                    {
                        int id = SceneData.ColorTest.id;
                        SceneData.Chunk.CreateVoxel(SceneData.ColorTest.id, castResult.lastPoint);
                    }
                }
            }
        }
    }

    public override void Disable()
    {
        SceneData.Extractor.SetActive(false);
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
