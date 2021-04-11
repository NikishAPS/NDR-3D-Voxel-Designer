using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionWidgets : MonoBehaviour
{
    public Vector2 offset;

    private Text text;
    private Image background;

    private bool show;

    private float alpha; //прозрачность текста

    private Transform target;  //обработка теущего виджета

    private float time;

    void Start()
    {
        background = transform.GetChild(0).GetComponent<Image>();
        text = transform.GetChild(1).GetComponent<Text>();

        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        background.color = new Color(background.color.r, background.color.g, background.color.b, 0);
    }

    void Update()
    {
        
        if(target != null)
        {
            time += Time.deltaTime;
            if(time >= 0.5f)
            alpha = Mathf.MoveTowards(alpha, 1, Time.deltaTime * 2f);

            if (!target.gameObject.active) target = null;
        }
        else
        {
            alpha = Mathf.MoveTowards(alpha, 0, Time.deltaTime * 2f);
            time = 0;
        }

        text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
        background.color = new Color(background.color.r, background.color.g, background.color.b, alpha * 0.8f);
    }

    public void Hide(Transform target)
    {
        if (this.target == target)
            this.target = null;
    }

    public void SetTarget(string text, Transform target)
    {
        if (alpha != 0 || text == string.Empty) return;

        transform.position = (Vector2)Input.mousePosition + offset;
        this.text.text = text;
        this.target = target;

        SetSizeBackground();
    }
    public float size;
    private void SetSizeBackground()
    {
        float x = size * text.text.Length;
        background.rectTransform.sizeDelta = new Vector2(x, background.rectTransform.sizeDelta.y);
        background.rectTransform.localPosition = new Vector2(x * 0.5f, background.rectTransform.localPosition.y);
    }
}
