using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyGUI
{
    public class MySlider : MonoBehaviour
    {
        public float step = 0.5f;
        public float value, stepValue;
        public float min, max;

        private RectTransform rectTransform;
        private Image background, fill, handle;


        private Text textValue;

        private bool capture;

        private Vector2 mousePos;

        public bool update = true;

        void Start()
        {
            rectTransform = GetComponent<RectTransform>();

            background = transform.Find("Background").GetComponent<Image>();
            fill = transform.Find("Fill").GetComponent<Image>();
            handle = transform.Find("Handle").GetComponent<Image>(); //handleRect = handle.transform.GetComponent<RectTransform>();




            textValue = transform.Find("TextValue").GetComponent<Text>();
            // slider = GetComponent<Slider>();
        }

        void Update()
        {
            textValue.text = (stepValue * 1.0f / step).ToString();

            Vector2 mouseSpeed = (Vector2)Input.mousePosition - mousePos;
            mousePos = Input.mousePosition;

            if (Input.GetMouseButtonDown(0))
            {
                if (InRect())
                {
                    capture = true;
                }
            }

            if (Input.GetMouseButton(0))
            {
                if(capture)
                {
                    float mouseX = Input.mousePosition.x - transform.position.x;
                    mouseX = Mathf.Clamp(mouseX, -rectTransform.rect.width * 0.5f, rectTransform.rect.width * 0.5f);

                    handle.rectTransform.localPosition = new Vector3(mouseX, handle.rectTransform.localPosition.y, handle.rectTransform.localPosition.z);
                    value = min + (max - min) * ((handle.rectTransform.localPosition.x + rectTransform.rect.width * 0.5f) / rectTransform.rect.width);
                }
            }
            else
            {
                capture = false;
                //value = stepValue;
            }


            UpdateValue();
            


            float per = ((stepValue - min) / (max - min) - 0.5f);
            handle.rectTransform.localPosition = new Vector3(per * rectTransform.rect.width, handle.rectTransform.localPosition.y, handle.rectTransform.localPosition.z);

            fill.rectTransform.localPosition = new Vector3((handle.rectTransform.localPosition.x - rectTransform.rect.width * 0.5f) * 0.5f, fill.rectTransform.localPosition.y, fill.rectTransform.localPosition.z);
            fill.rectTransform.sizeDelta = new Vector2(per * rectTransform.rect.width, fill.rectTransform.sizeDelta.y);
        }

        public void SetStepValue(float value)
        {
            this.value = value;
            UpdateValue();
        }

        public void AddValue(float value)
        {
            this.value += value;
            UpdateValue();
        }

        private void UpdateValue()
        {
            value = Mathf.Clamp(value, min, max);

            if (value > 0)
            {
                if (value % step < 0.25f)
                {
                    stepValue = value - value % step;
                }
                else
                {
                    stepValue = value + (step - value % step);
                }
            }
            else if (value < 0)
            {
                if (value % step > -0.25f)
                {
                    stepValue = value - value % step;
                }
                else
                {
                    stepValue = value - (step + value % step);

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

        public void SetValue(float value)
        {
            this.value = value;
            stepValue = value;
            UpdateValue();
        }

        public void SetStep(float step)
        {
            this.step = step;
        }

        public void SetLimit(float limit)
        {
            min = -limit;
            max = limit;
        }
    }
}
