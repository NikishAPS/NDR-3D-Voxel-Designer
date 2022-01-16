using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldCorrector : MonoBehaviour
{
    public void CorrectIntegerInputField(InputField inputField)
    {
        if (inputField.text.Length > 0 && inputField.text[0] == '-')
        {
            inputField.text = inputField.text.Replace("-", "");
        }

        int value;
        if (int.TryParse(inputField.text, out value))
        {
            if (value > 512)
                inputField.text = "512";
            else if (value == 0)
                inputField.text = "1";
        }
    }

    public void CorrectNameField(InputField inputField)
    {
        string value = inputField.text;

        if (char.IsDigit(value[0]))
        {
            for (int i = 0; i < value.Length; i++)
            {
                if (!char.IsDigit(value[i]))
                {
                    string newValue = string.Empty;
                    for (; i < value.Length; i++)
                    {
                        newValue += value[i];
                    }
                    inputField.text = newValue;
                    return;
                }
            }
        }
    }

}
