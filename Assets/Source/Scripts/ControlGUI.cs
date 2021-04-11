using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlGUI : MonoBehaviour
{

    public Panel[] panels;

    private bool cursorInGameScene;

    private void Awake()
    {
        panels = FindObjectsOfType<Panel>();
    }

    private void Update()
    {
        
    }


    private void SetCursorInGameScene()
    {
        cursorInGameScene = false;
        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i].gameObject.activeSelf && InPanel(panels[i].rectTransform))
            {
                cursorInGameScene = false;
                break;
            }
        }
    }

    private bool InPanel(RectTransform rectTransform)
    {
        Vector2 mousePos = Input.mousePosition;

        return mousePos.x > rectTransform.transform.position.x - rectTransform.rect.width * 0.5f &&
            mousePos.x < rectTransform.transform.position.x + rectTransform.rect.width * 0.5f &&
            mousePos.y > rectTransform.transform.position.y - rectTransform.rect.height * 0.5f &&
            mousePos.y < rectTransform.transform.position.y + rectTransform.rect.height * 0.5f;
    }
}
