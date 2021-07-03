using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUnityEvents;

namespace NDR.UI
{
    public class InputField : MonoBehaviour
    {
        private UnityEngine.UI.InputField _inputField;

        public EventFloat InitEvent;

        public void SetValue(string text)
        {
            _inputField.text = text;
        }

        private void Awake()
        {
            _inputField = gameObject.GetComponentInParent<UnityEngine.UI.InputField>();
        }

        public void InitIntField()
        {
            int value;
            if (TryParseInt(out value))
            {
                InitEvent?.Invoke(value);
            }
        }

        private bool TryParseInt(out int value)
        {
            value = 0;
            return int.TryParse(_inputField.text, out value);
        }

        private bool TryParseFloat(out float value)
        {
            value = 0;
            return float.TryParse(_inputField.text, out value);
        }

       
    }
}