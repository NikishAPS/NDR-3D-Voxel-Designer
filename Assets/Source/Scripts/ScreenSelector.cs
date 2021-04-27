using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenSelector : MonoBehaviour
{
    private RectTransform rectTransform;
    private Image image;

    private Vector2 startPos;

    void Start()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }

    void Update()
    {
        //startPos = new Vector3(Screen.width, Screen.height) * 0.5f;
        Vector2 mouse = Input.mousePosition;

        Vector2 dir = mouse - startPos;
        rectTransform.position = dir * 0.5f + startPos;

        rectTransform.sizeDelta = new Vector3(Mathf.Abs(dir.x), Mathf.Abs(dir.y));
    }

    public void Stop()
    {
        gameObject.SetActive(false);
    }

    public void Run()
    {
        gameObject.SetActive(true);
        startPos = Input.mousePosition;
        Update();
    }
}
