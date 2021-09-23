using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchSelector : MonoBehaviour
{
    private RectTransform _rectTransform;

    public void Set(RectTransform rectTransform)
    {
        Awake();
        _rectTransform.position = rectTransform.position;
        _rectTransform.sizeDelta = rectTransform.sizeDelta + Vector2.one* 3;
    }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
}
