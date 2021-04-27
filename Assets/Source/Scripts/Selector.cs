using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : Mode
{
    public override void Invoke()
    {
        SceneData.extractor.SetActive(false);
        if (!SceneData.controlGUI.IsPanel)
        {
            CastResult castResult = RayCaster.CastByMouse(SceneData.rayLength);
            if(castResult != null)
            {
                if (castResult.voxel != null)
                {
                    SceneData.extractor.SetActive(true);
                    SceneData.extractor.SetPosition(castResult.point);
                }

                if (SceneData.eventInput.Mouse0)
                {
                    //SceneData.chunk.SelectVoxel(castResult.point);
                }
            }
        }
    }
}
