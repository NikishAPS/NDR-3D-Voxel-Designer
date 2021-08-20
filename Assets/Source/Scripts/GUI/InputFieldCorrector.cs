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
        }
    }
}
