using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldControl : MonoBehaviour
{
    private InputField inputField;

    private void Awake()
    {
        inputField = GetComponent<InputField>();
    }

    private void LateUpdate()
    {
        if (inputField.text.Length > 0 && inputField.text[0] == '-')
        {
            inputField.text = inputField.text.Replace("-", "");
        }
    }
}
