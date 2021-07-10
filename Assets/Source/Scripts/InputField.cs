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

        public EventInt InitIntFields;
        public EventFloat InitFloatFields;
        public EventString InitStringFields;
        public EventString ValueChanged;
        public EventString EndEdit;

        public void SetValue(string text)
        {
            _inputField.text = text;
        }

        private void Awake()
        {
            _inputField = GetComponentInChildren<UnityEngine.UI.InputField>();
        }

        public void OnInitIntFields()
        {
            int value;
            if (TryParseInt(out value))
            {
                InitIntFields?.Invoke(value);
            }
        }

        public void OnInitFloatFields()
        {
            float value;
            if(TryParseFloat(out value))
            {
                InitFloatFields?.Invoke(value);
            }
        }

        public void OnInitStringFields()
        {
            InitStringFields?.Invoke(_inputField.text);
        }

        public void OnValueChanged(string value)
        {
            ValueChanged?.Invoke(value);
        }

        public void OnEndEdit(string value)
        {
            EndEdit?.Invoke(value);
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