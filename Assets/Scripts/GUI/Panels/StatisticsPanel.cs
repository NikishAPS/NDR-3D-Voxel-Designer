using UnityEngine;
using UnityEngine.UI;

public class StatisticsPanel : Panel
{
    private static StatisticsPanel _this;

    public static bool Saved
    {
        set
        {
            if(value)
            {
                _this.statusText.text = "Saved";
                _this.statusText.color = Color.green;
            }
            else
            {
                _this.statusText.text = "Not Saved";
                _this.statusText.color = Color.red;
            }
        }
    }

    [SerializeField] private Text statusText;

    public override void OnInit()
    {
        _this = PanelManager.GetPanel<StatisticsPanel>();
    }

}
