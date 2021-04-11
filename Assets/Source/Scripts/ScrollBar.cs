using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGUI
{
    public class ScrollBar : MonoBehaviour
    {
        public RectTransform window;
        public RectTransform background;
        public RectTransform bar;

        public Vector2 windowPos;

        public Vector2 windowClampPos;

        private Vector2 saveWindowPos;
        private Vector2 saveMousePos;
        public Vector2 offset;

        private bool capture;

        public Vector2 fff;

        void Start()
        {
            background = transform.Find("Background").GetComponent<RectTransform>();
            bar = background.Find("Bar").GetComponent<RectTransform>();
        }

        void Update()
        {
            Vector2 mousePos = (Vector2)Input.mousePosition - new Vector2(0, Screen.height * 0.5f);
            fff = mousePos;

            if (window.rect.height > Screen.height)
            {
                background.gameObject.SetActive(true);
                bar.gameObject.SetActive(true);

                if (InRect(background))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        saveWindowPos = windowPos;
                        saveMousePos = Input.mousePosition;
                        offset = mousePos - (Vector2)bar.localPosition;
                        capture = true;
                    }

                    
                }

                if(capture)
                {
                    if (Input.GetMouseButton(0))
                    {
                        Vector2 delta = (Vector2)Input.mousePosition - saveMousePos;
                        windowPos = saveWindowPos - delta;
                        windowPos = -mousePos - offset;
                    }
                    else
                    {
                        capture = false;
                    }
                }

                //window
                windowClampPos.y = Mathf.Abs(Screen.height - window.rect.height) * 0.5f;
                windowPos.y = Mathf.Clamp(windowPos.y, -windowClampPos.y, windowClampPos.y);
                window.localPosition = new Vector2(window.localPosition.x, windowPos.y);


                //background
                background.sizeDelta = new Vector2(background.sizeDelta.x, Screen.height - 10);
                transform.position = new Vector2(transform.position.x, Screen.height * 0.5f);

                //bar
                bar.sizeDelta = new Vector2(background.sizeDelta.x - 5, background.sizeDelta.y * (Screen.height / window.sizeDelta.y));
                bar.localPosition = new Vector2(bar.localPosition.x, -(background.rect.height - bar.rect.height - 5) * 0.5f * (windowPos.y / windowClampPos.y));

            }
            else
            {
                window.localPosition = new Vector2(window.localPosition.x, 0);
                background.gameObject.SetActive(false);
            }
        }

        public bool InRect(RectTransform rectTransform)
        {
            Vector2 mousePos = Input.mousePosition;

            return mousePos.x > rectTransform.transform.position.x - rectTransform.rect.width * 0.5f &&
                mousePos.x < rectTransform.transform.position.x + rectTransform.rect.width * 0.5f &&
                mousePos.y > rectTransform.transform.position.y - rectTransform.rect.height * 0.5f &&
                mousePos.y < rectTransform.transform.position.y + rectTransform.rect.height * 0.5f;
        }


    }
}
