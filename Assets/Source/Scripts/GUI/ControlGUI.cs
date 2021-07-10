using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlGUI : MonoBehaviour
{
    [SerializeField]
    private int _curIndexPanel;
    private Panel[] _panels;


    public bool IsPanel => _curIndexPanel != -1;

    private void Awake()
    {
        _panels = FindObjectsOfType<Panel>();
    }

    private void Update()
    {
        _curIndexPanel = -1;

        for(int i = 0; i < _panels.Length; i++)
        {
            _panels[i].Tick();
            if (_panels[i].IsPanel) _curIndexPanel = i;
        }
    }

    public static bool InRect(RectTransform rectTransform)
    {
        Vector2 mousePos = Input.mousePosition;

        return mousePos.x > rectTransform.transform.position.x - rectTransform.rect.width * 0.5f &&
            mousePos.x < rectTransform.transform.position.x + rectTransform.rect.width * 0.5f &&
            mousePos.y > rectTransform.transform.position.y - rectTransform.rect.height * 0.5f &&
            mousePos.y < rectTransform.transform.position.y + rectTransform.rect.height * 0.5f;
    }
}
