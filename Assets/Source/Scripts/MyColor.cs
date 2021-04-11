using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Voxelator;

namespace MyGUI
{
    public class MyColor : MonoBehaviour
    {
        public InputField hexValue;
        public RectTransform frame;
        public GameObject picker;

        private Image[] images;
        private Image image;
        private string[] hex;
        private MaterialsControl materialsControl;

        public int index;

        void Start()
        {
            images = new Image[transform.childCount - 1];
            hex = new string[images.Length];

            materialsControl = Camera.main.GetComponent<MaterialsControl>();
            for (int i = 0; i < images.Length; i++)
            {
                images[i] = transform.GetChild(i).GetComponent<Image>();
                hex[i] = "#FFFFFF";
                materialsControl.SetMaterialColor(i, images[i].color);
            }
            image = transform.GetChild(transform.childCount - 1).GetComponent<Image>();


            hex[0] = "#FF0000"; //red
            hex[1] = "#00FF00"; //green
            hex[2] = "#0000FF"; //blue
            hex[3] = "#FFFF00"; //yellow
            hex[4] = "#FFFFFF"; //white
            hex[5] = "#000000"; //black

            picker.SetActive(false);
        }

        public void SetMaterial()
        {
            materialsControl.SetMaterial(hex[index], index);
        }

        public void GetMaterial()
        {
            index = materialsControl.GetMaterialIndex();
            if (index == -1)
            {
                frame.position = image.transform.position;
                picker.SetActive(false);
            }
            else
            {
                frame.position = images[index].transform.position;
                picker.SetActive(true);
                GetHex();
            }

        }

        public void SetColor()
        {
            Color color;
            ColorUtility.TryParseHtmlString(hex[index], out color);
            images[index].color = color;
            hex[index] = hexValue.text;

            materialsControl.SetColor(index, images[index].color);
        }

        public void GetHex()
        {
            hexValue.text = hex[index];
        }

        public Material GetCurrentMaterial()
        {
            return materialsControl.GetMaterial(index);
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (InRect(image))
                {
                    index = -1;
                    materialsControl.ResetColor();
                    frame.position = image.transform.position;
                    picker.SetActive(false);
                }
            }

            for (int i = 0; i < images.Length; i++)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if(InRect(images[i]))
                    {
                        index = i;
                        hexValue.text = hex[index];
                        SetMaterial();
                        frame.position = images[index].transform.position;
                        picker.SetActive(true);
                        break;
                    }
                }
            }


            if (index != -1)
            {
                SetColor();
            }

        }


        private bool InRect(Image image)
        {
            Vector2 mousePos = Input.mousePosition;

            return mousePos.x > image.transform.position.x - image.rectTransform.rect.width * 0.5f &&
                mousePos.x < image.transform.position.x + image.rectTransform.rect.width * 0.5f &&
                mousePos.y > image.transform.position.y - image.rectTransform.rect.height * 0.5f &&
                mousePos.y < image.transform.position.y + image.rectTransform.rect.height * 0.5f;
        }
    }
}
