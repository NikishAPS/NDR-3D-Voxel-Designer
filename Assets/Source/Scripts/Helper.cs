using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGUI;

namespace Voxelator
{
    public class Helper : MonoBehaviour
    {
        private CursorPanelsGUI cursorPanelsGUI;

        void Start()
        {
            cursorPanelsGUI = GetComponent<CursorPanelsGUI>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                cursorPanelsGUI.windowGUI[3].panel.gameObject.SetActive(!cursorPanelsGUI.windowGUI[3].panel.gameObject.active);
            }
        }
    }
}
