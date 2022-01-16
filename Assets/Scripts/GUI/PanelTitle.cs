using UnityEngine;
using UnityEngine.UI;

public class PanelTitle : MonoBehaviour
{
    public bool Active { get => gameObject.activeSelf; set => gameObject.SetActive(value); }
    public Text TextComponent { get; private set;}
    public string TextValue { get => TextComponent.text; set => TextComponent.text = value; }

    private void Awake()
    {
        TextComponent = GetComponentInChildren<Text>();
    }
}