using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyGUI
{
    public class MyFlag : MonoBehaviour
    {
        public string description;
        private DescriptionWidgets descriptionWidgets;


        public bool active = true;

        private RectTransform rectTransform;
        private Image image;

        private 

        void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            image = GetComponent<Image>();

            image.enabled = active;

            descriptionWidgets = GameObject.Find("Canvas").transform.Find("DescriptionWidgets").GetComponent<DescriptionWidgets>();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (InRect())
                {
                    active = !active;
                }
            }

            if (InRect())
            {
                descriptionWidgets.SetTarget(description, transform);
            }
            else
            {
                descriptionWidgets.Hide(transform);
            }

                image.enabled = active;
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