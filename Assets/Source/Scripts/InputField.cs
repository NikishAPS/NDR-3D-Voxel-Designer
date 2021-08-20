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
        public EventObject InitField;
        public EventString ValueChanged;
        public EventString EndEdit;

        public int Int
        {
            get => ParseInt();
            set => _inputField.text =value.ToString();
        }
        public string String
        {
            get => ParseString();
            set => _inputField.text = value;
        }



        public void SetValue(string text)
        {
            _inputField.text = text;
        }



        public void OnInitField(object obj)
        {
            InitField?.Invoke(obj);
        }

        public void OnInitIntFields()
        {
            int value;
            if (TryToParseInt(out value))
            {
                InitIntFields?.Invoke(value);
            }
        }

        public void OnInitFloatFields()
        {
            float value;
            if(TryToParseFloat(out value))
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


        private void Awake()
        {
            _inputField = GetComponentInChildren<UnityEngine.UI.InputField>();
        }

        private int ParseInt()
        {
            int value;
            TryToParseInt(out value);

            return value;
        }

        private string ParseString()
        {
            return _inputField.text;
        }

        private bool TryToParseInt(out int value)
        {
            value = 0;
            return int.TryParse(_inputField.text, out value);
        }

        private bool TryToParseFloat(out float value)
        {
            value = 0;
            return float.TryParse(_inputField.text, out value);
        }

    }
}