using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RectTransformExtention
{
    public static bool Inside(this RectTransform rectTransform, Vector2 point)
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            MonoBehaviour.print(rectTransform.gameObject.name);
            MonoBehaviour.print(rectTransform.position);
            MonoBehaviour.print(point);
        }
        return 
            point.x > rectTransform.position.x - rectTransform.rect.width * 0.5f &&
            point.x < rectTransform.position.x + rectTransform.rect.width * 0.5f &&
            point.y > rectTransform.position.y - rectTransform.rect.height * 0.5f &&
            point.y < rectTransform.position.y + rectTransform.rect.height * 0.5f;

    }
}
