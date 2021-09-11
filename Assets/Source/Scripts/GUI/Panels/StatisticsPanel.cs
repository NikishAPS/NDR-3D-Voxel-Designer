using UnityEngine;
using UnityEngine.UI;

public class StatisticsPanel : Panel
{
    public bool Saved
    {
        set
        {
            if(value)
            {
                statusText.text = "Saved";
                statusText.color = Color.green;
            }
            else
            {
                statusText.text = "Not Saved";
                statusText.color = Color.red;
            }
        }
    }

    [SerializeField] private Text statusText;

}
