using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace MyGUI
{
    public class MySwitch : MonoBehaviour
    {
        public string[] description;
        public int mode;
        public Color hover, selected, notSelected, unavailable;

        public GameObject[] options;
        public bool[] active;

        private RectTransform[] rectTransforms;
        private Image[] images;

        public UnityEvent[] eventOnce;

        private DescriptionWidgets descriptionWidgets;

        void Start()
        {
            options = new GameObject[transform.childCount];
            rectTransforms = new RectTransform[options.Length];
            images = new Image[options.Length];

            for(int i = 0; i < options.Length; i++)
            {
                options[i] = transform.GetChild(i).gameObject;
                rectTransforms[i] = options[i].GetComponent<RectTransform>();
                images[i] = options[i].GetComponent<Image>();
            }

            active = new bool[options.Length];
            for(int i = 0; i < active.Length; i++)
            {
                active[i] = true;
            }

            eventOnce[mode - 1].Invoke();

            descriptionWidgets = GameObject.Find("Canvas").transform.Find("DescriptionWidgets").GetComponent<DescriptionWidgets>();

            if(description.Length == 0)
            {
                description = new string[options.Length];
            }
        }

        void Update()
        {
            for (int i = 0; i < options.Length; i++)
            {
                if (active[i])
                {
                    if (InRect(i))
                    {
                        descriptionWidgets.SetTarget(description[i], images[i].transform);
                        if (Input.GetMouseButtonDown(0))
                        {
                            images[i].color = selected;
                            mode = i + 1;
                            eventOnce[i].Invoke();

                        }
                        else if (mode - 1 != i)
                        {
                            images[i].color = hover;
                        }
                    }
                    else
                    {
                        if (mode - 1 != i)
                            images[i].color = notSelected;
                        else images[mode - 1].color = selected;

                        descriptionWidgets.Hide(images[i].transform);
                    }
                }
                else
                {
                    images[i].color = unavailable;
                }
            }
        }

        private bool InRect(int index)
        {
            Vector2 mousePos = Input.mousePosition;

            return mousePos.x > options[index].transform.position.x - rectTransforms[index].rect.width * 0.5f &&
                mousePos.x < options[index].transform.position.x + rectTransforms[index].rect.width * 0.5f &&
                mousePos.y > options[index].transform.position.y - rectTransforms[index].rect.height * 0.5f &&
                mousePos.y < options[index].transform.position.y + rectTransforms[index].rect.height * 0.5f;
        }
    }

}
