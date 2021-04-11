using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyGUI
{
    public class CursorPanelsGUI : MonoBehaviour
    {

        public bool cursorInGameScene;

        public WindowGUI[] windowGUI;


        void Start()
        {

        }

        void Update()
        {
            cursorInGameScene = CursorInGameScene();
        }

        private bool CursorInGameScene()
        {
            bool notPanel = true;
            for(int i = 0; i < windowGUI.Length; i++)
            {
                if(InPanel(windowGUI[i].panel) && windowGUI[i].panel.gameObject.active)
                {
                    notPanel = false;
                    break;
                }
            }
            return notPanel;
        }

        private bool InPanel(RectTransform rectTransform)
        {
            Vector2 mousePos = Input.mousePosition;

            return mousePos.x > rectTransform.transform.position.x - rectTransform.rect.width * 0.5f &&
                mousePos.x < rectTransform.transform.position.x + rectTransform.rect.width * 0.5f &&
                mousePos.y > rectTransform.transform.position.y - rectTransform.rect.height * 0.5f &&
                mousePos.y < rectTransform.transform.position.y + rectTransform.rect.height * 0.5f;
        }

        public void WindowActive(int windowIndex, bool active)
        {
            windowGUI[windowIndex].panel.gameObject.SetActive(active);
        }

        public void CaptionActive(int windowIndex, int captionIndex, bool active)
        {
            windowGUI[windowIndex].caption[captionIndex].gameObject.SetActive(active);
        }
    }

    [System.Serializable]
    public struct WindowGUI
    {
        public RectTransform panel;
        public RectTransform[] caption;
    }
}
