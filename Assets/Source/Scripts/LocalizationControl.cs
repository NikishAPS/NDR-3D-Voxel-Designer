using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using MyGUI;

namespace Localization
{
    public class LocalizationControl : MonoBehaviour
    {
        public Text[] captions, textsDescriptions;
        public MyButton[] buttons;
        public MySwitch[] switches;
        public MyFlag[] flags;

        [SerializeField]
        private Data data = new Data();

        private void Awake()
        {
            LoadData("LocData.txt");
            LoadText();

            Destroy(gameObject);
        }

        private void LoadData(string name)
        {
            //File.WriteAllText(Application.dataPath + "/" + name, JsonUtility.ToJson(data)); return;

            string json = File.ReadAllText(Application.dataPath + "/" + name);
            data = JsonUtility.FromJson<Data>(json);
        }

        private void LoadText()
        {
            for (int i = 0; i < captions.Length; i++)
                captions[i].text = data.captions[i];

            for (int i = 0; i < textsDescriptions.Length; i++)
                textsDescriptions[i].text = data.textsDescriptions[i];

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].description = data.buttonsData[i].description;
                Text text = buttons[i].transform.GetChild(0).GetComponent<Text>();
                if (text != null)
                    text.text = data.buttonsData[i].inscription;
                else continue;
            }

            for (int i = 0; i < switches.Length; i++)
            {
                for (int j = 0; j < switches[i].description.Length; j++)
                {
                    switches[i].description[j] = data.switchesData[i].cases[j].description;
                    switches[i].transform.GetChild(j).GetChild(0).GetComponent<Text>().text =
                       data.switchesData[i].cases[j].inscription;
                }
            }



            for (int i = 0; i < flags.Length; i++)
            {
                flags[i].description = data.flagsData[i].description;
                flags[i].transform.GetChild(0).GetComponent<Text>().text = data.flagsData[i].inscription;
            }
        }
    }

    [System.Serializable]
    public class Data
    {
        //заголовки
        public string[] captions;

        //вспомогательные тексты
        public string[] textsDescriptions;

        //кнопки
        public ButtonData[] buttonsData;

        //переключатели
        public SwitchData[] switchesData;

        //флаги
        public FlagData[] flagsData;
    }

    [System.Serializable]
    public class ButtonData
    {
        public string inscription;
        public string description;
    }

    [System.Serializable]
    public class SwitchData
    {
        [SerializeField] public ButtonData[] cases;
    }

    [System.Serializable]
    public class FlagData
    {
        public string inscription;
        public string description;
    }
}