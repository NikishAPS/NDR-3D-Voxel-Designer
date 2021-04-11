using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : MonoBehaviour
{
    public RectTransform rectTransform { get; private set; }

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        
    }
}
