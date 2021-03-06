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

    public static bool Saved2
    {
        set
        {
            MonoBehaviour.print(value);
        }
    }

    [SerializeField] private Text statusText;

    public override void OnInit()
    {
        _this = PanelManager.GetPanel<StatisticsPanel>();
        Project.Saved.BindAction(OnSavedChange);
    }

    private void OnSavedChange(bool value)
    {
        Saved = value;
    }

}
