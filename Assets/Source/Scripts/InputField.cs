using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NDR.UI
{
    public class InputField : MonoBehaviour
    {
        UnityEngine.UI.InputField _inputField;

        public void SetValue(string text)
        {
            _inputField.text = text;
        }

        private void Awake()
        {
            _inputField = gameObject.GetComponentInParent<UnityEngine.UI.InputField>();
        }
    }
}