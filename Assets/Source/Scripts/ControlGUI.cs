using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlGUI : MonoBehaviour
{

    private Panel[] _panels;


    public bool IsPanel { get; private set; }

    private void Awake()
    {
        _panels = FindObjectsOfType<Panel>();
    }

    private void Update()
    {
        IsPanel = IsPanels();
    }


    private bool IsPanels()
    {
        for (int i = 0; i < _panels.Length; i++)
        {
            if (_panels[i].gameObject.activeSelf && _panels[i].IsPanel)
            {
                return true;
            }
        }
        return false;
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
