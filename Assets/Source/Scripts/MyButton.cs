using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace MyGUI
{
    public class MyButton : MonoBehaviour
    {
        public string description;
        public Color hover, selected, notSelected;

        public UnityEvent eventsButton;

        private RectTransform rectTransform;
        private Image image;

        public bool press;

        private DescriptionWidgets descriptionWidgets;

        void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            image = GetComponent<Image>();

            descriptionWidgets = GameObject.Find("Canvas").transform.Find("DescriptionWidgets").GetComponent<DescriptionWidgets>();
        }

        void Update()
        {
            if (InRect())
            {
                descriptionWidgets.SetTarget(description, transform);

                if (!press)
                    image.color = hover;
                if (Input.GetMouseButtonDown(0))
                {
                    image.color = selected;
                    press = true;
                }
                if (Input.GetMouseButtonUp(0))
                {
                    if (press)
                    {
                        eventsButton.Invoke();
                    }
                    press = false;
                }
            }
            else
            {
                descriptionWidgets.Hide(transform);
                image.color = notSelected;
                if (!Input.GetMouseButton(0))
                {
                    press = false;
                }
            }
        }

        private bool InRect()
        {
            Vector2 mousePos = Input.mousePosition;

            return mousePos.x > transform.position.x - rectTransform.rect.width * 0.5f &&
                mousePos.x < transform.position.x + rectTransform.rect.width * 0.5f &&
                mousePos.y > transform.position.y - rectTransform.rect.height * 0.5f &&
                mousePos.y < transform.position.y + rectTransform.rect.height * 0.5f;
        }
        
    }

 
}
