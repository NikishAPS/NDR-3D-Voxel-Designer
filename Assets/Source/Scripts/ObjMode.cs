using UnityEngine;
using System.Collections;

public class ObjMode : Mode
{
    public override void Disable()
    {
        SceneData.DragSystem.SetActive(false);
        InputEvent.LMouseDown -= OnLMouseDown;
    }

    public override void Enable()
    {
        InputEvent.LMouseDown += OnLMouseDown;
    }

    public void OnLMouseDown()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            if(hit.transform.CompareTag("OBJ"))
            {
                SceneData.DragSystem.SetPosition(hit.transform.position);
                SceneData.DragSystem.SetActive(true);
            }
        }
    }

    public override void Tick()
    {
        //throw new System.NotImplementedException();
    }
}
