using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageSystem : MonoBehaviour
{
    public Text msgText;
    [SerializeField]
    private List<Text> messages = new List<Text>();
    [SerializeField]
    private List<float> timeMessages = new List<float>();

    private void Start()
    {
        msgText.text = "";
    }

    void LateUpdate()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            AddMessage("Hello world");
        }

        for(int i = 0; i < messages.Count; i++)
        {
            messages[i].transform.localPosition = new Vector3(0f, (messages.Count - i) * messages[i].fontSize * 1.5f, 0f);
            timeMessages[i] -= Time.deltaTime;

            if(timeMessages[i] <= 0)
            {
                Color color = messages[i].color;

                color = new Color(color.r, color.g, color.b, Mathf.MoveTowards(color.a, 0, Time.deltaTime));

                messages[i].color = color;

                if (color.a == 0)
                {
                    Destroy(messages[i].gameObject);

                    messages.Remove(messages[i]);
                    timeMessages.Remove(timeMessages[i]);
                }
            }
        }
    }

    public void AddMessage(string message)
    {
        Text msg = Instantiate(msgText, Vector3.zero, Quaternion.identity);
        msg.transform.SetParent(transform);
        msg.transform.localPosition = Vector3.zero;

        msg.text = message;

        this.messages.Add(msg);
        timeMessages.Add(3f);
    }
}
